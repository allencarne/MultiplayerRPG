using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Buff_Regeneration : NetworkBehaviour
{
    [SerializeField] Player player;
    [SerializeField] Enemy enemy;

    [SerializeField] GameObject buffBar;
    [SerializeField] GameObject buff_Regeneration;

    public void Regeneration(HealType type, float amount, float rate, float duration)
    {
        if (!IsOwner) return;

        if (IsServer)
        {
            StartCoroutine(Duration(type, amount, rate, duration));
        }
        else
        {
            RequestRegenerationServerRPC(type, amount, rate, duration);
        }
    }

    [ServerRpc]
    void RequestRegenerationServerRPC(HealType type, float amount, float rate, float duration)
    {
        StartCoroutine(Duration(type, amount, rate, duration));
    }

    IEnumerator Duration(HealType type, float amount, float rate, float duration)
    {
        float elapsed = 0f;
        float nextHealTime = rate;

        GameObject UI = Instantiate(buff_Regeneration, buffBar.transform);
        StatusEffects fill = UI.GetComponent<StatusEffects>();

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            if (fill != null)
                fill.UpdateFill(elapsed / duration);

            if (elapsed >= nextHealTime)
            {
                player.GiveHeal(amount, type);
                nextHealTime += rate;
            }

            yield return null;
        }

        Destroy(UI);
    }
}
