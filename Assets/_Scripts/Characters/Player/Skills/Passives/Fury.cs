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
    [SerializeField] PlayerStateMachine stateMachine;

    public Fury(SkillData data) : base(data)
    {
    }

    public override void StartSkill(PlayerStateMachine owner)
    {

    }

    [ClientRpc]
    public void FuryClientRPC(NetworkObjectReference attackerRef)
    {
        if (attackerRef.TryGet(out NetworkObject attackerObject))
        {
            if (attackerObject.IsOwner)
            {
                if (stateMachine.IsServer)
                {
                    ApplyFury();
                }
                else
                {
                    ApplyFuryServerRPC();
                }

                if (idleCoroutine != null)
                {
                    stateMachine.StopCoroutine(idleCoroutine);
                }

                idleCoroutine = stateMachine.StartCoroutine(IdleFuryDecay());
            }
        }
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
            if (stateMachine.IsServer)
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
        if (!stateMachine.IsOwner) return;

        int delta = newStacks - furyHasteStacks;
        if (delta != 0)
        {
            stateMachine.Buffs.swiftness.StartSwiftness(delta, -1);
        }

        furyHasteStacks = newStacks;
    }
}
