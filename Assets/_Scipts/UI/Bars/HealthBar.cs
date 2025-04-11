using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Image healthBar;
    [SerializeField] Image healthBar_Back;

    private float lerpSpeed = 5f;
    private Coroutine lerpCoroutine;

    public void UpdateHealthBar(float maxHealth, float currentHealth)
    {
        if (maxHealth <= 0) return;

        healthBar.fillAmount = currentHealth / maxHealth;

        if (lerpCoroutine != null)
        {
            StopCoroutine(lerpCoroutine);
        }
        lerpCoroutine = StartCoroutine(LerpHealthBarBack(currentHealth / maxHealth));
    }

    IEnumerator LerpHealthBarBack(float targetFillAmount)
    {
        float currentFillAmount = healthBar_Back.fillAmount;

        while (!Mathf.Approximately(currentFillAmount, targetFillAmount))
        {
            currentFillAmount = Mathf.Lerp(currentFillAmount, targetFillAmount, lerpSpeed * Time.deltaTime);
            healthBar_Back.fillAmount = currentFillAmount;
            yield return null;
        }
    }
}
