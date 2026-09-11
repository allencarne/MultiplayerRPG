using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ManaBar : NetworkBehaviour
{
    [Header("Stats")]
    [SerializeField] PlayerStats stats;

    [Header("UI")]
    [SerializeField] Image manaBar;
    [SerializeField] Image manaBar_Back;

    [Header("Variables")]
    bool isRecharging = false;
    float lerpSpeed = 5f;
    Coroutine lerpCoroutine;

    public override void OnNetworkSpawn()
    {
        stats.Mana.OnValueChanged += OnManaChanged;
        stats.MaxMana.OnValueChanged += OnMaxManaChanged;

        UpdateManaBar(stats.MaxMana.Value, stats.Mana.Value);
    }

    public override void OnNetworkDespawn()
    {
        stats.Mana.OnValueChanged -= OnManaChanged;
        stats.MaxMana.OnValueChanged -= OnMaxManaChanged;
    }

    public void SpendMana(float amount)
    {
        if (IsServer)
        {
            if (stats.Mana.Value >= amount)
            {
                stats.Mana.Value -= amount;

                if (!isRecharging)
                {
                    StartCoroutine(RechargeMana());
                }
            }
        }
        else
        {
            SpendManaServerRpc(amount);
        }
    }

    [ServerRpc]
    void SpendManaServerRpc(float amount)
    {
        if (stats.Mana.Value >= amount)
        {
            stats.Mana.Value -= amount;

            if (!isRecharging)
            {
                StartCoroutine(RechargeMana());
            }
        }
    }

    IEnumerator RechargeMana()
    {
        isRecharging = true;

        while (stats.Mana.Value < stats.MaxMana.Value)
        {
            yield return new WaitForSeconds(1);

            stats.Mana.Value += stats.ManaRechargeRate.Value;
            stats.Mana.Value = Mathf.Min(stats.Mana.Value, stats.MaxMana.Value);
        }

        isRecharging = false;
    }

    void UpdateManaBar(float maxMana, float currentMana)
    {
        if (maxMana <= 0) return;
        manaBar.fillAmount = currentMana / maxMana;

        if (lerpCoroutine != null)
        {
            StopCoroutine(lerpCoroutine);
        }

        lerpCoroutine = StartCoroutine(LerpManaBar(currentMana / maxMana));
    }

    IEnumerator LerpManaBar(float targetFillAmount)
    {
        float currentFillAmount = manaBar_Back.fillAmount;

        while (!Mathf.Approximately(currentFillAmount, targetFillAmount))
        {
            currentFillAmount = Mathf.Lerp(currentFillAmount, targetFillAmount, lerpSpeed * Time.deltaTime);
            manaBar_Back.fillAmount = currentFillAmount;
            yield return null;
        }
    }

    void OnManaChanged(float oldValue, float newValue)
    {
        UpdateManaBar(stats.MaxMana.Value, newValue);
    }

    void OnMaxManaChanged(float oldValue, float newValue)
    {
        UpdateManaBar(newValue, stats.Mana.Value);
    }
}
