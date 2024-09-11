using System.Collections;
using TMPro;
using UnityEngine;

public class BlinkingText : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;
    public float blinkInterval; // Intervalle de clignotement en secondes

    private string originalText;

    public IEnumerator BlinkText(string originalText, int blinkCount = int.MaxValue)
    {
        this.originalText = originalText;
        while (blinkCount > 0)
        {
            blinkCount--;
            if(textMeshPro.text == "")
            {
                textMeshPro.text = this.originalText;
            }
            else
            {
                textMeshPro.text = "";
            }
            yield return new WaitForSeconds(blinkInterval);
        }
    }
}