using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class HasteOnTrigger : NetworkBehaviour
{
    public int Stacks;
    public float Duration;
    public NetworkObject attacker;

    public bool IgnorePlayer;
    public bool IgnoreEnemy;
    public bool IgnoreNPC;

    public bool IsRepeatable;
    public float CooldownDuration;

    private HashSet<NetworkObject> hastedObjects = new HashSet<NetworkObject>();
    private Dictionary<NetworkObject, float> cooldowns = new Dictionary<NetworkObject, float>();
    public UnityEvent<float> OnCoolDownStarted;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServer) return;

        if (collision.CompareTag("Player") && IgnorePlayer) return;
        if (collision.CompareTag("Enemy") && IgnoreEnemy) return;
        if (collision.CompareTag("NPC") && IgnoreNPC) return;

        // Prevent Attacking Self
        NetworkObject objectThatWasHit = collision.GetComponent<NetworkObject>();
        if (objectThatWasHit == null || objectThatWasHit == attacker) return;

        // Haste
        if (!IsRepeatable)
        {
            if (hastedObjects.Contains(objectThatWasHit)) return;
            hastedObjects.Add(objectThatWasHit);
        }
        else
        {
            if (cooldowns.ContainsKey(objectThatWasHit))
            {
                if (Time.time < cooldowns[objectThatWasHit]) return;
            }
            cooldowns[objectThatWasHit] = Time.time + CooldownDuration;


            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { objectThatWasHit.OwnerClientId }
                }
            };
            ShowCooldownClientRpc(CooldownDuration, clientRpcParams);
        }

        HasteClientRPC(objectThatWasHit, Stacks, Duration);
    }

    [ClientRpc]
    void HasteClientRPC(NetworkObjectReference targetRef, int stacks, float duration)
    {
        if (!targetRef.TryGet(out NetworkObject netObj)) return;
        if (!netObj.IsOwner) return;

        IHasteable hasteable = netObj.GetComponentInChildren<IHasteable>();
        if (hasteable != null) hasteable.StartHaste(stacks, duration);
    }

    [ClientRpc]
    void ShowCooldownClientRpc(float duration, ClientRpcParams clientRpcParams = default)
    {
        OnCoolDownStarted?.Invoke(duration);
    }
}
