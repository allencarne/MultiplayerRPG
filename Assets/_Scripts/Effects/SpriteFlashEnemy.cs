using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class SpriteFlashEnemy : NetworkBehaviour
{
    [SerializeField] CharacterStats stats;
    [SerializeField] SpriteRenderer bodySprite;

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
        bodySprite.material = flashRedMaterial;
        bodySprite.color = Color.red;
        yield return new WaitForSeconds(0.05f);

        bodySprite.material = defaultMaterial;
        bodySprite.color = Color.white;
        yield return new WaitForSeconds(0.05f);

        bodySprite.material = flashRedMaterial;
        bodySprite.color = Color.red;
        yield return new WaitForSeconds(0.05f);

        bodySprite.material = defaultMaterial;
        bodySprite.color = Color.white;

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
        bodySprite.color = Color.green;
        yield return new WaitForSeconds(0.05f);

        bodySprite.material = defaultMaterial;
        bodySprite.color = Color.white;
        yield return new WaitForSeconds(0.05f);

        bodySprite.material = flashGreenMaterial;
        bodySprite.color = Color.green;
        yield return new WaitForSeconds(0.05f);

        bodySprite.material = defaultMaterial;
        bodySprite.color = Color.white;

        flashRoutine = null;
    }
}
