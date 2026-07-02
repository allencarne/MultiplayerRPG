using Unity.Netcode;
using UnityEngine;

public class NPCEffects : NetworkBehaviour
{
    [SerializeField] NPCStateMachine stateMachine;
    [SerializeField] CharacterStats stats;
    [SerializeField] GameObject death_Effect;
    [SerializeField] GameObject spawn_Effect;

    private void OnEnable()
    {
        stateMachine.OnSpawn.AddListener(SpawnClientRPC);
        stats.OnDeath.AddListener(DeathClientRPC);
    }

    private void OnDisable()
    {
        stateMachine.OnSpawn.RemoveListener(SpawnClientRPC);
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
