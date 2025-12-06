using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : NetworkBehaviour
{
    [SerializeField] PlayerStats stats;
    [SerializeField] Image healthBar;
    [SerializeField] Image healthBar_Back;

    private float lerpSpeed = 5f;
    private Coroutine lerpCoroutine;

    public override void OnNetworkSpawn()
    {
        stats.Health.OnValueChanged += OnHealthChanged;
        stats.MaxHealth.OnValueChanged += OnMaxHealthChanged;

        UpdateHealthBar(stats.MaxHealth.Value, stats.Health.Value);
    }

    private void OnDisable()
    {
        stats.Health.OnValueChanged -= OnHealthChanged;
        stats.MaxHealth.OnValueChanged -= OnMaxHealthChanged;
    }

    public void UpdateHealthBar(float maxHealth, float currentHealth)
    {
        if (maxHealth <= 0) return;

        healthBar.fillAmount = currentHealth / maxHealth;

        if (!gameObject.activeInHierarchy)
            return;

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

    void OnHealthChanged(float oldValue, float newValue)
    {
        UpdateHealthBar(stats.MaxHealth.Value, newValue);
    }

    void OnMaxHealthChanged(float oldValue, float newValue)
    {
        UpdateHealthBar(newValue, stats.Health.Value);
    }
}
