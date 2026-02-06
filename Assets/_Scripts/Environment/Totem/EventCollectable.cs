using Unity.Netcode;
using UnityEngine;

public class EventCollectable : NetworkBehaviour
{
    [HideInInspector] public Totem TotemReference;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                if (IsServer)
                {
                    TotemReference.CollectEvent.Collected(player);
                    GetComponent<NetworkObject>().Despawn();
                }
            }
        }
    }
}
