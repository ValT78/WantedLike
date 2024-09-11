using System.Collections;
using UnityEngine;

public class ButtonAnimation : MonoBehaviour
{
    [Header("Time")]
    [SerializeField]
    private bool bypassTimescale;
    [SerializeField]
    private bool enableOnAwake = true;
    [SerializeField]
    private bool animateOnAwake = true;

    [Header("Wave Rotation")]
    [SerializeField]
    private bool enableRotation;
    [SerializeField]
    private float rotationDepth, rotationSpeed;

    [Header("Grow on hover")]
    [SerializeField]
    private bool enableHoverScale;
    [SerializeField]
    private float hoverDepth, hoverSpeed;

    private Vector3 scaleOnHover;

    [Header("Click animation")]
    [SerializeField]
    private bool enableClickAnimation;
    [SerializeField]
    private float clickDepth, clickSpeed;

    private Vector3 scaleOnClick;


    [Header("Sound")]
    [SerializeField]
    private bool playSoundOnHover;
    [SerializeField]
    private AudioClip soundOnHover;
    [SerializeField]
    private bool playSoundOnClick;
    [SerializeField]
    private AudioClip soundOnClick;
    [Space]
    [SerializeField]
    private bool onUpClickInstead;
    [SerializeField]
    private bool useFunctionInstead;

    private float _time;
    private bool isMouseOver;
    private bool isMouseOverFirstFrame;
    private bool isButtonEnabled;
    private bool isAnimating;
    private Vector3 scaleReference;

#if UNITY_EDITOR

    private void OnValidate()
    {
        scaleOnHover = Vector3.one * (1 + hoverDepth);
        scaleOnClick = Vector3.one * (1 - clickDepth);
    }

#endif


    private void OnEnable()
    {
        _time = 0;

        if (bypassTimescale && !isAnimating)
        {
            isAnimating = true;
            StartCoroutine(BypassTimescale());
        }
    }

    private void OnDisable()
    {
        isAnimating = false;
    }

    private void Start()
    {
        isButtonEnabled = enableOnAwake;
        isAnimating = animateOnAwake;

        scaleReference = transform.localScale;
        scaleOnHover = scaleReference * (1 + hoverDepth);
        scaleOnClick = scaleReference * (1 - clickDepth);
        _time = 0;

        isMouseOver = false;
        isMouseOverFirstFrame = false;
    }


    private IEnumerator BypassTimescale()
    {
        while (isAnimating)
        {
            if (enableRotation)
                HandleWaveMotion();

            isMouseOverFirstFrame &= IsMouseOver();


            if (enableHoverScale)
                HandleGrowthOnHover(Time.unscaledDeltaTime);

            //if (playSoundOnClick && isMouseOver && Input.GetMouseButtonUp(0) && !useFunctionInstead && (Input.GetMouseButtonUp(0) && onUpClickInstead || Input.GetMouseButtonDown(0) && !onUpClickInstead))
            //    PlayButtonSound();
            if (playSoundOnHover)
            {
                if (isMouseOver && !isMouseOverFirstFrame)
                {
                    //SoundManager.Instance.PlaySound(soundOnHover);
                    isMouseOverFirstFrame = true;
                }
            }
            _time += Time.unscaledDeltaTime;

            yield return null;
        }
    }

    //public void PlayButtonSound() => SoundManager.Instance.PlaySound(soundOnClick);

    private void Update()
    {
        if (bypassTimescale && isAnimating)
            return;

        if (enableRotation)
            HandleWaveMotion();

        isMouseOverFirstFrame &= IsMouseOver();

        if (enableHoverScale)
            HandleGrowthOnHover(Time.deltaTime);

        //if (playSoundOnClick && isMouseOver && !useFunctionInstead && (Input.GetMouseButtonUp(0) && onUpClickInstead || Input.GetMouseButtonDown(0) && !onUpClickInstead))
        //    PlayButtonSound();

        if (playSoundOnHover)
        {
            if (isMouseOver && !isMouseOverFirstFrame)
            {
                //SoundManager.Instance.PlaySound(soundOnHover);
                isMouseOverFirstFrame = true;
            }
        }
        _time += Time.deltaTime;
    }

    private void HandleGrowthOnHover(float dTime)
    {
        if (isMouseOver)
        {
            if (enableClickAnimation && Input.GetMouseButton(0))
                transform.localScale = Vector3.Lerp(transform.localScale, scaleOnClick, dTime * clickSpeed);
            else
                transform.localScale = Vector3.Lerp(transform.localScale, scaleOnHover, dTime * hoverSpeed);
        }
        else
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, dTime * hoverSpeed);
        }
    }

    private void HandleWaveMotion()
    {
        float rotationAngle = Mathf.Sin(_time * rotationSpeed) * rotationDepth;
        transform.rotation = Quaternion.Euler(0f, 0f, rotationAngle);

    }


    private bool IsMouseOver()
    {
        if (!isButtonEnabled)
        {
            isMouseOver = false;
            return false;
        }

        // Check if the mouse is over the button
        RectTransform rectTransform = GetComponent<RectTransform>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, null, out Vector2 localPoint);

        isMouseOver = rectTransform.rect.Contains(localPoint);

        return isMouseOver;
    }

}
