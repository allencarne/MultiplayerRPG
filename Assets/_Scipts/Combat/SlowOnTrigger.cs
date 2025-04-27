using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SlowOnTrigger : NetworkBehaviour
{
    [HideInInspector] public int Stacks;
    [HideInInspector] public float Duration;

    [HideInInspector] public NetworkObject attacker;
    [HideInInspector] public bool IgnoreEnemy;

    private HashSet<NetworkObject> slowedObjects = new HashSet<NetworkObject>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServer) return;

        if (collision.CompareTag("Enemy") && IgnoreEnemy) return;

        NetworkObject objectThatWasHit = collision.GetComponent<NetworkObject>();
        if (objectThatWasHit != null)
        {
            if (objectThatWasHit == attacker)
            {
                return;
            }
        }

        if (slowedObjects.Contains(objectThatWasHit)) return;

        ISlowable slowable = collision.GetComponent<ISlowable>();
        if (slowable != null)
        {
            slowable.Slow(Stacks, Duration);
            slowedObjects.Add(objectThatWasHit);
        }
    }
}
