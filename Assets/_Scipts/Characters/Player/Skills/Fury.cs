using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Fury : PlayerAbility
{
    int furyHasteStacks = 0;
    int furyPerHit = 5;
    int furyFallOff = 5;
    int furyIdleTime = 8;

    Dictionary<PlayerStateMachine, Coroutine> idleCoroutines = new();

    public override void StartAbility(PlayerStateMachine owner)
    {
        if (!IsServer) return;
        DamageOnTrigger.OnBasicAttack.AddListener(GainFury);
    }

    public override void UpdateAbility(PlayerStateMachine owner)
    {

    }

    public override void FixedUpdateAbility(PlayerStateMachine owner)
    {

    }

    void GainFury(NetworkObject attacker)
    {
        if (!IsServer) return;

        PlayerStateMachine owner = attacker.GetComponent<PlayerStateMachine>();
        if (owner == null) return;

        owner.player.Fury.Value = Mathf.Min(owner.player.Fury.Value + furyPerHit, owner.player.MaxFury.Value);
        UpdateFuryBuff(owner);

        if (idleCoroutines.TryGetValue(owner, out Coroutine existing))
        {
            StopCoroutine(existing);
        }

        Coroutine newCoroutine = StartCoroutine(IdleFuryDecay(owner));
        idleCoroutines[owner] = newCoroutine;
    }

    IEnumerator IdleFuryDecay(PlayerStateMachine owner)
    {
        yield return new WaitForSeconds(furyIdleTime);

        while (owner.player.Fury.Value > 0)
        {
            owner.player.Fury.Value -= furyFallOff;
            UpdateFuryBuff(owner);
            yield return new WaitForSeconds(1f);
        }

        idleCoroutines.Remove(owner);
    }

    void UpdateFuryBuff(PlayerStateMachine owner)
    {
        float fury = owner.player.Fury.Value;
        int newStacks = 0;

        if (fury >= 100) newStacks = 5;
        else if (fury >= 80) newStacks = 4;
        else if (fury >= 60) newStacks = 3;
        else if (fury >= 40) newStacks = 2;
        else if (fury >= 20) newStacks = 1;

        int delta = newStacks - furyHasteStacks;

        if (delta > 0)
        {
            owner.Buffs.SetConditionalHaste(delta);
        }
        else if (delta < 0)
        {
            owner.Buffs.RemoveConditionalHaste(-delta);
        }

        furyHasteStacks = newStacks; // Update tracker
    }

    public override void OnDestroy()
    {
        if (!IsServer) return;
        DamageOnTrigger.OnBasicAttack.RemoveListener(GainFury);

        foreach (var coroutine in idleCoroutines.Values)
        {
            StopCoroutine(coroutine);
        }
        idleCoroutines.Clear();
    }
}
