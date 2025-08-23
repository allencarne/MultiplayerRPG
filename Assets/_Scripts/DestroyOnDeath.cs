using Unity.Netcode;
using UnityEngine;

public class DestroyOnDeath : NetworkBehaviour
{
    [HideInInspector] public Player player;
    [HideInInspector] public Enemy enemy;

    void Update()
    {
        if (!IsServer) return;

        if (player != null)
        {
            if (player.IsDead)
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

        if (enemy != null)
        {
            if (enemy.isDead)
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
}
