using UnityEngine;

public class SwineSlam : EnemySkill
{
    public override void StartSkill(EnemyStateMachine owner)
    {
        InitializeAbility(skillType, owner);

        // Aim
        AimDirection = (owner.Target.position - transform.position).normalized;

        ChangeState(State.Cast, CastTime);
        CastState(owner);
    }

    public override void CastState(EnemyStateMachine owner)
    {
        Animate(owner, skillType, State.Cast);
        owner.EnemyAnimator.SetFloat("Horizontal", AimDirection.x);
        owner.EnemyAnimator.SetFloat("Vertical", AimDirection.y);

        owner.enemy.CastBar.StartCast(CastTime);
        Telegraph(CastTime, false, false);
    }

    public override void ImpactState(EnemyStateMachine owner)
    {
        Animate(owner, skillType, State.Impact);
        Attack(owner.NetworkObject, false, false);
    }

    public override void RecoveryState(EnemyStateMachine owner)
    {
        Animate(owner, skillType, State.Recovery);
        owner.enemy.CastBar.StartRecovery(RecoveryTime);
    }
}
