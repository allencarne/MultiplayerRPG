using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class StatusEffects : MonoBehaviour
{
    [SerializeField] private Image imageFill;

    public void UpdateFill(float fillAmount)
    {
        imageFill.fillAmount = fillAmount;
    }

    public void UpdateUI(float remainingTime, float maxDuration)
    {
        if (maxDuration <= 0f)
            maxDuration = 1f; // prevent division by zero

        float fillAmount = 1f - Mathf.Clamp01(remainingTime / maxDuration);
        imageFill.fillAmount = fillAmount;
    }
}
