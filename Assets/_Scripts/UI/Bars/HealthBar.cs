using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : NetworkBehaviour
{
    [SerializeField] CharacterStats stats;
    [SerializeField] Image healthBar;
    [SerializeField] Image healthBar_Back;

    private float lerpSpeed = 5f;
    private Coroutine lerpCoroutine;

    public override void OnNetworkSpawn()
    {
        stats.Health.OnValueChanged += OnHealthChanged;

        UpdateHealthBar();
    }

    public override void OnNetworkDespawn()
    {
        stats.Health.OnValueChanged -= OnHealthChanged;
    }

    public void UpdateHealthBar()
    {
        float maxHealth = stats.ModifiedMaxHealth;
        float currentHealth = stats.Health.Value;

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

    void OnHealthChanged(float oldValue, float newValue)
    {
        UpdateHealthBar();
    }
}
