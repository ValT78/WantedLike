using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Sprites")]
    [SerializeField] private GameObject spritePrefab; // Prefab de l'objet qui contiendra les sprites
    public int nombreDeSprites; // Nombre de sprites à générer par round
    [SerializeField] private SpawnSpriteManager spawnSpriteManager;
    [SerializeField] private Sprite[] sprites; // Tableau de sprites des objets recherchables
    public RectTransform zoneSpawnSprite; // Transform de la zone où les sprites seront générés

    [Header("Affiche")]
    public Image affiche; // SpriteRenderer où le sprite à rechercher sera affiché
    private int spriteRechercheID; // L'ID du sprite à rechercher
    private GameObject spriteRecherche; // Le sprite à rechercher

    [Header("Detect clic")]
    [SerializeField] private Canvas canvas;
    private GraphicRaycaster raycaster;
    private EventSystem eventSystem;

    [Header("Score")]
    [SerializeField] private ScoreAnimator scoreDisplay;
    [SerializeField] private GameObject bdaPointPrefab;
    [HideInInspector] public int score;

    [Header("Animations")]
    [SerializeField] private float blinkSpeedWrongTarget = 10; // Blinks per second
    [SerializeField] private float blinkNumberWrongTarget = 10; // Total number of blinks

    [Header("Chrono")]
    [SerializeField] private FloatingTime bonusChronoPrefab;
    [SerializeField] private float chronoInitial;
    public float chronoBonus;
    public float chronoMalus;
    [SerializeField] private float chronoMax;
    [SerializeField] private TextMeshProUGUI chronoText;
    [SerializeField] private ChronoAnimator chronoAnimator;
    private float chrono;
    private bool displayAnimation;

    [Header("EndGame")]
    [SerializeField] private GameObject gameOverText;
    [SerializeField] private ScoreAnimator finalBlinkingScore;
    private bool isGameOver;

    private void Awake()
    {
        chrono = chronoInitial;

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        raycaster = canvas.GetComponent<GraphicRaycaster>();
        eventSystem = EventSystem.current;
        LaunchNewRound();
        scoreDisplay.BlinkText("0", 4);
    }

    void Update()
    {
        DetectClicOnScreen();

        if(!isGameOver && !displayAnimation)
        {
            UpdateChrono();
        }
    }

    private IEnumerator FindTargetSprite(SpriteClickable target)
    {
        displayAnimation = true;
        DestroyAllSprites(target.transform);
        yield return new WaitForSeconds(1f);
        displayAnimation = false;
        LaunchNewRound();
    }

    private IEnumerator MissedTarget(SpriteClickable target)
    {
        Image image = target.GetComponent<Image>();
        Color color = image.color;

        // Faire clignoter le sprite manqué de invisible à visible sans retirer l'ancienne couleur
        for (int i = 0; i < blinkNumberWrongTarget*2; i++)
        {
            color.a = i % 2 == 0 ? 0 : 1;

            if (!image)
                yield break;

            image.color = color;
            yield return new WaitForSeconds(1 / blinkSpeedWrongTarget);
        }

        if (!image)
            yield break;

        color.a = 1;
        image.color = color;
    }

    private void LaunchNewRound()
    {
        // Choisir un sprite aléatoire pour l'affiche
        int index = Random.Range(0, sprites.Length);
        spriteRechercheID = index;

        // Afficher le sprite sur l'affiche
        affiche.sprite = sprites[index];

        DestroyAllSprites(); // Détruire tous les sprites générés
        nombreDeSprites = (int)(5 * Mathf.Log(score / 3 + 1.6f, 2));
        // Sélectionner les transformations
        spawnSpriteManager.ChooseTransformation(score);

        // Ajouter le sprite recherché parmi les sprites générés
        spriteRecherche = SummonSprite(index);

        // Générer les nouveaux sprites
        for (int i = 0; i < nombreDeSprites - 1; i++) // Générer un sprite de moins
        {
            SummonSprite();
        }
        SortSpriteById();
    }

    private GameObject SummonSprite(int spriteIndex = -1)
    {
        GameObject spriteObject = Instantiate(spritePrefab);
        while (spriteIndex == -1)
        {
            spriteIndex = Random.Range(0, sprites.Length);
            if (spriteIndex == spriteRechercheID)
            {
                spriteIndex = -1;
            }
        }
        spriteObject.GetComponent<Image>().sprite = sprites[spriteIndex];
        if (spriteIndex == spriteRechercheID)
        {
            spawnSpriteManager.ApplyTransformations(spriteObject, true, spriteIndex);
        }
        else
        {
            spawnSpriteManager.ApplyTransformations(spriteObject, false, spriteIndex);
        }
        return spriteObject;

    }

    public bool OnSpriteClicked(int clickedSpriteID)
    {
        return clickedSpriteID == spriteRechercheID;
    }

    private void DetectClicOnScreen()
    {
        
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            if (isGameOver)
            {
                SceneManager.LoadScene("Menu");
            }
            Vector2 touchPosition = Input.mousePosition;
            if (Input.touchCount > 0)
            {
                touchPosition = Input.GetTouch(0).position;
            }
            Vector2 normalizedTouchPosition = Camera.main.ScreenToViewportPoint(touchPosition);
            PointerEventData pointerEventData = new(eventSystem)
            {
                position = touchPosition
            };

            List<RaycastResult> results = new();
            raycaster.Raycast(pointerEventData, results);

            bool isTargetClicked = false;
            SpriteClickable spriteClickable = null;
            foreach (RaycastResult result in results.Reverse<RaycastResult>())
            {
                if (result.gameObject.TryGetComponent<SpriteClickable>(out var clickable))
                {
                    if(clickable.isClicked) continue;
                    isTargetClicked = OnSpriteClicked(clickable.spriteID);
                    spriteClickable = clickable;
                    if (isTargetClicked) break;
                    
                }
            }
            if (spriteClickable != null)
            {
                spriteClickable.isClicked = true;
                if (isTargetClicked)
                {
                    UpdateScore();
                    FloatingTime chronoInstance = Instantiate(bonusChronoPrefab, Vector3.zero, Quaternion.identity, canvas.transform);
                    chronoInstance.Initialized(true, normalizedTouchPosition);

                    StartCoroutine(Instantiate(bdaPointPrefab, Camera.main.ScreenToWorldPoint(touchPosition), Quaternion.identity).GetComponent<HyperbolicTrajectory>().MoveObject(scoreDisplay.transform.position + new Vector3(1f, 0f, 0f)));
                    chronoAnimator.AnimateBonus((int)Mathf.Ceil(chrono), (int)Mathf.Ceil(Mathf.Min(chrono + chronoBonus, chronoMax)), .65f);
                    chrono = Mathf.Min(chrono + chronoBonus, chronoMax);
                    StartCoroutine(FindTargetSprite(spriteClickable));
                }
                else
                {
                    FloatingTime chronoInstance = Instantiate(bonusChronoPrefab, Vector3.zero, Quaternion.identity, canvas.transform);
                    chronoInstance.Initialized(false, normalizedTouchPosition);

                    chrono = Mathf.Max(chrono - chronoMalus, 0);
                    StartCoroutine(MissedTarget(spriteClickable));
                }
            }
            
        }
    }   

    private void SortSpriteById()
    {
        // Trier les enfants par spriteID
        var sortedChildren = zoneSpawnSprite.GetComponentsInChildren<SpriteClickable>().OrderBy(child => child.spriteID).ToArray();

        // Réorganiser les enfants dans la hiérarchie
        for (int i = 1; i <= sortedChildren.Length; i++)
        {
            sortedChildren[i-1].transform.SetSiblingIndex(i);
        }

        spriteRecherche.transform.SetSiblingIndex(Mathf.CeilToInt(GenerateExponentialRandom(20/(score+1)) * sortedChildren.Length));
    }

    private void DestroyAllSprites(Transform exception = null)
    {
        foreach (Transform child in zoneSpawnSprite)
        {
            if (child != exception)
            {
                Destroy(child.gameObject);
            }
        }
    }

    private void UpdateScore()
    {
        score++;
        scoreDisplay.BlinkText(score.ToString(), 4);
        scoreDisplay.PulseLogoGrow();
    }

    private void UpdateChrono()
    {
        int oldInteger = Mathf.CeilToInt(chrono);
        chrono -= Time.deltaTime;
        bool tick = Mathf.CeilToInt(chrono) != oldInteger;
        chronoAnimator.UpdateTime(chrono, tick);

        if (chrono <= 0)
        {
            chrono = 0;
            GameOver();
        }
    }

    private void GameOver()
    {
        isGameOver = true;
        finalBlinkingScore.BlinkText("Final Score : " + score + "\nPress to Continue");
        Instantiate(gameOverText);
        DestroyAllSprites();
    }

    float GenerateExponentialRandom(float lambda)
    {
        float u = Random.value; // Génère un nombre aléatoire uniforme entre 0 et 1
        float result = 1 - Mathf.Exp(-u/lambda); // Appliquer ta fonction avec la nouvelle abscisse
        return result;
    }
}
