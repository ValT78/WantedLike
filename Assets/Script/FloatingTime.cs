using UnityEngine;
using TMPro;

public class FloatingTime : MonoBehaviour
{
    [Header("Animations Settings")]
    [SerializeField] private float lifetime = 2f; // Durée de vie du texte
    [SerializeField] private float startFloatingSpeed = 300f; // Vitesse de montée du texte
    [SerializeField] private float acceleration = -150f; // Accélération de la montée du texte
    [SerializeField, Range(0f, 1f)] private float fadeDelay = 1f; // Delay avant de commencer à réduire l'opacité

    [Header("References")]
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform groupTransform;

    private float m_fadeDelay;

    void Start()
    {
        m_fadeDelay = fadeDelay * lifetime;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Monter le texte vers le haut
        groupTransform.anchoredPosition += new Vector2(0, startFloatingSpeed * Time.deltaTime);

        // Accélérer la montée du texte
        startFloatingSpeed += acceleration * Time.deltaTime;

        // Réduire l'opacité du groupe
        if (m_fadeDelay > 0)
            m_fadeDelay -= Time.deltaTime;
        else if (canvasGroup.alpha > 0)
            canvasGroup.alpha -= Time.deltaTime / (lifetime * fadeDelay);
    }

    public void Initialized(int bonus, Vector2 position)
    {
        // Centrer le texte + décaler vers le haut (adapter du world space au screen space)
        groupTransform.anchoredPosition = new Vector2((position.x)* 1080 - 540, (position.y)*1920 - 860);

        if (bonus>=0)
        {
            text.text = "+" + bonus.ToString();
            text.color = Color.red;
        }
        else
        {
            text.text = bonus.ToString();
            text.color = Color.cyan;
        }
    }
}
