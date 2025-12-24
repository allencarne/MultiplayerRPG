using System.Collections;
using UnityEngine;

public class SecondWind : PlayerSkill
{
    bool isOnCooldown = false;

    //int healAmount = 1;
    //float healRate = 1f;
    //float abilityDuration = 5f;

    public override void UpdateSkill(PlayerStateMachine owner)
    {
        if (!owner.IsOwner) return;

        float healthPercent = owner.Stats.net_CurrentHP.Value / owner.Stats.net_BaseHP.Value;

        if (!isOnCooldown && healthPercent <= 0.4f)
        {
            isOnCooldown = true;

            //owner.Buffs.regeneration.Regeneration(HealType.Flat, healAmount, healRate, abilityDuration);
            owner.StartCoroutine(CoolDownTime());
        }
    }

    private IEnumerator CoolDownTime()
    {
        yield return new WaitForSeconds(CoolDown);
        isOnCooldown = false;
    }
}
