using System.Collections;
using UnityEngine;

public class DiveBite : EnemySkill
{
    Vector2 targetLandingPos;

    public override void StartSkill(EnemyStateMachine owner)
    {
        InitializeAbility(skillData.skillType, owner);

        // Clamp landing position to skill range
        Vector2 targetPos = owner.Target.position;
        Vector2 direction = (targetPos - SpawnPosition).normalized;
        float clampedDistance = Mathf.Min(Vector2.Distance(targetPos, SpawnPosition), skillData.SkillRange);

        targetLandingPos = SpawnPosition + direction * clampedDistance;
        AimDirection = direction;
        AimOffset = targetLandingPos - SpawnPosition; // Used by Telegraph/Attack with useOffset = true
        AimRotation = Quaternion.Euler(0, 0, Mathf.Atan2(AimDirection.y, AimDirection.x) * Mathf.Rad2Deg);

        ChangeState(State.Cast, skillData.CastTime);
        CastState(owner);
    }

    public override void CastState(EnemyStateMachine owner)
    {
        Animate(owner, skillData.skillType, State.Cast);
        owner.EnemyAnimator.SetFloat("Horizontal", AimDirection.x);
        owner.EnemyAnimator.SetFloat("Vertical", AimDirection.y);
        owner.enemy.CastBar.StartCast(skillData.CastTime);
        Telegraph(skillData.CastTime, false, true);
    }

    public override void ActionState(EnemyStateMachine owner)
    {
        owner.Buffs.immoveable.StartImmovable(skillData.ActionTime);
        owner.Buffs.immune.StartImmune(skillData.ActionTime);

        Animate(owner, skillData.skillType, State.Action);
        Attack(owner.NetworkObject, false, false);
        Telegraph(skillData.ActionTime, true, false);

        StartCoroutine(delay(owner));
    }
    
    IEnumerator delay(EnemyStateMachine owner)
    {
        yield return new WaitForSeconds(.33f);

        owner.EnemyRB.linearVelocity = Vector2.zero;
        owner.EnemyRB.position = targetLandingPos;
    }

    public override void ImpactState(EnemyStateMachine owner)
    {
        Animate(owner, skillData.skillType, State.Impact);
        Attack(owner.NetworkObject, true, false);
    }

    public override void RecoveryState(EnemyStateMachine owner)
    {
        Animate(owner, skillData.skillType, State.Recovery);
        owner.enemy.CastBar.StartRecovery(skillData.RecoveryTime);
    }
}
