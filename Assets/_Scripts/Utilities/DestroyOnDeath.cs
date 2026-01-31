using Unity.Netcode;
using UnityEngine;

public class DestroyOnDeath : NetworkBehaviour
{
    [HideInInspector] public CharacterStats stats;

    void Update()
    {
        if (!IsServer) return;

        if (stats != null && stats.isDead)
        {
            NetworkObject net = GetComponent<NetworkObject>();
            if (net != null)
            {
                net.Despawn(true);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
