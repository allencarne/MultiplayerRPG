using Unity.Netcode;
using UnityEngine;

public class NPCEffects : NetworkBehaviour
{
    [SerializeField] CharacterStats stats;
    [SerializeField] GameObject death_Effect;
    [SerializeField] GameObject spawn_Effect;

    public override void OnNetworkSpawn()
    {
        SpawnClientRPC();

        stats.OnDeath.AddListener(DeathClientRPC);
    }

    public override void OnNetworkDespawn()
    {
        stats.OnDeath.RemoveListener(DeathClientRPC);
    }

    [ClientRpc]
    void SpawnClientRPC()
    {
        Instantiate(spawn_Effect, transform.position, transform.rotation);
    }

    [ClientRpc]
    void DeathClientRPC()
    {
        Instantiate(death_Effect, transform.position, transform.rotation);
    }
}
