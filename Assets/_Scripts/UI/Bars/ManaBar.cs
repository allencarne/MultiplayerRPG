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
        stats.net_CurrentMana.OnValueChanged+= OnManaChanged;
        stats.net_BaseMana.OnValueChanged += OnMaxManaChanged;
        UpdateManaBar(stats.TotalMana, stats.net_CurrentMana.Value);

        if (IsServer && stats.net_CurrentMana.Value < stats.TotalMana)
        {
            if (!isRecharging) StartCoroutine(RechargeMana());
        }
    }

    public override void OnNetworkDespawn()
    {
        stats.net_CurrentMana.OnValueChanged -= OnManaChanged;
        stats.net_BaseMana.OnValueChanged -= OnMaxManaChanged;
    }

    public void SpendMana(float amount)
    {
        if (IsServer)
        {
            if (stats.net_CurrentMana.Value >= amount)
            {
                stats.net_CurrentMana.Value -= amount;

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
        if (stats.net_CurrentMana.Value >= amount)
        {
            stats.net_CurrentMana.Value -= amount;

            if (!isRecharging)
            {
                StartCoroutine(RechargeMana());
            }
        }
    }

    IEnumerator RechargeMana()
    {
        isRecharging = true;

        while (stats.net_CurrentMana.Value < stats.TotalMana)
        {
            yield return new WaitForSeconds(1);

            stats.net_CurrentMana.Value += stats.net_BaseManaRegen.Value;
            stats.net_CurrentMana.Value = Mathf.Min(stats.net_CurrentMana.Value, stats.net_CurrentMana.Value);
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
        UpdateManaBar(stats.TotalMana, newValue);
    }

    void OnMaxManaChanged(float oldValue, float newValue)
    {
        UpdateManaBar(newValue, stats.net_CurrentMana.Value);
    }
}
