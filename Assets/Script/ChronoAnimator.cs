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

    [Header("References")]
    [SerializeField]
    private TextMeshProUGUI chronoText;

    private float referenceScale;

    private void Start()
    {
        referenceScale = transform.localScale.x;
    }

    public void Animate(int from, int to, float duration)
    {
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
            yield return null;
        }
        transform.localScale = Vector3.one * referenceScale;
    }
}
