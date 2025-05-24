using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Fury : PlayerAbility
{
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

        // Gain Fury
        owner.player.Fury.Value = Mathf.Min(owner.player.Fury.Value + 5, owner.player.MaxFury.Value);
        UpdateFuryBuff(owner);

        // Restart that player's coroutine
        if (idleCoroutines.TryGetValue(owner, out Coroutine existing))
        {
            StopCoroutine(existing);
        }

        Coroutine newCoroutine = StartCoroutine(IdleFuryDecay(owner));
        idleCoroutines[owner] = newCoroutine;
    }

    IEnumerator IdleFuryDecay(PlayerStateMachine owner)
    {
        yield return new WaitForSeconds(8f);

        while (owner.player.Fury.Value > 0)
        {
            owner.player.Fury.Value--;
            UpdateFuryBuff(owner);
            yield return new WaitForSeconds(1f);
        }

        idleCoroutines.Remove(owner);
    }

    void UpdateFuryBuff(PlayerStateMachine owner)
    {
        float fury = owner.player.Fury.Value;
        int stacks = 0;

        if (fury >= 100) stacks = 5;
        else if (fury >= 80) stacks = 4;
        else if (fury >= 60) stacks = 3;
        else if (fury >= 40) stacks = 2;
        else if (fury >= 20) stacks = 1;

        owner.Buffs.SetExactConditionalHaste(stacks); // new method, see below
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
