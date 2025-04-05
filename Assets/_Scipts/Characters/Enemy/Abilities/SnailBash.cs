using UnityEngine;

public class SnailBash : EnemyAbility
{
    float castTime;
    float recoveryTime;
    float cooldown;

    public override void AbilityStart(EnemyStateMachine owner)
    {
        Debug.Log("Ability Start");
    }

    public override void AbilityUpdate(EnemyStateMachine owner)
    {
        Debug.Log("Ability Update");
    }

    public override void AbilityFixedUpdate(EnemyStateMachine owner)
    {
        Debug.Log("Ability FixedUpdate");
    }
}
