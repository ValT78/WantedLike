using System.Collections;
using UnityEngine;

public class ScaleObject : MonoBehaviour
{
    public Vector3 maxScale; // �chelle maximale
    public float scaleUpTime; // Temps pour augmenter l'�chelle
    public float scaleDownTime; // Temps pour diminuer l'�chelle

    private void Start()
    {
        StartCoroutine(ScaleUpAndDown());
    }

    private IEnumerator ScaleUpAndDown()
    {
        // Mise � l'�chelle vers le haut
        yield return StartCoroutine(ScaleOverTime(Vector3.zero, maxScale, scaleUpTime));

        // Mise � l'�chelle vers le bas
        yield return StartCoroutine(ScaleOverTime(maxScale, Vector3.zero, scaleDownTime));
    }

    private IEnumerator ScaleOverTime(Vector3 startScale, Vector3 endScale, float duration)
    {
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            t *= t; // Interpolation quadratique pour une augmentation rapide au d�but et plus lente � la fin
            transform.localScale = Vector3.Lerp(startScale, endScale, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = endScale;
    }
}
