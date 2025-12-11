using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class SpriteFlash : NetworkBehaviour
{
    [SerializeField] SpriteRenderer bodySprite;
    [SerializeField] PlayerStats playerStats;
    [SerializeField] CharacterStats stats;

    public override void OnNetworkSpawn()
    {
        stats.OnDamaged.AddListener(TriggerFlashEffectClientRpc);
    }

    public override void OnNetworkDespawn()
    {
        stats.OnDamaged.RemoveListener(TriggerFlashEffectClientRpc);
    }

    [ClientRpc]
    public void TriggerFlashEffectClientRpc(float amount)
    {
        StartCoroutine(FlashEffect());
    }

    IEnumerator FlashEffect()
    {
        bodySprite.color = Color.white;
        yield return new WaitForSeconds(0.05f);

        bodySprite.color = playerStats.net_bodyColor.Value;
        yield return new WaitForSeconds(0.05f);

        bodySprite.color = Color.white;
        yield return new WaitForSeconds(0.05f);

        bodySprite.color = playerStats.net_bodyColor.Value;
    }
}
