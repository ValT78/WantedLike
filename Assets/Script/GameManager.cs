using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
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
    private List<GameObject> spriteRecherches; // Le sprite à rechercher
    [SerializeField] private ScoreAnimator howManySpriteText;

    [Header("Detect clic")]
    [SerializeField] private Canvas canvas;
    private GraphicRaycaster raycaster;
    private EventSystem eventSystem;
    [HideInInspector] public int howManySpriteToFind;

    [Header("Score")]
    [SerializeField] private ScoreAnimator scoreDisplay;
    [SerializeField] private GameObject bdaPointPrefab;
    [HideInInspector] public int score;

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

    private IEnumerator BlinkSprite(SpriteClickable target, int blickNumber, float blinkTime)
    {
        Image image = target.GetComponent<Image>();
        Color color = image.color;

        // Faire clignoter le sprite manqué de invisible à visible sans retirer l'ancienne couleur
        for (int i = 0; i < blickNumber; i++)
        {
            color.a = i % 2 == 0 ? 0 : 1;

            if (!image)
                yield break;

            image.color = color;
            yield return new WaitForSeconds(blinkTime);
        }
    }

    private void LaunchNewRound()
    {
        // Choisir un sprite aléatoire pour l'affiche
        int index = Random.Range(0, sprites.Length);
        spriteRechercheID = index;

        // Afficher le sprite sur l'affiche
        affiche.sprite = sprites[index];

        DestroyAllSprites(); // Détruire tous les sprites générés
        nombreDeSprites = (int)(7 * Mathf.Log(score / 1f + 1.4f, 2));

        // Sélectionner les transformations
        howManySpriteToFind = 1;
        spawnSpriteManager.ChooseTransformation(score);
        if(howManySpriteToFind > 1) howManySpriteText.BlinkText("x" + howManySpriteToFind, 5);

        // Ajouter le sprite recherché parmi les sprites générés
        spriteRecherches = new();
        for (int i = 0; i < howManySpriteToFind; i++) // Générer un sprite de moins
        {
            spriteRecherches.Add(SummonSprite(index));
        }

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
        spriteObject.GetComponent<Image>().preserveAspect = true;
        if (spriteIndex == spriteRechercheID)
        {
            spawnSpriteManager.ApplyTransformations(spriteObject, true, spriteIndex, score);
        }
        else
        {
            spawnSpriteManager.ApplyTransformations(spriteObject, false, spriteIndex, score);
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
                    howManySpriteToFind--;
                    if (howManySpriteToFind < 1)
                    {
                        UpdateScore();
                        Instantiate(bonusChronoPrefab, Vector3.zero, Quaternion.identity, canvas.transform).Initialized((int)chronoBonus, normalizedTouchPosition);

                        StartCoroutine(Instantiate(bdaPointPrefab, Camera.main.ScreenToWorldPoint(touchPosition), Quaternion.identity).GetComponent<HyperbolicTrajectory>().MoveObject(scoreDisplay.transform.position + new Vector3(1f, 0f, 0f)));
                        chronoAnimator.AnimateBonus((int)Mathf.Ceil(chrono), (int)Mathf.Ceil(Mathf.Min(chrono + chronoBonus, chronoMax)), .65f);
                        chrono = Mathf.Min(chrono + chronoBonus, chronoMax);
                        StartCoroutine(FindTargetSprite(spriteClickable));
                        howManySpriteText.GetComponent<TextMeshProUGUI>().text = "";

                    }
                    else
                    {
                        spriteClickable.transform.SetSiblingIndex(zoneSpawnSprite.childCount);
                        Instantiate(bonusChronoPrefab, Vector3.zero, Quaternion.identity, canvas.transform).Initialized((int)Mathf.Ceil(chronoBonus/2), normalizedTouchPosition);
                        chrono = Mathf.Min(chrono + chronoBonus/2, chronoMax);
                        StartCoroutine(spriteClickable.GetComponent<HyperbolicTrajectory>().MoveObject(howManySpriteText.transform.position));
                        howManySpriteText.BlinkText("x" + howManySpriteToFind, 2);

                    }
                }
                else
                {
                    Instantiate(bonusChronoPrefab, Vector3.zero, Quaternion.identity, canvas.transform).Initialized((int)chronoMalus, normalizedTouchPosition);
                    StartCoroutine(BlinkSprite(spriteClickable, 7, 0.3f));
                    chrono = Mathf.Max(chrono + chronoMalus, 0);
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
        for(int i = 0; i < spriteRecherches.Count; i++)
        {
            spriteRecherches[i].transform.SetSiblingIndex(Mathf.CeilToInt(GenerateExponentialRandom(10f / (score + 1)) * sortedChildren.Length));
        }
    }

    private void DestroyAllSprites(Transform exception = null)
    {
        foreach (Transform child in zoneSpawnSprite)
        {
            if (child != exception)
            {
                Destroy(child.gameObject);
            }
            else
            {
                child.GetComponent<SpriteClickable>().isClicked = true;
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
        finalBlinkingScore.BlinkText("Final Score : " + score);
        finalBlinkingScore.transform.GetChild(0).gameObject.SetActive(true);
        finalBlinkingScore.transform.GetChild(1).gameObject.SetActive(true);
        Instantiate(gameOverText, new Vector3(0,-3, 0), Quaternion.identity);
        DestroyAllSprites(spriteRecherches[0].transform);
    }

    float GenerateExponentialRandom(float lambda)
    {
        float u = Random.value; // Génère un nombre aléatoire uniforme entre 0 et 1
        float result = Mathf.Exp(-u/lambda); // Appliquer ta fonction avec la nouvelle abscisse
        return result;
    }
}
