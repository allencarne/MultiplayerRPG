using System;
using System.Collections;
using UnityEngine;

public class PassiveSkill
{
    public PassiveSkillData passiveData;
    int passiveIndex;
    bool onCooldown;
    Action unsubscribe;

    public PassiveSkill(PassiveSkillData data, int index)
    {
        passiveData = data;
        passiveIndex = index;
    }

    public virtual void StartPassive(StateMachine owner)
    {
        Debug.Log($"[PlayerPassive] Starting {passiveData.Name} on {owner.name}");

        if (passiveData.Trigger != null)
        {
            unsubscribe = passiveData.Trigger.Subscribe(owner, () => TryActivate(owner));
        }
    }

    public virtual void UpdatePassive(StateMachine owner)
    {

    }
    public virtual void FixedUpdatePassive(StateMachine owner)
    {

    }

    public virtual void EndPassive(StateMachine owner)
    {
        Debug.Log($"[PlayerPassive] Ending {passiveData.Name} on {owner.name}");
        unsubscribe?.Invoke();
        unsubscribe = null;
    }

    protected void TryActivate(StateMachine owner)
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

    IEnumerator CooldownRoutine(StateMachine owner)
    {
        onCooldown = true;
        yield return new WaitForSeconds(passiveData.CoolDown);
        onCooldown = false;
        Debug.Log($"[PlayerPassive] {passiveData.Name} cooldown finished");
    }

    SkillContext BuildContext(StateMachine owner) => new SkillContext
    {
        Attacker = owner.NetworkObject,
        AttackerDamage = owner.Stats.TotalDamage,
    };
}
