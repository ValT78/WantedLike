using System.Collections;
using UnityEngine;

public class ScaleObject : MonoBehaviour
{
    public Vector3 maxScale; // Échelle maximale
    public float scaleUpTime; // Temps pour augmenter l'échelle
    public float scaleDownTime; // Temps pour diminuer l'échelle

    private void Start()
    {
        StartCoroutine(ScaleUpAndDown());
    }

    private IEnumerator ScaleUpAndDown()
    {
        // Mise à l'échelle vers le haut
        yield return StartCoroutine(ScaleOverTime(Vector3.zero, maxScale, scaleUpTime));

        // Mise à l'échelle vers le bas
        yield return StartCoroutine(ScaleOverTime(maxScale, Vector3.zero, scaleDownTime));
    }

    private IEnumerator ScaleOverTime(Vector3 startScale, Vector3 endScale, float duration)
    {
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            t *= t; // Interpolation quadratique pour une augmentation rapide au début et plus lente à la fin
            transform.localScale = Vector3.Lerp(startScale, endScale, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = endScale;
    }
}
