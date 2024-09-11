using UnityEngine;

public enum Axis
{
    Default, X, Y
}

public class SineAnimation : MonoBehaviour
{
    [Header("Animation parameters")]
    [SerializeField]
    private float speed;
    [SerializeField]
    private float depth;
    [SerializeField]
    private Axis animationAxis;

    [Header("Special parameters")]
    [SerializeField, Range(-Mathf.PI, Mathf.PI)]
    private float phase = 0;
    [SerializeField]
    private bool useRealTime = false;
    [SerializeField]
    private bool activateOnAwake = false;

    [Header("Override")]
    [SerializeField]
    private bool isRectTransform;
    [SerializeField]
    private Transform overrideObjectToAnimate;

    [Header("Debug")]
    [SerializeField]
    private bool activated;

    private float m_time;
    private float m_speed;  
    private Vector2 m_basePosition;
    private Transform m_toAnimate;
    private RectTransform m_rectToAnimate;

    private void Start()
    {
        // Get the rectTransform if the option is checked (from the object itself or the override object)
        if (isRectTransform && overrideObjectToAnimate)
            m_rectToAnimate = overrideObjectToAnimate.GetComponent<RectTransform>();
        else if (isRectTransform)
            m_rectToAnimate = GetComponent<RectTransform>();

        // Check if rect was found if using rectTransform
        if(!m_rectToAnimate && isRectTransform)
        {
            Debug.LogError("No RectTransform found on this object or the object to animate although the rect transform option is checked.");
            activated = false;
            return;
        }

        // Manage Phase and activat on awake or not
        m_time = 0;
        m_speed = speed / 2 / Mathf.PI;
        activated = activateOnAwake;

        // Get the object to animate
        if (overrideObjectToAnimate != null)
            m_toAnimate = overrideObjectToAnimate;
        else
            m_toAnimate = transform;

        // Get Start Position
        if (m_rectToAnimate)
            m_basePosition = m_rectToAnimate.anchoredPosition;
        else if (overrideObjectToAnimate)
            m_basePosition = m_toAnimate.position;

    }

    private void Update()
    {
        if (!activated)
            return;

        if (useRealTime)
            m_time += Time.unscaledDeltaTime;
        else
            m_time += Time.deltaTime;

        AnimatePosition();
    }

    public void Activate(bool reset = true)
    {
        if (reset)
            m_time = 0;

        activated = true;
    }

    public void Deactivate() => activated = false;

    private void AnimatePosition()
    {
        Vector2 dir = animationAxis == Axis.X ? Vector2.right : Vector2.up;
        Vector2 desiredPosition = depth * Mathf.Sin(phase + m_time * speed) * dir;

        if (isRectTransform)
        {
            m_rectToAnimate.anchoredPosition = m_basePosition + desiredPosition;
        }
        else
        {
            m_toAnimate.position = m_basePosition + desiredPosition;
        }
    }
}

