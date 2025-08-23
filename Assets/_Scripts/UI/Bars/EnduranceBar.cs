using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class EnduranceBar : NetworkBehaviour
{
    [SerializeField] Player player;
    [SerializeField] Image enduranceBar;
    [SerializeField] Image enduranceBar_Back;
    bool isRecharging = false;

    private float lerpSpeed = 5f;
    private Coroutine lerpCoroutine;

    public void SpendEndurance(float amount)
    {
        if (IsServer)
        {
            if (player.Endurance.Value >= amount)
            {
                player.Endurance.Value -= amount;

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
        if (player.Endurance.Value >= amount)
        {
            player.Endurance.Value -= amount;

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

        while (player.Endurance.Value < player.MaxEndurance.Value)
        {
            yield return new WaitForSeconds(0.2f); // How fast it recharges

            player.Endurance.Value += 5f;
            player.Endurance.Value = Mathf.Min(player.Endurance.Value, player.MaxEndurance.Value);
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
}
