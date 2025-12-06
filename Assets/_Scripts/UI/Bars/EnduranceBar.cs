using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class EnduranceBar : NetworkBehaviour
{
    [SerializeField] PlayerStats stats;
    [SerializeField] Image enduranceBar;
    [SerializeField] Image enduranceBar_Back;
    bool isRecharging = false;

    private float lerpSpeed = 5f;
    private Coroutine lerpCoroutine;

    public override void OnNetworkSpawn()
    {
        stats.Endurance.OnValueChanged += OnEnduranceChanged;
        stats.MaxEndurance.OnValueChanged += OnMaxEnduranceChanged;

        UpdateEnduranceBar(stats.MaxEndurance.Value, stats.Endurance.Value);
    }

    private void OnDisable()
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
            RequestDodgeServerRpc(amount);
        }
    }

    [ServerRpc]
    public void RequestDodgeServerRpc(float amount)
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

    public void UpdateEnduranceBar(float maxEndurance, float currentEndurance)
    {
        if (maxEndurance <= 0) return;
        enduranceBar.fillAmount = currentEndurance / maxEndurance;

        if (lerpCoroutine != null)
        {
            StopCoroutine(lerpCoroutine);
        }

        lerpCoroutine = StartCoroutine(LerpEnduranceBar(currentEndurance / maxEndurance));
    }

    private IEnumerator RechargeEndurance()
    {
        isRecharging = true;

        yield return new WaitForSeconds(1f); // Optional delay before recharge starts

        while (stats.Endurance.Value < stats.MaxEndurance.Value)
        {
            yield return new WaitForSeconds(0.2f); // How fast it recharges

            stats.Endurance.Value += 5f;
            stats.Endurance.Value = Mathf.Min(stats.Endurance.Value, stats.MaxEndurance.Value);
        }

        isRecharging = false;
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
