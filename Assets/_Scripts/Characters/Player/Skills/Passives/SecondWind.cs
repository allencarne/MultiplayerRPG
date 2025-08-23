using System.Collections;
using UnityEngine;

public class SecondWind : PlayerAbility
{
    bool isOnCooldown = false;

    int healAmount = 1;
    float healRate = 1f;
    float abilityDuration = 5f;

    public override void StartAbility(PlayerStateMachine owner)
    {

    }

    public override void UpdateAbility(PlayerStateMachine owner)
    {
        if (!owner.IsOwner) return;

        float healthPercent = owner.player.Health.Value / owner.player.MaxHealth.Value;

        if (!isOnCooldown && healthPercent <= 0.4f)
        {
            isOnCooldown = true;

            owner.Buffs.regeneration.Regeneration(HealType.Flat, healAmount, healRate, abilityDuration);
            owner.StartCoroutine(CoolDownTime());
        }
    }

    public override void FixedUpdateAbility(PlayerStateMachine owner)
    {

    }

    private IEnumerator CoolDownTime()
    {
        yield return new WaitForSeconds(CoolDown);
        isOnCooldown = false;
    }
}
