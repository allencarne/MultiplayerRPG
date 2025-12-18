using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : NetworkBehaviour
{
    [SerializeField] CharacterStats stats;
    [SerializeField] Image healthBar;
    [SerializeField] Image healthBar_Back;

    float lerpSpeed = 5f;
    Coroutine lerpCoroutine;

    public override void OnNetworkSpawn()
    {
        stats.net_CurrentHealth.OnValueChanged += OnHealthChanged;
        stats.net_TotalHealth.OnValueChanged += OnHealthChanged;
        UpdateHealthBar();
    }

    public override void OnNetworkDespawn()
    {
        stats.net_CurrentHealth.OnValueChanged -= OnHealthChanged;
        stats.net_TotalHealth.OnValueChanged -= OnHealthChanged;
    }

    void OnHealthChanged(float oldValue, float newValue)
    {
        UpdateHealthBar();
    }

    public void UpdateHealthBar()
    {
        float maxHealth = stats.net_TotalHealth.Value;
        float currentHealth = stats.net_CurrentHealth.Value;

        if (maxHealth <= 0) return;

        float fill = currentHealth / maxHealth;
        healthBar.fillAmount = fill;

        if (!gameObject.activeInHierarchy) return;

        if (lerpCoroutine != null) StopCoroutine(lerpCoroutine);
        lerpCoroutine = StartCoroutine(LerpHealthBarBack(fill));
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
