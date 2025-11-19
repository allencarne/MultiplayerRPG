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
}
