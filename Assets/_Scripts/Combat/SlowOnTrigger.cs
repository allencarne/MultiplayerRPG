using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SlowOnTrigger : NetworkBehaviour
{
    [HideInInspector] public int Stacks;
    [HideInInspector] public float Duration;
    [HideInInspector] public NetworkObject attacker;

    [HideInInspector] public bool IgnorePlayer;
    [HideInInspector] public bool IgnoreEnemy;
    [HideInInspector] public bool IgnoreNPC;

    private HashSet<NetworkObject> slowedObjects = new HashSet<NetworkObject>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServer) return;

        if (collision.CompareTag("Player") && IgnorePlayer) return;
        if (collision.CompareTag("Enemy") && IgnoreEnemy) return;
        if (collision.CompareTag("NPC") && IgnoreNPC) return;

        // Prevent Attacking Self
        NetworkObject objectThatWasHit = collision.GetComponent<NetworkObject>();
        if (objectThatWasHit != null && objectThatWasHit == attacker) return;

        // Slow
        if (slowedObjects.Contains(objectThatWasHit)) return;
        slowedObjects.Add(objectThatWasHit);
        SlowClientRPC(objectThatWasHit, Stacks, Duration);
    }

    [ClientRpc]
    void SlowClientRPC(NetworkObjectReference targetRef, int stacks, float duration)
    {
        if (!targetRef.TryGet(out NetworkObject netObj)) return;
        if (!netObj.IsOwner) return;

        ISlowable slowable = netObj.GetComponentInChildren<ISlowable>();
        if (slowable != null) slowable.StartSlow(stacks, duration);
    }
}
