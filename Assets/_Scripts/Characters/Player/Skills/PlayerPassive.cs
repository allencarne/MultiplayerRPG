using System;
using System.Collections;
using UnityEngine;

public class PlayerPassive
{
    public PassiveSkillData passiveData;
    int passiveIndex;
    bool onCooldown;
    Action unsubscribe;

    public PlayerPassive(PassiveSkillData data, int index)
    {
        passiveData = data;
        passiveIndex = index;
    }

    public virtual void StartPassive(PlayerStateMachine owner)
    {
        Debug.Log($"[PlayerPassive] Starting {passiveData.Name} on {owner.name}");

        if (passiveData.Trigger != null)
        {
            unsubscribe = passiveData.Trigger.Subscribe(owner, () => TryActivate(owner));
        }
    }

    public virtual void UpdatePassive(PlayerStateMachine owner)
    {

    }
    public virtual void FixedUpdatePassive(PlayerStateMachine owner)
    {

    }

    public virtual void EndPassive(PlayerStateMachine owner)
    {
        Debug.Log($"[PlayerPassive] Ending {passiveData.Name} on {owner.name}");
        unsubscribe?.Invoke();
        unsubscribe = null;
    }

    protected void TryActivate(PlayerStateMachine owner)
    {
        if (onCooldown)
        {
            Debug.Log($"[PlayerPassive] {passiveData.Name} tried to activate but is on cooldown");
            return;
        }

        Debug.Log($"[PlayerPassive] {passiveData.Name} activating on {owner.name}");

        if (passiveData.OnActivateEffects != null)
        {
            SkillContext ctx = BuildContext(owner);
            foreach (SkillEffect effect in passiveData.OnActivateEffects)
            {
                effect.Execute(owner, ctx);
            }
        }

        if (passiveData.CoolDown > 0)
        {
            owner.StartCoroutine(CooldownRoutine(owner));
        }
    }

    IEnumerator CooldownRoutine(PlayerStateMachine owner)
    {
        onCooldown = true;
        yield return new WaitForSeconds(passiveData.CoolDown);
        onCooldown = false;
        Debug.Log($"[PlayerPassive] {passiveData.Name} cooldown finished");
    }

    SkillContext BuildContext(PlayerStateMachine owner) => new SkillContext
    {
        AttackerId = owner.OwnerClientId,
        AttackerDamage = owner.PlayerStats.TotalDamage,
    };
}
