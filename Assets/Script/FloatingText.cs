using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private float lifetime; // Durée de vie du texte
    [SerializeField] private float floatSpeed; // Vitesse de montée du texte
    [SerializeField] private bool isBonus;

    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] private RectTransform rectTransform; // Durée de vie du texte

    void Start()
    {
        Destroy(gameObject, lifetime);
        

    }

    void Update()
    {
        // Monter le texte vers le haut
        rectTransform.anchoredPosition += new Vector2(0, floatSpeed * Time.deltaTime);

        //Réduire l'opacité du texte
        textMesh.alpha -= Time.deltaTime / lifetime;
    }

    public void Initialized(bool isBonus, Vector2 position)
    {
        this.isBonus = isBonus;
        rectTransform.anchoredPosition = new Vector2((position.x)* 1080 - 540, (position.y)*1920 - 860); // Centrer le texte + décaler vers le haut
        if (isBonus)
        {
            textMesh.text = "+" + GameManager.Instance.chronoBonus.ToString();
            textMesh.color = Color.red;
        }
        else
        {
            textMesh.text = "-" + GameManager.Instance.chronoMalus.ToString();
            textMesh.color = Color.cyan;
        }
    }
}
