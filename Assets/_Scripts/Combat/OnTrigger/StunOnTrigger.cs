using Unity.Netcode;
using UnityEngine;

public class StunOnTrigger : NetworkBehaviour
{
    [HideInInspector] public NetworkObject attacker;
    [HideInInspector] public float Duration;

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

        // Stun
        IStunnable stunnable = collision.GetComponentInChildren<IStunnable>();
        if (stunnable != null) stunnable.StartStun(Duration);
    }
}
