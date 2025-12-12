using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class SpriteFlashNPC : NetworkBehaviour
{
    [SerializeField] NPC npc;

    [SerializeField] SpriteRenderer bodySprite;
    [SerializeField] CharacterStats stats;

    [SerializeField] Material defaultMaterial;
    [SerializeField] Material flashRedMaterial;
    [SerializeField] Material flashGreenMaterial;

    Coroutine flashRoutine;

    [ClientRpc]
    public void FlashRedClientRPC(float amount)
    {
        if (flashRoutine != null) StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(FlashRed());
    }

    IEnumerator FlashRed()
    {
        bodySprite.material = flashRedMaterial;
        bodySprite.color = Color.white;
        yield return new WaitForSeconds(0.05f);

        bodySprite.material = defaultMaterial;
        bodySprite.color = npc.Data.skinColor;
        yield return new WaitForSeconds(0.05f);

        bodySprite.material = flashRedMaterial;
        bodySprite.color = Color.white;
        yield return new WaitForSeconds(0.05f);

        bodySprite.material = defaultMaterial;
        bodySprite.color = npc.Data.skinColor;

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
        bodySprite.material = flashGreenMaterial;
        bodySprite.color = Color.white;
        yield return new WaitForSeconds(0.05f);

        bodySprite.material = defaultMaterial;
        bodySprite.color = npc.Data.skinColor;
        yield return new WaitForSeconds(0.05f);

        bodySprite.material = flashGreenMaterial;
        bodySprite.color = Color.white;
        yield return new WaitForSeconds(0.05f);

        bodySprite.material = defaultMaterial;
        bodySprite.color = npc.Data.skinColor;

        flashRoutine = null;
    }
}
