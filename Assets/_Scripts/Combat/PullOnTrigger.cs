using Unity.Netcode;
using UnityEngine;

public class PullOnTrigger : NetworkBehaviour
{
    [HideInInspector] public NetworkObject attacker;
    [HideInInspector] public Vector2 Direction;
    [HideInInspector] public float Amount;
    [HideInInspector] public float Duration;
    [HideInInspector] public bool IgnoreEnemy;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServer) return;

        if (collision.CompareTag("Enemy"))
        {
            if (IgnoreEnemy)
            {
                return;
            }
        }

        NetworkObject objectThatWasHit = collision.GetComponent<NetworkObject>();
        if (objectThatWasHit != null)
        {
            if (objectThatWasHit == attacker)
            {
                return;
            }
        }

        Buffs buffs = collision.GetComponent<Buffs>();
        if (buffs != null)
        {
            if (buffs.immoveable.IsImmovable)
            {
                return;
            }
        }

        IPullable pullable = collision.GetComponentInChildren<IPullable>();
        if (pullable != null)
        {
            pullable.Pull(Direction, Amount, Duration);
        }
    }
}
