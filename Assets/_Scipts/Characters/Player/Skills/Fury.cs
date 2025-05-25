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
            _owner.player.Fury.Value = Mathf.Min(_owner.player.Fury.Value + furyPerHit, _owner.player.MaxFury.Value);
            UpdateFuryBuff();
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
        Player player = GetComponentInParent<Player>();
        player.Fury.Value = Mathf.Min(player.Fury.Value + furyPerHit, player.MaxFury.Value);
        UpdateFuryBuff();
    }

    IEnumerator IdleFuryDecay()
    {
        yield return new WaitForSeconds(furyIdleTime);

        while (_owner.player.Fury.Value > 0)
        {
            if (IsServer)
            {
                _owner.player.Fury.Value -= furyFallOff;
                UpdateFuryBuff();
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
        UpdateFuryBuff();
    }

    void UpdateFuryBuff()
    {
        Player player = GetComponentInParent<Player>();
        PlayerStateMachine stateMachine = GetComponentInParent<PlayerStateMachine>();

        float fury = player.Fury.Value;
        int newStacks = 0;

        if (fury >= 100) newStacks = 5;
        else if (fury >= 80) newStacks = 4;
        else if (fury >= 60) newStacks = 3;
        else if (fury >= 40) newStacks = 2;
        else if (fury >= 20) newStacks = 1;

        int delta = newStacks - furyHasteStacks;
        if (delta != 0)
        {
            stateMachine.Buffs.ConditionalHaste(delta);
        }

        furyHasteStacks = newStacks;
    }
}
