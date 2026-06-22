using UnityEngine;

public class SwineSlam : EnemySkill
{
    public override void StartSkill(EnemyStateMachine owner)
    {
        InitializeAbility(skillData.skillType, owner);

        // Aim
        AimDirection = (owner.Target.position - transform.position).normalized;

        ChangeState(State.Cast, skillData.CastTime);
        CastState(owner);
    }

    public override void CastState(EnemyStateMachine owner)
    {
        Animate(owner, skillData.skillType, State.Cast);
        owner.EnemyAnimator.SetFloat("Horizontal", AimDirection.x);
        owner.EnemyAnimator.SetFloat("Vertical", AimDirection.y);

        owner.enemy.CastBar.StartCast(skillData.CastTime);
        Telegraph(skillData.CastTime, false, false);
    }

    public override void ImpactState(EnemyStateMachine owner)
    {
        Animate(owner, skillData.skillType, State.Impact);
        Attack(owner.NetworkObject, false, false);
    }

    public override void RecoveryState(EnemyStateMachine owner)
    {
        Animate(owner, skillData.skillType, State.Recovery);
        owner.enemy.CastBar.StartRecovery(skillData.RecoveryTime);
    }
}
