using UnityEngine;

public class HyperbolicTrajectory : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float hyperbolicFactor;

    public System.Collections.IEnumerator MoveObject(Vector3 targetPosition)
    {
        Vector3 startPosition = transform.position;
        float journeyLength = Vector3.Distance(startPosition, targetPosition);
        float startTime = Time.time;
        bool curveRight = Random.value > 0.5f;

        while (this!= null && Vector3.Distance(transform.position, targetPosition) > 0.2f)
        {
            float distCovered = (Time.time - startTime) * speed;
            float fractionOfJourney = distCovered / journeyLength;

            Vector3 direction = (targetPosition - startPosition).normalized;
            Vector3 curve = curveRight ? Vector3.Cross(direction, Vector3.forward) : Vector3.Cross(direction, Vector3.back);
            float curveAmount = Mathf.Sin(fractionOfJourney * Mathf.PI) * hyperbolicFactor; // Augmenter ce facteur pour plus de courbure

            // Calculer la nouvelle position avec courbure
            Vector3 newPosition = Vector3.Lerp(startPosition, targetPosition, fractionOfJourney) + curve * curveAmount;

            transform.position = newPosition;
            transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);

            yield return null;
        }

        if(this != null) Destroy(gameObject);
    }


}
