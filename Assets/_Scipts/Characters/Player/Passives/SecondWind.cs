using System.Collections;
using UnityEngine;

public class SecondWind : PlayerAbility
{
    bool isOnCooldown = false;
    bool isHealing = false;

    int healAmount = 1;
    float healRate = 1f;
    float abilityDuration = 5f;
    float cooldown = 30f;

    public override void StartAbility(PlayerStateMachine owner)
    {

    }

    public override void UpdateAbility(PlayerStateMachine owner)
    {
        float healthPercent = owner.player.Health.Value / owner.player.MaxHealth.Value;

        if (!isHealing && !isOnCooldown && healthPercent <= 0.4f)
        {
            isHealing = true;
            owner.StartCoroutine(HealOverTime(owner));
        }
    }

    public override void FixedUpdateAbility(PlayerStateMachine owner)
    {

    }

    private IEnumerator HealOverTime(PlayerStateMachine owner)
    {
        float elapsed = 0f;

        while (elapsed < abilityDuration)
        {
            // Heal the player once per tick
            owner.player.GiveHeal(healAmount, HealType.Flat);

            yield return new WaitForSeconds(healRate);
            elapsed += healRate;
        }

        isHealing = false;
        isOnCooldown = true;

        // Start cooldown timer
        owner.StartCoroutine(CoolDownTime());
    }

    private IEnumerator CoolDownTime()
    {
        yield return new WaitForSeconds(cooldown);
        isOnCooldown = false;
    }
}
