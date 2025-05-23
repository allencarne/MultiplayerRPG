using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Fury : PlayerAbility
{
    Coroutine idleCoroutine;

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
        GiveBuff(owner);

        // Restart idle coroutine
        if (idleCoroutine != null)
        {
            StopCoroutine(idleCoroutine);
        }
        idleCoroutine = StartCoroutine(IdleFuryDecay(owner));
    }

    IEnumerator IdleFuryDecay(PlayerStateMachine owner)
    {
        yield return new WaitForSeconds(8f);

        while (owner.player.Fury.Value > 0)
        {
            owner.player.Fury.Value--;
            yield return new WaitForSeconds(1f);
        }

        idleCoroutine = null;
    }

    void GiveBuff(PlayerStateMachine owner)
    {
        if (owner.player.Fury.Value >= 100)
        {
            //owner.Buffs.Swiftness(5, 5);
        }
        else if (owner.player.Fury.Value >= 80)
        {
            //owner.Buffs.Swiftness(4, 5);
        }
        else if (owner.player.Fury.Value >= 60)
        {
            //owner.Buffs.Swiftness(3, 5);
        }
        else if (owner.player.Fury.Value >= 40)
        {
            //owner.Buffs.Swiftness(2, 5);
        }
        else if (owner.player.Fury.Value >= 20)
        {
            //owner.Buffs.Swiftness(1,5);
        }
    }

    public override void OnDestroy()
    {
        if (!IsServer) return;
        DamageOnTrigger.OnBasicAttack.RemoveListener(GainFury);
    }
}
