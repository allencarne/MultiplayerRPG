using System.Collections;
using UnityEngine;

public class PlayerPassive
{
    public PassiveSkillData passiveData;
    int passiveIndex;
    bool onCooldown;

    public PlayerPassive(PassiveSkillData data, int index)
    {
        passiveData = data;
        passiveIndex = index;
    }

    public virtual void StartPassive(PlayerStateMachine owner)
    {

    }

    public virtual void UpdatePassive(PlayerStateMachine owner)
    {

    }
    public virtual void FixedUpdatePassive(PlayerStateMachine owner)
    {

    }

    public virtual void EndPassive(PlayerStateMachine owner)
    {

    }

    protected void TryActivate(PlayerStateMachine owner)
    {
        if (onCooldown) return;
        if (passiveData.OnActivateEffects == null) return;

        foreach (SkillEffect effect in passiveData.OnActivateEffects)
        {
            SkillContext ctx = BuildContext(owner);
            effect.Execute(owner, ctx);
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
    }

    SkillContext BuildContext(PlayerStateMachine owner) => new SkillContext
    {
        AttackerId = owner.OwnerClientId,
        AttackerDamage = owner.Stats.TotalDamage,
    };
}
