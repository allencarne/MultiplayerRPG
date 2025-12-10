using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Buff_Regeneration : NetworkBehaviour
{
    [SerializeField] CharacterStats stats;

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
        UIClientRpc(duration);

        float elapsed = 0f;
        float nextHealTime = rate;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            if (elapsed >= nextHealTime)
            {
                stats.GiveHeal(amount,type);
                nextHealTime += rate;
            }

            yield return null;
        }
    }

    [ClientRpc]
    void UIClientRpc(float duration)
    {
        GameObject UI = Instantiate(buff_Regeneration, buffBar.transform);
        StatusEffects fill = UI.GetComponent<StatusEffects>();
        StartCoroutine(UpdateUIFill(UI, fill, duration));
    }

    IEnumerator UpdateUIFill(GameObject uiObject, StatusEffects fill, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            if (fill != null)
                fill.UpdateFill(elapsed / duration);

            yield return null;
        }

        Destroy(uiObject);
    }
}
