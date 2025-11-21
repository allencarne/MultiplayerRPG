using Unity.Netcode;
using UnityEngine;

public class EventCollectable : NetworkBehaviour
{
    [HideInInspector] public Totem TotemReference;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            TotemReference.CollectEvent.Collected();
            GetComponent<NetworkObject>().Despawn();
        }
    }
}
