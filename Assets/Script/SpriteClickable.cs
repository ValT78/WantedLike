using UnityEngine;
using UnityEngine.EventSystems;

public class SpriteClickable : MonoBehaviour
{
    [HideInInspector] public int spriteID;

    [Header("Mouvement")]
    public Vector2 direction;
    public float speed;
    public bool bounceOnEdges;

    [SerializeField] private RectTransform rectTransform;
    private RectTransform zoneSpawnSprite;
    [HideInInspector] public bool isClicked;

    void Start()
    {
        zoneSpawnSprite = GameManager.Instance.zoneSpawnSprite;
    }

    void Update()
    {
        MoveSprite();
    }

    public void Initialized(int spriteID, Vector2 direction, float speed, bool bounceOnEdges)
    {
        this.spriteID = spriteID;
        this.direction = direction;
        this.speed = speed;
        this.bounceOnEdges = bounceOnEdges;
    }

    void MoveSprite()
    {
        Vector3 newPosition = rectTransform.localPosition + speed * Time.deltaTime * (Vector3)direction;

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
