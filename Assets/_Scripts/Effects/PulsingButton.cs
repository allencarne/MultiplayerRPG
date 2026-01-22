using UnityEngine;
using UnityEngine.UI;

public class PulsingButton : MonoBehaviour
{
    [Header("Scale Pulse Settings")]
    [SerializeField] bool enableScalePulse = true;
    float scaleMin = 0.9f;
    float scaleMax = 1.1f;
    float scalePulseSpeed = 2f;

    [Header("Alpha Pulse Settings")]
    [SerializeField] bool enableAlphaPulse = true;
    float alphaMin = 0.6f;
    float alphaMax = 1f;
    float alphaPulseSpeed = 2f;

    private Vector3 originalScale;
    private CanvasGroup canvasGroup;
    private Image[] images;
    private float scaleTime;
    private float alphaTime;

    private void Awake()
    {
        originalScale = transform.localScale;

        // Get or add CanvasGroup for alpha pulsing
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null && enableAlphaPulse)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // Get all images for potential alpha fallback
        images = GetComponentsInChildren<Image>();
    }

    private void OnEnable()
    {
        // Reset timing when enabled
        scaleTime = 0f;
        alphaTime = 0f;
        transform.localScale = originalScale;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
        }
    }

    private void OnDisable()
    {
        // Reset to original state when disabled
        transform.localScale = originalScale;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
        }
    }

    private void Update()
    {
        if (enableScalePulse)
        {
            PulseScale();
        }

        if (enableAlphaPulse)
        {
            PulseAlpha();
        }
    }

    private void PulseScale()
    {
        scaleTime += Time.deltaTime * scalePulseSpeed;
        float scale = Mathf.Lerp(scaleMin, scaleMax, (Mathf.Sin(scaleTime) + 1f) / 2f);
        transform.localScale = originalScale * scale;
    }

    private void PulseAlpha()
    {
        alphaTime += Time.deltaTime * alphaPulseSpeed;
        float alpha = Mathf.Lerp(alphaMin, alphaMax, (Mathf.Sin(alphaTime) + 1f) / 2f);

        if (canvasGroup != null)
        {
            canvasGroup.alpha = alpha;
        }
    }
}
