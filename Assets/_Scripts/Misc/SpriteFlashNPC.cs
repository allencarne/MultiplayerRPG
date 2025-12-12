using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class SpriteFlashNPC : NetworkBehaviour
{
    [SerializeField] NPC npc;

    [SerializeField] SpriteRenderer bodySprite;
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
        bodySprite.color = npc.Data.skinColor;
        yield return new WaitForSeconds(0.05f);

        bodySprite.material = flashMaterial;
        bodySprite.color = Color.white;
        yield return new WaitForSeconds(0.05f);

        bodySprite.material = defaultMaterial;
        bodySprite.color = npc.Data.skinColor;

        flashRoutine = null;
    }
}
