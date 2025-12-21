using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StatusEffects : MonoBehaviour
{
    [SerializeField] private Image imageFill;
    Coroutine currentDuration;

    public void StartUI(float time)
    {
        if (currentDuration != null) StopCoroutine(currentDuration);
        currentDuration = StartCoroutine(HandleUI(time));
    }

    IEnumerator HandleUI(float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float fill = Mathf.Clamp01(elapsed / duration);
            imageFill.fillAmount = fill;

            yield return null;
        }

        imageFill.fillAmount = 0;
    }





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
