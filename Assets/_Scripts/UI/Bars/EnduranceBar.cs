using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class EnduranceBar : NetworkBehaviour
{
    [Header("Stats")]
    [SerializeField] PlayerStats stats;

    [Header("UI")]
    [SerializeField] Image enduranceBar;
    [SerializeField] Image enduranceBar_Back;

    [Header("Variables")]
    bool isRecharging = false;
    float lerpSpeed = 5f;
    Coroutine lerpCoroutine;

    public override void OnNetworkSpawn()
    {
        stats.Endurance.OnValueChanged += OnEnduranceChanged;
        stats.MaxEndurance.OnValueChanged += OnMaxEnduranceChanged;

        UpdateEnduranceBar(stats.MaxEndurance.Value, stats.Endurance.Value);
    }

    public override void OnNetworkDespawn()
    {
        stats.Endurance.OnValueChanged -= OnEnduranceChanged;
        stats.MaxEndurance.OnValueChanged -= OnMaxEnduranceChanged;
    }

    public void SpendEndurance(float amount)
    {
        if (IsServer)
        {
            if (stats.Endurance.Value >= amount)
            {
                stats.Endurance.Value -= amount;

                if (!isRecharging)
                {
                    StartCoroutine(RechargeEndurance());
                }
            }
        }
        else
        {
            SpendEnduranceServerRpc(amount);
        }
    }

    [ServerRpc]
    void SpendEnduranceServerRpc(float amount)
    {
        if (stats.Endurance.Value >= amount)
        {
            stats.Endurance.Value -= amount;

            if (!isRecharging)
            {
                StartCoroutine(RechargeEndurance());
            }
        }
    }

    IEnumerator RechargeEndurance()
    {
        isRecharging = true;
        yield return new WaitForSeconds(1f);

        while (stats.Endurance.Value < stats.MaxEndurance.Value)
        {
            yield return new WaitForSeconds(stats.EnduranceRechargeRate.Value);

            stats.Endurance.Value += 5f;
            stats.Endurance.Value = Mathf.Min(stats.Endurance.Value, stats.MaxEndurance.Value);
        }

        isRecharging = false;
    }

    void UpdateEnduranceBar(float maxEndurance, float currentEndurance)
    {
        if (maxEndurance <= 0) return;
        enduranceBar.fillAmount = currentEndurance / maxEndurance;

        if (lerpCoroutine != null)
        {
            StopCoroutine(lerpCoroutine);
        }

        lerpCoroutine = StartCoroutine(LerpEnduranceBar(currentEndurance / maxEndurance));
    }

    IEnumerator LerpEnduranceBar(float targetFillAmount)
    {
        float currentFillAmount = enduranceBar_Back.fillAmount;

        while (!Mathf.Approximately(currentFillAmount, targetFillAmount))
        {
            currentFillAmount = Mathf.Lerp(currentFillAmount, targetFillAmount, lerpSpeed * Time.deltaTime);
            enduranceBar_Back.fillAmount = currentFillAmount;
            yield return null;
        }
    }

    void OnEnduranceChanged(float oldValue, float newValue)
    {
        UpdateEnduranceBar(stats.MaxEndurance.Value, newValue);
    }

    void OnMaxEnduranceChanged(float oldValue, float newValue)
    {
        UpdateEnduranceBar(newValue, stats.Endurance.Value);
    }
}
