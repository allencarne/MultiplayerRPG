using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class HasteOnTrigger : NetworkBehaviour
{
    private struct CooldownData
    {
        public float NextAvailableTime;
        public bool HasBeenTriggered;
    }

    private Dictionary<NetworkObject, CooldownData> cooldownTracker = new Dictionary<NetworkObject, CooldownData>();

    [Header("Buff Settings")]
    public int Stacks;
    public float Duration;
    public float CooldownDuration;
    public NetworkObject attacker;

    [Header("Filter")]
    public bool IgnorePlayer;
    public bool IgnoreEnemy;
    public bool IgnoreNPC;

    [Header("Events")]
    public UnityEvent<float> OnCoolDownStarted;
    public UnityEvent OnTriggered;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject);

        if (!IsServer) return;
        TryApplyHaste(collision);
    }

    private void TryApplyHaste(Collider2D collision)
    {
        Debug.Log("TryApplyHaste");

        if (collision.CompareTag("Player") && IgnorePlayer) return;
        if (collision.CompareTag("Enemy") && IgnoreEnemy) return;
        if (collision.CompareTag("NPC") && IgnoreNPC) return;

        NetworkObject objectThatWasHit = collision.GetComponent<NetworkObject>();
        if (objectThatWasHit == null || objectThatWasHit == attacker || !objectThatWasHit.IsSpawned) return;

        // Check cooldown with single dictionary lookup
        if (!CanApplyHaste(objectThatWasHit, out bool isFirstTime)) return;

        // Update cooldown state
        UpdateCooldown(objectThatWasHit);

        // Only send cooldown RPC to players when there's an actual cooldown
        if (CooldownDuration > 0 && collision.CompareTag("Player"))
        {
            SendCooldownToClient(objectThatWasHit.OwnerClientId);
        }

        // Send effect RPCs
        SpawnParticleClientRpc();
        HasteClientRPC(objectThatWasHit, Stacks, Duration);
    }

    [ClientRpc]
    private void HasteClientRPC(NetworkObjectReference targetRef, int stacks, float duration)
    {
        if (!targetRef.TryGet(out NetworkObject netObj)) return;
        if (!netObj.IsOwner) return;

        // Cache component lookup if possible
        IHasteable hasteable = netObj.GetComponentInChildren<IHasteable>();
        if (hasteable != null)
        {
            hasteable.StartHaste(stacks, duration);
        }
    }

    [ClientRpc]
    private void ShowCooldownClientRpc(float duration, ClientRpcParams clientRpcParams = default)
    {
        OnCoolDownStarted?.Invoke(duration);
    }

    [ClientRpc]
    private void SpawnParticleClientRpc()
    {
        OnTriggered?.Invoke();
    }

    private void OnDisable()
    {
        cooldownTracker.Clear();
    }

    private bool CanApplyHaste(NetworkObject target, out bool isFirstTime)
    {
        isFirstTime = false;

        if (!cooldownTracker.TryGetValue(target, out CooldownData data))
        {
            // First time encountering this object
            isFirstTime = true;
            return true;
        }

        // If cooldown is 0, allow spam (removed IsRepeatable)
        if (CooldownDuration <= 0)
        {
            return true;
        }

        // Check if cooldown has expired
        return Time.time >= data.NextAvailableTime;
    }

    private void UpdateCooldown(NetworkObject target)
    {
        cooldownTracker[target] = new CooldownData
        {
            NextAvailableTime = Time.time + CooldownDuration,
            HasBeenTriggered = true
        };
    }

    private void SendCooldownToClient(ulong clientId)
    {
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientId }
            }
        };
        ShowCooldownClientRpc(CooldownDuration, clientRpcParams);
    }
}
