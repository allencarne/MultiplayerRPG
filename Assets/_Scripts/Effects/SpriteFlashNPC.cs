using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class SpriteFlashNPC : NetworkBehaviour
{
    [SerializeField] SpriteRenderer headSprite;
    [SerializeField] SpriteRenderer bodySprite;

    [SerializeField] NPC npc;
    [SerializeField] CharacterStats stats;

    [SerializeField] Material defaultMaterial;
    [SerializeField] Material flashRedMaterial;
    [SerializeField] Material flashGreenMaterial;

    Coroutine flashRoutine;

    public override void OnNetworkSpawn()
    {
        stats.OnDamaged.AddListener(FlashRedClientRPC);
        stats.OnHealed.AddListener(FlashGreenClientRPC);
    }

    public override void OnNetworkDespawn()
    {
        stats.OnDamaged.RemoveListener(FlashRedClientRPC);
        stats.OnHealed.RemoveListener(FlashGreenClientRPC);
    }

    [ClientRpc]
    public void FlashRedClientRPC(float amount)
    {
        if (flashRoutine != null) StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(FlashRed());
    }

    IEnumerator FlashRed()
    {
        headSprite.material = flashRedMaterial;
        headSprite.color = Color.white;

        bodySprite.material = flashRedMaterial;
        bodySprite.color = Color.white;
        yield return new WaitForSeconds(0.05f);

        headSprite.material = defaultMaterial;
        headSprite.color = npc.Custom.skinColors[npc.Data.skinColorIndex];

        bodySprite.material = defaultMaterial;
        bodySprite.color = npc.Custom.skinColors[npc.Data.skinColorIndex];
        yield return new WaitForSeconds(0.05f);

        headSprite.material = flashRedMaterial;
        headSprite.color = Color.white;

        bodySprite.material = flashRedMaterial;
        bodySprite.color = Color.white;
        yield return new WaitForSeconds(0.05f);

        headSprite.material = defaultMaterial;
        headSprite.color = npc.Custom.skinColors[npc.Data.skinColorIndex];

        bodySprite.material = defaultMaterial;
        bodySprite.color = npc.Custom.skinColors[npc.Data.skinColorIndex];

        flashRoutine = null;
    }

    [ClientRpc]
    public void FlashGreenClientRPC(float amount)
    {
        if (flashRoutine != null) StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(FlashGreen());
    }

    IEnumerator FlashGreen()
    {
        headSprite.material = flashGreenMaterial;
        headSprite.color = Color.white;

        bodySprite.material = flashGreenMaterial;
        bodySprite.color = Color.white;
        yield return new WaitForSeconds(0.05f);

        headSprite.material = defaultMaterial;
        headSprite.color = npc.Custom.skinColors[npc.Data.skinColorIndex];

        bodySprite.material = defaultMaterial;
        bodySprite.color = npc.Custom.skinColors[npc.Data.skinColorIndex];
        yield return new WaitForSeconds(0.05f);

        headSprite.material = flashGreenMaterial;
        headSprite.color = Color.white;

        bodySprite.material = flashGreenMaterial;
        bodySprite.color = Color.white;
        yield return new WaitForSeconds(0.05f);

        headSprite.material = defaultMaterial;
        headSprite.color = npc.Custom.skinColors[npc.Data.skinColorIndex];

        bodySprite.material = defaultMaterial;
        bodySprite.color = npc.Custom.skinColors[npc.Data.skinColorIndex];

        flashRoutine = null;
    }
}
