using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Fury : PlayerAbility
{
    int furyHasteStacks = 0;
    int furyPerHit = 5;
    int furyFallOff = 5;
    int furyIdleTime = 8;

    Coroutine idleCoroutine;
    PlayerStateMachine _owner;

    public override void StartAbility(PlayerStateMachine owner)
    {
        _owner = owner;
    }

    public override void UpdateAbility(PlayerStateMachine owner)
    {

    }

    public override void FixedUpdateAbility(PlayerStateMachine owner)
    {

    }

    [ClientRpc]
    public void FuryClientRPC()
    {
        if (!IsOwner) return;

        if (IsServer)
        {
            ApplyFury();
        }
        else
        {
            IncreaseFuryServerRPC();
        }

        if (idleCoroutine != null)
        {
            StopCoroutine(idleCoroutine);
        }

        idleCoroutine = StartCoroutine(IdleFuryDecay());
    }

    [ServerRpc]
    void IncreaseFuryServerRPC()
    {
        ApplyFury();
    }

    void ApplyFury()
    {
        Player player = GetComponentInParent<Player>();
        player.Fury.Value = Mathf.Min(player.Fury.Value + furyPerHit, player.MaxFury.Value);

        int newStacks = CalculateHasteStacks(player.Fury.Value);
        ApplyHasteClientRpc(newStacks);
    }

    IEnumerator IdleFuryDecay()
    {
        yield return new WaitForSeconds(furyIdleTime);

        while (_owner.player.Fury.Value > 0)
        {
            if (IsServer)
            {
                _owner.player.Fury.Value -= furyFallOff;
                int newStacks = CalculateHasteStacks(_owner.player.Fury.Value);
                ApplyHasteClientRpc(newStacks);
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
        Player player = GetComponentInParent<Player>();
        player.Fury.Value -= furyFallOff;

        int newStacks = CalculateHasteStacks(player.Fury.Value);
        ApplyHasteClientRpc(newStacks);
    }

    int CalculateHasteStacks(float fury)
    {
        if (fury >= 100) return 5;
        if (fury >= 80) return 4;
        if (fury >= 60) return 3;
        if (fury >= 40) return 2;
        if (fury >= 20) return 1;
        return 0;
    }


    [ClientRpc]
    void ApplyHasteClientRpc(int newStacks)
    {
        if (!IsOwner) return;

        int delta = newStacks - furyHasteStacks;
        if (delta != 0)
        {
            _owner.Buffs.haste.StartConditionalHaste(delta);
        }

        furyHasteStacks = newStacks;
    }
}
