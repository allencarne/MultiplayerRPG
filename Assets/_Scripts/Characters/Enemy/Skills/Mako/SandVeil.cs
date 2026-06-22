using UnityEngine;
using UnityEngine.UIElements;

public class SandVeil : EnemySkill
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

        owner.Buffs.haste.StartHaste(2,7);
        owner.Buffs.swiftness.StartSwiftness(2,7);
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
