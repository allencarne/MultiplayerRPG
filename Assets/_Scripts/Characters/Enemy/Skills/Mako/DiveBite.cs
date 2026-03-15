using System.Collections;
using UnityEngine;

public class DiveBite : EnemySkill
{
    Vector2 targetLandingPos;

    public override void StartSkill(EnemyStateMachine owner)
    {
        InitializeAbility(skillType, owner);

        // Clamp landing position to skill range
        Vector2 targetPos = owner.Target.position;
        Vector2 direction = (targetPos - SpawnPosition).normalized;
        float clampedDistance = Mathf.Min(Vector2.Distance(targetPos, SpawnPosition), SkillRange);

        targetLandingPos = SpawnPosition + direction * clampedDistance;
        AimDirection = direction;
        AimOffset = targetLandingPos - SpawnPosition; // Used by Telegraph/Attack with useOffset = true
        AimRotation = Quaternion.Euler(0, 0, Mathf.Atan2(AimDirection.y, AimDirection.x) * Mathf.Rad2Deg);

        ChangeState(State.Cast, CastTime);
        CastState(owner);
    }

    public override void CastState(EnemyStateMachine owner)
    {
        Animate(owner, skillType, State.Cast);
        owner.EnemyAnimator.SetFloat("Horizontal", AimDirection.x);
        owner.EnemyAnimator.SetFloat("Vertical", AimDirection.y);
        owner.enemy.CastBar.StartCast(CastTime);
        Telegraph(CastTime, false, true);
    }

    public override void ActionState(EnemyStateMachine owner)
    {
        Animate(owner, skillType, State.Action);
        Attack(owner.NetworkObject, false, false);
        owner.Buffs.immoveable.StartImmovable(ActionTime);
        Telegraph(ActionTime, true, false);

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
        Animate(owner, skillType, State.Impact);
        Attack(owner.NetworkObject, true, false);
    }

    public override void RecoveryState(EnemyStateMachine owner)
    {
        Animate(owner, skillType, State.Recovery);
        owner.enemy.CastBar.StartRecovery(RecoveryTime);
    }
}
