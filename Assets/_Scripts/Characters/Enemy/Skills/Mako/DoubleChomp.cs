using System.Collections;
using UnityEngine;

public class DoubleChomp : EnemySkill
{
    Coroutine attack;

    public override void StartSkill(EnemyStateMachine owner)
    {
        InitializeAbility(skillType, owner);

        // Basic
        ModifiedCastTime = CastTime / owner.enemy.stats.TotalAS;

        // Aim
        AimDirection = (owner.Target.position - transform.position).normalized;
        float angle = Mathf.Atan2(AimDirection.y, AimDirection.x) * Mathf.Rad2Deg;
        AimRotation = Quaternion.Euler(0, 0, angle);
        AimOffset = AimDirection.normalized * SkillRange;

        ChangeState(State.Cast, ModifiedCastTime);
        CastState(owner);
    }

    public override void CastState(EnemyStateMachine owner)
    {
        Animate(owner, skillType, State.Cast);
        owner.EnemyAnimator.SetFloat("Horizontal", AimDirection.x);
        owner.EnemyAnimator.SetFloat("Vertical", AimDirection.y);

        owner.enemy.CastBar.StartCast(ModifiedCastTime);
        Telegraph(ModifiedCastTime, true, false);
    }

    public override void ImpactState(EnemyStateMachine owner)
    {
        Animate(owner, skillType, State.Impact);
        Attack(owner.NetworkObject, true, false);
        attack = StartCoroutine(AttackPattern(owner));
    }

    public override void RecoveryState(EnemyStateMachine owner)
    {
        attack = null;
        Animate(owner, skillType, State.Recovery);
        owner.enemy.CastBar.StartRecovery(RecoveryTime);
    }

    public override void UpdateSkill(EnemyStateMachine owner)
    {
        base.UpdateSkill(owner);

        if (owner.enemy.stats.isDead)
        {
            if (attack != null)
            {
                StopCoroutine(attack);
                attack = null;
            }
        }
    }

    IEnumerator AttackPattern(EnemyStateMachine owner)
    {
        Attack(owner.NetworkObject, true, true);
        yield return new WaitForSeconds(.3f);

        Attack(owner.NetworkObject, true, true);
    }
}
