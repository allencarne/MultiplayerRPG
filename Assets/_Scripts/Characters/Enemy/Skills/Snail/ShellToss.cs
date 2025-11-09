using UnityEngine;

public class ShellToss : EnemySkill
{
    public override void StartSkill(EnemyStateMachine owner)
    {
        InitializeAbility(skillType, owner);

        // Aim
        AimDirection = (owner.Target.position - transform.position).normalized;
        float angle = Mathf.Atan2(AimDirection.y, AimDirection.x) * Mathf.Rad2Deg;
        AimRotation = Quaternion.Euler(0, 0, angle);
        AimOffset = AimDirection.normalized * AttackRange_;

        ChangeState(State.Cast, CastTime);
        CastState(owner);
    }

    public override void CastState(EnemyStateMachine owner)
    {
        Animate(owner, skillType, State.Cast);
        owner.EnemyAnimator.SetFloat("Horizontal", AimDirection.x);
        owner.EnemyAnimator.SetFloat("Vertical", AimDirection.y);

        owner.enemy.CastBar.StartCast(CastTime);
        Telegraph(CastTime, true, true);
    }

    public override void ImpactState(EnemyStateMachine owner)
    {
        Animate(owner, skillType, State.Impact);
        Attack(owner.NetworkObject, true, true);
    }

    public override void RecoveryState(EnemyStateMachine owner)
    {
        Animate(owner, skillType, State.Recovery);
        owner.enemy.CastBar.StartRecovery(RecoveryTime);
    }
}