using UnityEngine;
using UnityEngine.UI;

public class StatusEffects : MonoBehaviour
{
    [SerializeField] private Image imageFill;

    private float maxDuration;
    private float elapsedTime;
    private bool isStackable = false;

    #region Non-Stackable

    public void Initialize(float duration)
    {
        maxDuration = duration;
        isStackable = false;
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
        if (!isStackable)
            maxDuration = newMax;
    }

    #endregion

    #region Stackable

    public void UpdateStackUI(float buffDuration)
    {
        maxDuration = buffDuration;
        elapsedTime = 0f;
        isStackable = true;
    }

    private void Update()
    {
        if (!isStackable)
            return;

        elapsedTime += Time.deltaTime;

        float fillAmount = Mathf.Clamp01(elapsedTime / maxDuration);
        imageFill.fillAmount = fillAmount;
    }

    #endregion
}
