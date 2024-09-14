using System.Collections;
using TMPro;
using UnityEngine;

public class ChronoAnimator : MonoBehaviour
{
    [Header("Animations")]
    [SerializeField]
    private float maxGrowth = 2f;
    [SerializeField]
    private float growthSpeed = 5f;
    [SerializeField]
    private float delayBeforeAdding = 0.2f;
    [SerializeField]
    private float delayAfterAdding = 0.2f;
    [SerializeField]
    private float positionShift = 2f;

    [Header("References")]
    [SerializeField]
    private TextMeshProUGUI chronoText;
    [SerializeField]
    private PulseAnimation pulseAnimation;

    private float referenceScale;
    private float referencePosition;

    private void Start()
    {
        referenceScale = transform.localScale.x;
        referencePosition = transform.position.x;
    }

    // Tick is true whenever the integer part of the time changes
    public void UpdateTime(float time, bool tick)
    {
        chronoText.text = Mathf.Ceil(time).ToString();
        if (tick && time > 0)
        {
            pulseAnimation.Pulse(20 / Mathf.Pow(time, 1.2f));
            SoundManager.Instance.PlayChrono(SoundManager.Instance.tikChrono, Mathf.Lerp(0.1f, 1f, 1-time/60));
        }
    }

    public void AnimateBonus(int from, int to, float duration)
    {
        pulseAnimation.StopAllCoroutines();
        StartCoroutine(GrowAndAdd(duration, from, to));
    }

    private IEnumerator GrowAndAdd(float duration, int from, int to)
    {
        int steps = to - from;

        duration -= delayBeforeAdding + delayAfterAdding;
        float currentScale = referenceScale;

        // Grow
        while (currentScale < maxGrowth)
        {
            currentScale += growthSpeed * Time.deltaTime;
            transform.localScale = new Vector3(currentScale, currentScale, currentScale);
            transform.position = new Vector3(referencePosition + positionShift * (currentScale - referenceScale), transform.position.y, transform.position.z);
            yield return null;
        }

        yield return new WaitForSeconds(delayBeforeAdding);

        // Add
        for (int i = from+1; i <= to; i++)
        {
            chronoText.text = i.ToString();
            yield return new WaitForSeconds(duration/steps);
        }

        yield return new WaitForSeconds(delayAfterAdding);

        // Shrink
        while (currentScale > referenceScale)
        {
            currentScale -= growthSpeed * Time.deltaTime;
            transform.localScale = new Vector3(currentScale, currentScale, currentScale);
            transform.position = new Vector3(referencePosition + positionShift * (currentScale - referenceScale), transform.position.y, transform.position.z);

            yield return null;
        }
        transform.localScale = Vector3.one * referenceScale;
        transform.position = new Vector3(referencePosition, transform.position.y, transform.position.z);
    }
}
