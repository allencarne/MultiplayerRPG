using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SlowOnTrigger : NetworkBehaviour
{
    public int Stacks;
    public float Duration;
    public NetworkObject attacker;

    public bool IgnorePlayer;
    public bool IgnoreEnemy;
    public bool IgnoreNPC;

    public bool IsRepeatable;

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
        if (!IsRepeatable)
        {
            if (slowedObjects.Contains(objectThatWasHit)) return;
        }
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
