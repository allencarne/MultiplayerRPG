using Unity.Netcode;
using UnityEngine;

public class KnockupOnTrigger : NetworkBehaviour
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

        // Skip Immovable
        Buffs buffs = collision.GetComponent<Buffs>();
        if (buffs != null && buffs.immoveable.IsImmovable) return;

        // KnockUp
        IKnockupable knockupable = collision.GetComponentInChildren<IKnockupable>();
        if (knockupable != null) knockupable.StartKnockUp(Duration);
    }
}
