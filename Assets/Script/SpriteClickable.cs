using UnityEngine;
using UnityEngine.EventSystems;

public class SpriteClickable : MonoBehaviour
{
    [HideInInspector] public int spriteID;

    [Header("Mouvement")]
    public Vector2 direction;
    public float speed;
    public bool bounceOnEdges;
    public bool isSinusoidal;
    public float frequency; // Fréquence de l'oscillation
    public float amplitude; // Amplitude de l'oscillation
    private float time; // Temps écoulé
    private float rotationSpeed; // Vitesse de rotation

    [SerializeField] private RectTransform rectTransform;
    private RectTransform zoneSpawnSprite;
    [HideInInspector] public bool isClicked;

    void Start()
    {
        zoneSpawnSprite = GameManager.Instance.zoneSpawnSprite;
    }

    void FixedUpdate()
    {
        MoveSprite();
    }

    public void Initialized(int spriteID, Vector2 direction, float speed, bool bounceOnEdges, bool isSinusoidal, float amplitude, float frequency, float rotationSpeed)
    {
        this.spriteID = spriteID;
        this.direction = direction;
        this.speed = speed;
        this.bounceOnEdges = bounceOnEdges;
        this.isSinusoidal = isSinusoidal;
        this.amplitude = amplitude;
        this.frequency = frequency;
        this.rotationSpeed = rotationSpeed;
    }

    
    void MoveSprite()
    {
        time += Time.deltaTime;

        // Calcul de la nouvelle position avec oscillation sinusoïdale
        Vector3 newPosition = rectTransform.localPosition + speed * Time.deltaTime * (Vector3)direction;

        //Tourner le sprite autour de lui même à une certaine vitesse
        rectTransform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

        if (isSinusoidal)
        {
            // Calcul du vecteur perpendiculaire à la direction
            Vector2 perpendicularDirection = new Vector2(-direction.y, direction.x).normalized;

            // Appliquer l'oscillation sinusoïdale le long du vecteur perpendiculaire
            newPosition += (Vector3)(amplitude * Mathf.Sin(time * frequency) * perpendicularDirection);
        }

        if (bounceOnEdges)
        {
            // Gestion des rebonds
            if (newPosition.x < zoneSpawnSprite.rect.xMin || newPosition.x > zoneSpawnSprite.rect.xMax)
            {
                direction.x = -direction.x;
            }
            if (newPosition.y < zoneSpawnSprite.rect.yMin || newPosition.y > zoneSpawnSprite.rect.yMax)
            {
                direction.y = -direction.y;
            }
        }
        else
        {
            // Gestion du comportement en tore
            if (newPosition.x < zoneSpawnSprite.rect.xMin)
            {
                newPosition.x = zoneSpawnSprite.rect.xMax;
            }
            else if (newPosition.x > zoneSpawnSprite.rect.xMax)
            {
                newPosition.x = zoneSpawnSprite.rect.xMin;
            }

            if (newPosition.y < zoneSpawnSprite.rect.yMin)
            {
                newPosition.y = zoneSpawnSprite.rect.yMax;
            }
            else if (newPosition.y > zoneSpawnSprite.rect.yMax)
            {
                newPosition.y = zoneSpawnSprite.rect.yMin;
            }
        }

        rectTransform.localPosition = newPosition;
    }

}
