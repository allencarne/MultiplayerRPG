using Unity.Netcode;
using UnityEngine;

public class TotemParticles : NetworkBehaviour
{
    [Header("Effect")]
    [SerializeField] GameObject spawnParticle;
    [SerializeField] GameObject startParticle;
    [SerializeField] GameObject SuccessParticle;
    [SerializeField] GameObject failParticle;
    [SerializeField] GameObject despawnParticle;

    [Header("Visuals")]
    [SerializeField] Collider2D Trigger;
    [SerializeField] Collider2D Collider2d;
    [SerializeField] SpriteRenderer Sprite;
    [SerializeField] SpriteRenderer Shadow;
    [SerializeField] GameObject ParticleSystem;
    [SerializeField] Collider2D eventTrigger;

    public override void OnNetworkSpawn()
    {
        SpawnClientRPC();
    }

    [ClientRpc]
    public void SpawnClientRPC()
    {
        Instantiate(spawnParticle, transform.position, Quaternion.identity);
    }

    [ClientRpc]
    public void StartClientRPC()
    {
        Instantiate(startParticle, transform.position, Quaternion.identity);
    }

    [ClientRpc]
    public void SuccessClientRPC()
    {
        Instantiate(SuccessParticle, transform.position, Quaternion.identity);
    }

    [ClientRpc]
    public void FailClientRPC()
    {
        Instantiate(failParticle, transform.position, Quaternion.identity);
    }

    [ClientRpc]
    public void DespawnClientRPC(Vector2 transform)
    {
        Instantiate(despawnParticle, transform, Quaternion.identity);
    }

    [ClientRpc]
    public void ShowTotemClientRPC()
    {
        eventTrigger.enabled = true;

        Trigger.enabled = false;
        Collider2d.enabled = false;
        Sprite.enabled = false;
        Shadow.enabled = false;
        ParticleSystem.SetActive(false);
    }
}