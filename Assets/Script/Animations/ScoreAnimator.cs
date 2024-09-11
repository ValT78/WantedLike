using System.Collections;
using TMPro;
using UnityEngine;

public class ScoreAnimator : MonoBehaviour
{
    private const float MARGIN = 0.001f;

    [Header("Text Blink Animation")]
    [SerializeField]
    private TextMeshProUGUI textScore;
    [SerializeField, Tooltip("Intervalle de clignotement en secondes")]
    private float blinkInterval;

    [Header("Logo Pulse Animation")]
    [SerializeField]
    private Transform logoTransform;
    [SerializeField, Tooltip("Growth intensity in scale unit")]
    private float growthOnPulse = 0.2f;
    [SerializeField, Tooltip("Duration of the pulse in seconds")]
    private float pulseDuration = 0.7f;

    [Header("Optimization")]
    [SerializeField, Tooltip("Use cached deltaTime to optimize the pulse animation (but might not be as accurate)")]
    private bool useCachedDeltaTime = false;

    private float originalScale;

    private void Start()
    {
        originalScale = transform.localScale.x;
    }

    public void BlinkText(string originalText, int blinkCount = int.MaxValue)
    {
        StartCoroutine(BlinkTextCoroutine(originalText, blinkCount));
    }

    private IEnumerator BlinkTextCoroutine(string originalText, int blinkCount)
    {
        while (blinkCount > 0)
        {
            blinkCount--;
            if(textScore.text == "")
            {
                textScore.text = originalText;
            }
            else
            {
                textScore.text = "";
            }
            yield return new WaitForSeconds(blinkInterval);
        }
        textScore.text = originalText;
    }

    public void PulseLogoGrow()
    {
        StartCoroutine(PulseLogoGrowCoroutine());
    }

    private IEnumerator PulseLogoGrowCoroutine()
    {
        float tau = Mathf.Log(MARGIN / growthOnPulse) / pulseDuration;
        float interpolationFactor = 1 - Mathf.Exp(Time.deltaTime * tau);
        logoTransform.localScale = new Vector3(originalScale + growthOnPulse, originalScale + growthOnPulse, originalScale + growthOnPulse);

        while(Mathf.Abs(logoTransform.localScale.x - originalScale) > MARGIN)
        {
            if (!useCachedDeltaTime)
            {
                // Recalculate interpolation factor because Time.deltaTime can change
                interpolationFactor = 1 - Mathf.Exp(Time.deltaTime * tau);
            }

            float newScale = Mathf.Lerp(logoTransform.localScale.x, originalScale, interpolationFactor);
            logoTransform.localScale = Vector3.one * newScale;
            yield return null;
        }

        yield return null;
    }
}