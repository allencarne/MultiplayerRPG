using TMPro;
using UnityEngine;

public class PopUpEffect : MonoBehaviour
{
    [SerializeField] float floatSpeed = 0.5f;
    [SerializeField] float lifetime = 1f;
    [SerializeField] float fadeDuration = 0.8f;

    // Pop (spawn) settings
    [SerializeField] float startScale = 0.6f;
    [SerializeField] float overshootScale = 1.15f;
    [SerializeField] float popUpDuration = 0.14f;
    [SerializeField] float settleDuration = 0.06f;

    private TextMeshProUGUI text;
    private Color originalColor;
    private float timer;
    private float popTimer;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        if (text != null)
        {
            originalColor = text.color;
        }

        // Start scaled down for the pop effect
        transform.localScale = Vector3.one * startScale;
    }

    void Update()
    {
        float dt = Time.deltaTime;
        timer += dt;
        popTimer += dt;

        // POP scale animation: start -> overshoot -> settle to 1
        float scale = 1f;
        if (popTimer <= popUpDuration)
        {
            float t = Mathf.Clamp01(popTimer / popUpDuration);
            scale = EaseOutBack(startScale, overshootScale, t);
        }
        else if (popTimer <= popUpDuration + settleDuration)
        {
            float t = Mathf.Clamp01((popTimer - popUpDuration) / settleDuration);
            scale = Mathf.Lerp(overshootScale, 1f, t);
        }
        else
        {
            scale = 1f;
        }

        transform.localScale = Vector3.one * scale;

        // Float upwards
        transform.position += Vector3.up * floatSpeed * dt;

        // Fade out
        if (text != null)
        {
            float alpha = Mathf.Lerp(originalColor.a, 0f, timer / fadeDuration);
            alpha = Mathf.Clamp01(alpha);
            text.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
        }

        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    // EaseOutBack between two values (gives a nice pop/overshoot feel)
    static float EaseOutBack(float a, float b, float t)
    {
        // standard easeOutBack
        float s = 1.70158f;
        t = Mathf.Clamp01(t);
        t = t - 1f;
        float eased = 1f + (t * t * ((s + 1f) * t + s));
        return Mathf.Lerp(a, b, eased);
    }
}
