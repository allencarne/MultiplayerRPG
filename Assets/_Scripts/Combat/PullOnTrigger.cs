using Unity.Netcode;
using UnityEngine;

public class PullOnTrigger : NetworkBehaviour
{
    [HideInInspector] public NetworkObject attacker;
    [HideInInspector] public Vector2 Direction;
    [HideInInspector] public float Amount;
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

        // Pull
        IPullable pullable = collision.GetComponentInChildren<IPullable>();
        if (pullable != null) pullable.Pull(Direction, Amount, Duration);
    }
}
