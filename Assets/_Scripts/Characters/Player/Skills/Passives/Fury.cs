using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Fury : ActiveSkill
{
    int furyHasteStacks = 0;
    int furyPerHit = 5;
    int furyFallOff = 5;
    int furyIdleTime = 8;

    Coroutine idleCoroutine;
    [SerializeField] PlayerStateMachine stateMachine;

    public Fury(ActiveSkillData data, int index) : base(data, index)
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
        stateMachine.PlayerStats.Fury.Value = Mathf.Min(stateMachine.PlayerStats.Fury.Value + furyPerHit, stateMachine.PlayerStats.MaxFury.Value);
        int newStacks = CalculateBuffStacks(stateMachine.PlayerStats.Fury.Value);
        ApplyBuffClientRpc(newStacks);
    }

    IEnumerator IdleFuryDecay()
    {
        yield return new WaitForSeconds(furyIdleTime);

        while (stateMachine.PlayerStats.Fury.Value > 0)
        {
            if (stateMachine.IsServer)
            {
                stateMachine.PlayerStats.Fury.Value -= furyFallOff;
                int newStacks = CalculateBuffStacks(stateMachine.PlayerStats.Fury.Value);
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
        stateMachine.PlayerStats.Fury.Value -= furyFallOff;

        int newStacks = CalculateBuffStacks(stateMachine.PlayerStats.Fury.Value);
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
