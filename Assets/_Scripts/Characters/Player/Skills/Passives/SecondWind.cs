using System.Collections;
using UnityEngine;

public class SecondWind : ActiveSkill
{
    bool isOnCooldown = false;

    public SecondWind(ActiveSkillData data, int index) : base(data, index)
    {
    }

    public override void UpdateSkill(StateMachine owner)
    {
        if (!owner.IsOwner) return;

        float healthPercent = owner.Stats.net_CurrentHP.Value / owner.Stats.net_BaseHP.Value;

        if (!isOnCooldown && healthPercent <= 0.4f)
        {
            isOnCooldown = true;

            owner.Buffs.regeneration.StartRegen(5,5);
            owner.StartCoroutine(CoolDownTime());
        }
    }

    private IEnumerator CoolDownTime()
    {
        yield return new WaitForSeconds(skillData.CoolDown);
        isOnCooldown = false;
    }
}
