using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Fury : PlayerSkill
{
    int furyHasteStacks = 0;
    int furyPerHit = 5;
    int furyFallOff = 5;
    int furyIdleTime = 8;

    Coroutine idleCoroutine;
    PlayerStateMachine stateMachine;

    public override void StartSkill(PlayerStateMachine owner)
    {
        stateMachine = owner;
    }

    public void SetFury()
    {
        Debug.Log("SetFury " + OwnerClientId);

        if (IsServer)
        {
            ApplyFury();
        }
        else
        {
            ApplyFuryServerRPC();
        }

        if (idleCoroutine != null)
        {
            StopCoroutine(idleCoroutine);
        }

        idleCoroutine = StartCoroutine(IdleFuryDecay());
    }

    [ServerRpc]
    void ApplyFuryServerRPC()
    {
        ApplyFury();
    }

    void ApplyFury()
    {
        stateMachine.Stats.Fury.Value = Mathf.Min(stateMachine.Stats.Fury.Value + furyPerHit, stateMachine.Stats.MaxFury.Value);

        int newStacks = CalculateBuffStacks(stateMachine.Stats.Fury.Value);
        ApplyBuffClientRpc(newStacks);
    }

    IEnumerator IdleFuryDecay()
    {
        yield return new WaitForSeconds(furyIdleTime);

        while (stateMachine.Stats.Fury.Value > 0)
        {
            if (IsServer)
            {
                stateMachine.Stats.Fury.Value -= furyFallOff;
                int newStacks = CalculateBuffStacks(stateMachine.Stats.Fury.Value);
                ApplyBuffClientRpc(newStacks);
            }
            else
            {
                DecayFuryServerRPC();
            }
            yield return new WaitForSeconds(1f);
        }
    }

    [ServerRpc]
    void DecayFuryServerRPC()
    {
        stateMachine.Stats.Fury.Value -= furyFallOff;

        int newStacks = CalculateBuffStacks(stateMachine.Stats.Fury.Value);
        ApplyBuffClientRpc(newStacks);
    }

    int CalculateBuffStacks(float fury)
    {
        if (fury >= 100) return 5;
        if (fury >= 80) return 4;
        if (fury >= 60) return 3;
        if (fury >= 40) return 2;
        if (fury >= 20) return 1;
        return 0;
    }

    [ClientRpc]
    void ApplyBuffClientRpc(int newStacks)
    {
        if (!IsOwner) return;

        int delta = newStacks - furyHasteStacks;
        if (delta != 0)
        {
            stateMachine.Buffs.swiftness.StartSwiftness(delta, -1);
        }

        furyHasteStacks = newStacks;
    }
}
