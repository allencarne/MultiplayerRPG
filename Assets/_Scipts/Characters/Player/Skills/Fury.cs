using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Fury : PlayerAbility
{
    float idleTime;
    Coroutine timer;

    public override void StartAbility(PlayerStateMachine owner)
    {
        DamageOnTrigger.OnDamageDealt.AddListener(GainFury);
    }

    public override void UpdateAbility(PlayerStateMachine owner)
    {
        idleTime += Time.deltaTime;

        if (idleTime >= 8 && owner.player.Fury.Value > 0)
        {
            if (timer == null)
            {
                timer = StartCoroutine(DecreaseFury(owner));
            }
        }
        else if (timer != null)
        {
            StopCoroutine(DecreaseFury(owner));
            timer = null;
        }
    }

    IEnumerator DecreaseFury(PlayerStateMachine owner)
    {
        while (owner.player.Fury.Value > 0 && idleTime >= 8)
        {
            owner.player.Fury.Value--;
            yield return new WaitForSeconds(1f);
        }
    }

    public override void FixedUpdateAbility(PlayerStateMachine owner)
    {

    }

    void GainFury(NetworkObject attacker)
    {
        PlayerStateMachine owner = attacker.GetComponent<PlayerStateMachine>();
        if (owner != null)
        {
            owner.player.Fury.Value = Mathf.Min(owner.player.Fury.Value + 5, owner.player.MaxFury.Value);
            idleTime = 0;
            GiveBuff(owner);

            if (timer != null)
            {
                StopCoroutine(timer);
                timer = null;
            }
        }
    }

    void GiveBuff(PlayerStateMachine owner)
    {
        if (owner.player.Fury.Value >= 100)
        {
            owner.Buffs.Swiftness(5, 5);
        }
        else if (owner.player.Fury.Value >= 80)
        {
            owner.Buffs.Swiftness(4, 5);
        }
        else if (owner.player.Fury.Value >= 60)
        {
            owner.Buffs.Swiftness(3, 5);
        }
        else if (owner.player.Fury.Value >= 40)
        {
            owner.Buffs.Swiftness(2, 5);
        }
        else if (owner.player.Fury.Value >= 20)
        {
            owner.Buffs.Swiftness(1,5);
        }
    }

    public override void OnDestroy()
    {
        DamageOnTrigger.OnDamageDealt.RemoveListener(GainFury);
    }
}
