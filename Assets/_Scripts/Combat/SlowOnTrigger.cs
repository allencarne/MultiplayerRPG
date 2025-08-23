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

        NetworkObject targetNetObj = collision.GetComponent<NetworkObject>();
        if (targetNetObj == null || targetNetObj == attacker) return;
        if (slowedObjects.Contains(targetNetObj)) return;

        // Mark as slowed so we don't repeat it
        slowedObjects.Add(targetNetObj);

        // Tell all clients to apply the slow if they own the object
        SlowClientRPC(targetNetObj, Stacks, Duration);
    }

    [ClientRpc]
    void SlowClientRPC(NetworkObjectReference targetRef, int stacks, float duration)
    {
        if (!targetRef.TryGet(out NetworkObject netObj)) return;
        if (!netObj.IsOwner) return;

        ISlowable slowable = netObj.GetComponentInChildren<ISlowable>();
        if (slowable != null)
        {
            slowable.StartSlow(stacks, duration);
        }
    }
}
