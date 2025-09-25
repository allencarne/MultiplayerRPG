using Unity.Netcode;
using UnityEngine;

public class InterruptOnTrigger : NetworkBehaviour
{
    [HideInInspector] public NetworkObject attacker;

    [HideInInspector] public bool IgnorePlayer;
    [HideInInspector] public bool IgnoreEnemy;
    [HideInInspector] public bool IgnoreNPC;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServer) return;

        if (collision.CompareTag("Player") && IgnorePlayer) return;
        if (collision.CompareTag("Enemy") && IgnoreEnemy) return;
        if (collision.CompareTag("NPC") && IgnoreNPC) return;

        // Prevent Attacking Self
        NetworkObject objectThatWasHit = collision.GetComponent<NetworkObject>();
        if (objectThatWasHit != null && objectThatWasHit == attacker) return;

        // Interrupt
        IInterruptable interruptable = collision.GetComponentInChildren<IInterruptable>();
        if (interruptable != null) interruptable.Interrupt();
    }
}
