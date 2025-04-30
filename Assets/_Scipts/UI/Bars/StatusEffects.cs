using UnityEngine;
using UnityEngine.UI;

public class StatusEffects : MonoBehaviour
{
    [SerializeField] private Image imageFill;

    private float maxDuration;

    public void Initialize(float duration)
    {
        maxDuration = duration;
        UpdateUI(duration);
    }

    public void UpdateUI(float remainingTime)
    {
        if (maxDuration <= 0f)
            maxDuration = 1f; // prevent division by zero

        float fillAmount = 1f - Mathf.Clamp01(remainingTime / maxDuration);
        imageFill.fillAmount = fillAmount;
    }

    public void SetMaxDuration(float newMax)
    {
        maxDuration = newMax;
    }
}
