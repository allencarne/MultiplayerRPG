using UnityEngine;
using UnityEngine.UI;

public class FlashUI : MonoBehaviour
{
    float flashSpeed = 2f;
    float minAlpha = 0.2f;
    float maxAlpha = 1f;

    private Button button;
    private Image targetImage;
    private float alphaDirection = 1f;
    private float currentAlpha = 1f;

    private void Awake()
    {
        button = GetComponent<Button>();
        targetImage = GetComponent<Image>();

        if (targetImage != null)
            currentAlpha = targetImage.color.a;
    }

    private void Update()
    {
        if (button == null || targetImage == null) return;

        // Check if the button's current color is yellow
        if (IsColorApproximately(button.colors.normalColor, Color.yellow))
        {
            // Flash alpha
            currentAlpha += flashSpeed * alphaDirection * Time.deltaTime;

            if (currentAlpha >= maxAlpha)
            {
                currentAlpha = maxAlpha;
                alphaDirection = -1f;
            }
            else if (currentAlpha <= minAlpha)
            {
                currentAlpha = minAlpha;
                alphaDirection = 1f;
            }

            Color color = targetImage.color;
            color.a = currentAlpha;
            targetImage.color = color;
        }
        else
        {
            // Reset to fully visible if not yellow
            Color color = targetImage.color;
            color.a = 1f;
            targetImage.color = color;
        }
    }

    private bool IsColorApproximately(Color a, Color b, float tolerance = 0.1f)
    {
        return Mathf.Abs(a.r - b.r) < tolerance &&
               Mathf.Abs(a.g - b.g) < tolerance &&
               Mathf.Abs(a.b - b.b) < tolerance;
    }
}
