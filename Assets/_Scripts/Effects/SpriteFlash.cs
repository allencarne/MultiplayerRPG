using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class SpriteFlash : NetworkBehaviour
{
    [SerializeField] SpriteRenderer headSprite;
    [SerializeField] SpriteRenderer bodySprite;

    [SerializeField] PlayerStats playerStats;
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
        headSprite.color = playerStats.net_bodyColor.Value;

        bodySprite.material = defaultMaterial;
        bodySprite.color = playerStats.net_bodyColor.Value;
        yield return new WaitForSeconds(0.05f);

        headSprite.material = flashRedMaterial;
        headSprite.color = Color.white;

        bodySprite.material = flashRedMaterial;
        bodySprite.color = Color.white;
        yield return new WaitForSeconds(0.05f);

        headSprite.material = defaultMaterial;
        headSprite.color = playerStats.net_bodyColor.Value;

        bodySprite.material = defaultMaterial;
        bodySprite.color = playerStats.net_bodyColor.Value;

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
        headSprite.color = playerStats.net_bodyColor.Value;

        bodySprite.material = defaultMaterial;
        bodySprite.color = playerStats.net_bodyColor.Value;
        yield return new WaitForSeconds(0.05f);

        headSprite.material = flashGreenMaterial;
        headSprite.color = Color.white;

        bodySprite.material = flashGreenMaterial;
        bodySprite.color = Color.white;
        yield return new WaitForSeconds(0.05f);

        headSprite.material = defaultMaterial;
        headSprite.color = playerStats.net_bodyColor.Value;

        bodySprite.material = defaultMaterial;
        bodySprite.color = playerStats.net_bodyColor.Value;

        flashRoutine = null;
    }
}
