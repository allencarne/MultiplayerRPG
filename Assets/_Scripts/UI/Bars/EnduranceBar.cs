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
        // Subscribe to the current Endurance NetworkVariable so the UI updates whenever Endurance changes.
        stats.net_CurrentEndurance.OnValueChanged += OnEnduranceChanged;

        // Subscribe to the base Endurance NetworkVariable so the UI updates when the player's base maximum Endurance changes.
        stats.net_BaseEndurance.OnValueChanged += OnBaseEnduranceChanged;

        // Initialize the Endurance bar using the current Endurance and calculated total Endurance values.
        UpdateEnduranceBar(stats.TotalEndurance, stats.net_CurrentEndurance.Value);
    }

    public override void OnNetworkDespawn()
    {
        // Unsubscribe from the current Endurance NetworkVariable to prevent callbacks after this object is despawned.
        stats.net_CurrentEndurance.OnValueChanged -= OnEnduranceChanged;

        // Unsubscribe from the base Endurance NetworkVariable to prevent callbacks after this object is despawned.
        stats.net_BaseEndurance.OnValueChanged -= OnBaseEnduranceChanged;
    }

    public void SpendEndurance(float amount)
    {
        // Check whether this code is currently running on the server.
        if (IsServer)
        {
            // Check whether the player has enough Endurance to pay the cost.
            if (stats.net_CurrentEndurance.Value >= amount)
            {
                // Subtract the Endurance cost from the player's current Endurance.
                stats.net_CurrentEndurance.Value -= amount;

                // Check whether Endurance is not already being recharged.
                if (!isRecharging)
                {
                    // Start the Endurance recharge coroutine.
                    StartCoroutine(RechargeEndurance());
                }
            }
        }
        else
        {
            // Ask the server to spend Endurance because only the server can modify the NetworkVariable.
            SpendEnduranceServerRpc(amount);
        }
    }

    [ServerRpc]
    void SpendEnduranceServerRpc(float amount)
    {
        // Check whether the player has enough Endurance to pay the cost.
        if (stats.net_CurrentEndurance.Value >= amount)
        {
            // Subtract the Endurance cost from the player's current Endurance.
            stats.net_CurrentEndurance.Value -= amount;

            // Check whether Endurance is not already being recharged.
            if (!isRecharging)
            {
                // Start the Endurance recharge coroutine on the server.
                StartCoroutine(RechargeEndurance());
            }
        }
    }

    IEnumerator RechargeEndurance()
    {
        // Mark Endurance as currently recharging so another recharge coroutine cannot be started.
        isRecharging = true;

        // Continue recharging until current Endurance reaches total maximum Endurance.
        while (stats.net_CurrentEndurance.Value < stats.TotalEndurance)
        {
            // Wait for the player's total Endurance regeneration interval before restoring Endurance.
            yield return new WaitForSeconds(stats.TotalEnduranceRegen);

            // Add 5 Endurance to the player's current Endurance.
            stats.net_CurrentEndurance.Value += 5f;

            // Prevent current Endurance from exceeding the player's calculated maximum Endurance.
            stats.net_CurrentEndurance.Value = Mathf.Min(
                stats.net_CurrentEndurance.Value,
                stats.TotalEndurance
            );
        }

        // Mark Endurance as no longer recharging once maximum Endurance has been reached.
        isRecharging = false;
    }

    void UpdateEnduranceBar(float maxEndurance, float currentEndurance)
    {
        // Stop here if maximum Endurance is zero or below to prevent division by zero.
        if (maxEndurance <= 0) return;

        // Calculate the percentage of Endurance remaining and immediately update the front bar.
        enduranceBar.fillAmount = currentEndurance / maxEndurance;

        // Check whether an existing background-bar interpolation coroutine is running.
        if (lerpCoroutine != null)
        {
            // Stop the previous interpolation so it does not compete with the new one.
            StopCoroutine(lerpCoroutine);
        }

        // Start a new interpolation toward the new Endurance percentage.
        lerpCoroutine = StartCoroutine(
            LerpEnduranceBar(currentEndurance / maxEndurance)
        );
    }

    IEnumerator LerpEnduranceBar(float targetFillAmount)
    {
        // Store the background bar's current fill amount as the starting point for the interpolation.
        float currentFillAmount = enduranceBar_Back.fillAmount;

        // Continue interpolating until the background bar is approximately equal to the target.
        while (!Mathf.Approximately(currentFillAmount, targetFillAmount))
        {
            // Move the background bar toward the target percentage using the configured lerp speed.
            currentFillAmount = Mathf.Lerp(
                currentFillAmount,
                targetFillAmount,
                lerpSpeed * Time.deltaTime
            );

            // Apply the interpolated percentage to the background bar.
            enduranceBar_Back.fillAmount = currentFillAmount;

            // Wait until the next frame before continuing the interpolation.
            yield return null;
        }
    }

    void OnEnduranceChanged(float oldValue, float newValue)
    {
        // Update the UI using the new current Endurance and the player's calculated maximum Endurance.
        UpdateEnduranceBar(stats.TotalEndurance, newValue);
    }


    void OnBaseEnduranceChanged(float oldValue, float newValue)
    {
        // Update the UI because the player's base maximum Endurance has changed.
        UpdateEnduranceBar(stats.TotalEndurance, stats.net_CurrentEndurance.Value);
    }
}
