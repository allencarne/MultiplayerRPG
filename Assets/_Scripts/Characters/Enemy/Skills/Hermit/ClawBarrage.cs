using UnityEngine;

public class ClawBarrage : EnemySkill
{
    public override void StartSkill(EnemyStateMachine owner)
    {
        InitializeAbility(skillData.skillType, owner);

        // Aim
        AimDirection = (owner.Target.position - transform.position).normalized;
        float angle = Mathf.Atan2(AimDirection.y, AimDirection.x) * Mathf.Rad2Deg;
        AimRotation = Quaternion.Euler(0, 0, angle);
        AimOffset = AimDirection.normalized * skillData.SkillRange;

        ChangeState(State.Cast, skillData.CastTime);
        CastState(owner);
    }

    public override void CastState(EnemyStateMachine owner)
    {
        Animate(owner, skillData.skillType, State.Cast);
        owner.EnemyAnimator.SetFloat("Horizontal", AimDirection.x);
        owner.EnemyAnimator.SetFloat("Vertical", AimDirection.y);

        owner.enemy.CastBar.StartCast(skillData.CastTime);
        Telegraph(skillData.CastTime, true, false);
    }

    public override void ImpactState(EnemyStateMachine owner)
    {
        owner.Buffs.immoveable.StartImmovable(skillData.ImpactTime);
        //owner.Buffs.phase.StartPhase(ImpactTime);
        owner.Buffs.protection.StartProtection(2, 5);

        Animate(owner, skillData.skillType, State.Impact);
        Attack(owner.NetworkObject, true, false);
    }

    public override void RecoveryState(EnemyStateMachine owner)
    {
        Animate(owner, skillData.skillType, State.Recovery);
        owner.enemy.CastBar.StartRecovery(skillData.RecoveryTime);
    }
}
