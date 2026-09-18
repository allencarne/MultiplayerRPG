using Unity.Netcode;
using UnityEngine;

public class DestroyOnDeath : NetworkBehaviour
{
    [HideInInspector] public CharacterStats stats;
    [HideInInspector] public CrowdControl crowdControl;

    [Header("Cancel Options")]
    [HideInInspector] public bool DestroyOnInterrupt;
    [HideInInspector] public bool DestroyOnStagger;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (stats != null)
        {
            stats.OnDeath.AddListener(DoDespawn);

            if (DestroyOnInterrupt) crowdControl.OnInterrupted.AddListener(DoDespawn);
        }

        if (crowdControl != null && DestroyOnStagger) crowdControl.OnStagger.AddListener(DoDespawn);
    }

    public override void OnNetworkDespawn()
    {
        if (stats != null)
        {
            stats.OnDeath.RemoveListener(DoDespawn);

            if (DestroyOnInterrupt) crowdControl.OnInterrupted.RemoveListener(DoDespawn);
        }

        if (crowdControl != null && DestroyOnStagger) crowdControl.OnStagger.RemoveListener(DoDespawn);

        base.OnNetworkDespawn();
    }

    void Update()
    {
        if (!IsServer) return;
        if (stats != null && stats.isDead) DoDespawn();
    }

    void DoDespawn()
    {
        NetworkObject net = GetComponent<NetworkObject>();
        if (net != null)
        {
            if (net.IsSpawned && IsServer)
            {
                // Despawn the object on the server, which will also despawn it on all clients
                net.Despawn(true);
            }
            else if (!IsServer)
            {
                // If this is not the server, we can just destroy the object locally. The server will handle despawning it for all clients.
                Destroy(gameObject);
            }
        }
        else
        {
            // If there is no NetworkObject, we can just destroy the object locally. This will not affect other clients, but it will remove the object from the scene.
            Destroy(gameObject);
        }
    }
}
