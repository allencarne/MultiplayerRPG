using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : NetworkBehaviour
{
    [SerializeField] Player player;
    [SerializeField] Image healthBar;
    [SerializeField] Image healthBar_Back;

    private float lerpSpeed = 5f;
    private Coroutine lerpCoroutine;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            player.TakeDamage(1, DamageType.Flat, NetworkObject);
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            player.GiveHeal(1, HealType.Flat);
        }
    }

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
