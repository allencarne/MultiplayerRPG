using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Fury : PlayerSkill
{
    int furyHasteStacks = 0;
    int furyPerHit = 5;
    int furyFallOff = 5;
    int furyIdleTime = 8;

    Coroutine idleCoroutine;
    PlayerStateMachine _owner;

    [SerializeField] Material glow;
    [SerializeField] Image border;

    public override void StartSkill(PlayerStateMachine owner)
    {
        _owner = owner;
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
        Player player = GetComponentInParent<Player>();
        player.Fury.Value = Mathf.Min(player.Fury.Value + furyPerHit, player.MaxFury.Value);

        int newStacks = CalculateBuffStacks(player.Fury.Value);
        ApplyBuffClientRpc(newStacks);
    }

    IEnumerator IdleFuryDecay()
    {
        yield return new WaitForSeconds(furyIdleTime);

        while (_owner.player.Fury.Value > 0)
        {
            if (IsServer)
            {
                _owner.player.Fury.Value -= furyFallOff;
                int newStacks = CalculateBuffStacks(_owner.player.Fury.Value);
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
        Player player = GetComponentInParent<Player>();
        player.Fury.Value -= furyFallOff;

        int newStacks = CalculateBuffStacks(player.Fury.Value);
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
            _owner.Buffs.swiftness.StartConditionalSwiftness(delta);
        }

        furyHasteStacks = newStacks;
    }
}
