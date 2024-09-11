using UnityEngine;

public class NaturalMovement : MonoBehaviour
{
    [SerializeField]
    private RectTransform elementTransform;

    [SerializeField]
    private Vector3 baseMovementAmount = new(10, 10, 0);

    [Header("Speed and Frequencies")]
    [Range(0.1f, 5f), SerializeField]
    private float movementSpeed = 2f;
    [Range(0.1f, 5f), SerializeField]
    private float baseFrequencyX = 1f;
    [Range(0.1f, 5f), SerializeField]
    private float baseFrequencyY = 1f;
    [Range(0.1f, 5f), SerializeField]
    private float baseFrequencyZ = 1f;

    [Header("Randomness")]
    [Range(0f, 1f), SerializeField]
    private float randomRange = 0.2f;

    [Header("Damping")]
    [SerializeField]
    private bool noDamping = false;
    [Range(0.8f, 1f), SerializeField]
    private float baseDamping = 0.95f;

    private Vector3 initialPosition;
    private Vector3 randomMovementAmount;
    private float randomFrequencyX, randomFrequencyY, randomFrequencyZ;
    private float randomDamping;
    private Vector3 randomTimeOffset;
    private float time;

    void Start()
    {
        // Store the initial position of the element
        initialPosition = elementTransform.localPosition;

        // Random variations in movement amount
        randomMovementAmount = baseMovementAmount + new Vector3(
            Random.Range(-randomRange, randomRange),
            Random.Range(-randomRange, randomRange),
            Random.Range(-randomRange, randomRange));

        // Randomize frequencies for each axis
        randomFrequencyX = baseFrequencyX + Random.Range(-randomRange, randomRange);
        randomFrequencyY = baseFrequencyY + Random.Range(-randomRange, randomRange);
        randomFrequencyZ = baseFrequencyZ + Random.Range(-randomRange, randomRange);

        // Slight variations in damping
        if (noDamping)
            randomDamping = 1;
        else
            randomDamping = baseDamping + Random.Range(-randomRange, randomRange / 5f);

        // Random time offset for each axis
        randomTimeOffset = new Vector3(
            Random.Range(0f, Mathf.PI * 2f),
            Random.Range(0f, Mathf.PI * 2f),
            Random.Range(0f, Mathf.PI * 2f));
    }

    void Update()
    {
        time += Time.deltaTime * movementSpeed;
        Vector3 offset = Mathf.Exp(-time * (1 - randomDamping)) * Vector3.one;

        offset.x *= randomMovementAmount.x * Mathf.Sin((time * randomFrequencyX) + randomTimeOffset.x);
        offset.y *= randomMovementAmount.y * Mathf.Sin((time * randomFrequencyY) + randomTimeOffset.y);
        offset.z *= randomMovementAmount.z * Mathf.Sin((time * randomFrequencyZ) + randomTimeOffset.z);

        elementTransform.localPosition = initialPosition + offset;

        if (time > 1000f) time = 0f;
    }
}
