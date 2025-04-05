using UnityEngine;

public class SnailBash : EnemyAbility
{
    float castTime;
    float recoveryTime;
    float cooldown;

    public override void AbilityStart()
    {
        Debug.Log("Ability Start");
    }

    public override void AbilityUpdate()
    {
        Debug.Log("Ability Update");
    }

    public override void AbilityFixedUpdate()
    {
        Debug.Log("Ability FixedUpdate");
    }
}
