using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class SpriteFlash : NetworkBehaviour
{
    [SerializeField] SpriteRenderer bodySprite;
    [SerializeField] PlayerStats playerStats;
    [SerializeField] CharacterStats stats;

    [SerializeField] Material flashMaterial;
    [SerializeField] Material defaultMaterial;

    Coroutine flashRoutine;

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
        if (flashRoutine != null) StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(FlashEffect());
    }

    IEnumerator FlashEffect()
    {
        bodySprite.material = flashMaterial;
        bodySprite.color = Color.white;
        yield return new WaitForSeconds(0.05f);

        bodySprite.material = defaultMaterial;
        bodySprite.color = playerStats.net_bodyColor.Value;
        yield return new WaitForSeconds(0.05f);

        bodySprite.material = flashMaterial;
        bodySprite.color = Color.white;
        yield return new WaitForSeconds(0.05f);

        bodySprite.material = defaultMaterial;
        bodySprite.color = playerStats.net_bodyColor.Value;

        flashRoutine = null;
    }
}
