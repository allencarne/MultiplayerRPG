using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class SpriteFlashNPC : NetworkBehaviour
{
    [SerializeField] NPC npc;
    [SerializeField] CharacterStats stats;
    [SerializeField] CrowdControl crowdControl;

    [SerializeField] SpriteRenderer headSprite;
    [SerializeField] SpriteRenderer bodySprite;

    [SerializeField] Material defaultMaterial;
    [SerializeField] Material flashRedMaterial;
    [SerializeField] Material flashGreenMaterial;

    Coroutine flashRoutine;

    public override void OnNetworkSpawn()
    {
        stats.OnDamaged.AddListener(FlashRedClientRPC);
        stats.OnHealed.AddListener(FlashGreenClientRPC);
        crowdControl.OnStagger.AddListener(FlashWhite);
        crowdControl.OnStaggerEnd.AddListener(FlashWhiteEnd);
    }

    public override void OnNetworkDespawn()
    {
        stats.OnDamaged.RemoveListener(FlashRedClientRPC);
        stats.OnHealed.RemoveListener(FlashGreenClientRPC);
        crowdControl.OnStagger.RemoveListener(FlashWhite);
        crowdControl.OnStaggerEnd.RemoveListener(FlashWhiteEnd);
    }

    [ClientRpc]
    public void FlashRedClientRPC(float amount)
    {
        if (crowdControl.IsCrowdControlled) return;

        if (flashRoutine != null) StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(FlashRed());
    }

    IEnumerator FlashRed()
    {
        headSprite.material = flashRedMaterial;
        bodySprite.material = flashRedMaterial;

        yield return new WaitForSeconds(0.05f);

        headSprite.material = defaultMaterial;
        bodySprite.material = defaultMaterial;

        flashRoutine = null;
    }

    [ClientRpc]
    public void FlashGreenClientRPC(float amount)
    {
        if (crowdControl.IsCrowdControlled) return;

        if (flashRoutine != null) StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(FlashGreen());
    }

    IEnumerator FlashGreen()
    {
        headSprite.material = flashGreenMaterial;
        bodySprite.material = flashGreenMaterial;

        yield return new WaitForSeconds(0.05f);

        headSprite.material = defaultMaterial;
        bodySprite.material = defaultMaterial;

        flashRoutine = null;
    }

    void FlashWhite()
    {
        headSprite.color = Color.white;
        bodySprite.color = Color.white;
    }

    void FlashWhiteEnd()
    {
        headSprite.color = npc.Custom.skinColors[npc.Data.skinColorIndex];
        bodySprite.color = npc.Custom.skinColors[npc.Data.skinColorIndex];
    }
}
