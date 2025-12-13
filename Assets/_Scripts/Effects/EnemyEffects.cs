using Unity.Netcode;
using UnityEngine;

public class EnemyEffects : NetworkBehaviour
{
    [SerializeField] CharacterStats stats;
    
    [SerializeField] GameObject spawn_Effect;
    [SerializeField] GameObject death_Effect;

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
