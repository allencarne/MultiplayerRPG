using TMPro;
using UnityEngine;

public class PopUpEffect : MonoBehaviour
{
    public float floatSpeed = 0.5f;
    public float lifetime = 0.5f;
    public float fadeDuration = 0.5f;

    private TextMeshProUGUI text;
    private Color originalColor;
    private float timer;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        if (text != null)
        {
            originalColor = text.color;
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        // Float upwards
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        // Fade out
        if (text != null)
        {
            float alpha = Mathf.Lerp(originalColor.a, 0, timer / fadeDuration);
            text.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
        }

        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}
