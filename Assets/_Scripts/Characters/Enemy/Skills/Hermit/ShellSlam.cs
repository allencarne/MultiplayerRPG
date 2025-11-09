using UnityEngine;

public class ShellSlam : EnemySkill
{
    float dashTimer;
    Vector2 targetLandingPos;

    public override void StartSkill(EnemyStateMachine owner)
    {
        InitializeAbility(skillType, owner);

        // Aim
        Vector2 targetPos = owner.Target.position;
        Vector2 direction = (targetPos - SpawnPosition).normalized;
        float distance = Vector2.Distance(targetPos, SpawnPosition);
        float clampedDistance = Mathf.Min(distance, SkillRange);
        targetLandingPos = SpawnPosition + direction * clampedDistance;
        AimDirection = direction;
        AimOffset = targetLandingPos - SpawnPosition;
        float angle = Mathf.Atan2(AimDirection.y, AimDirection.x) * Mathf.Rad2Deg;
        AimRotation = Quaternion.Euler(0, 0, angle);

        ChangeState(State.Cast, CastTime);
        CastState(owner);
    }

    public override void FixedUpdateSkill(EnemyStateMachine owner)
    {
        if (currentState == State.Action)
        {
            dashTimer += Time.fixedDeltaTime;
            float t = Mathf.Clamp01(dashTimer / ActionTime);

            Vector2 newPos = Vector2.Lerp(SpawnPosition, targetLandingPos, t);
            owner.EnemyRB.MovePosition(newPos);

            if (t >= 1f)
            {
                owner.EnemyRB.linearVelocity = Vector2.zero;
            }
        }
    }

    public override void CastState(EnemyStateMachine owner)
    {
        Animate(owner, skillType, State.Cast);
        owner.EnemyAnimator.SetFloat("Horizontal", AimDirection.x);
        owner.EnemyAnimator.SetFloat("Vertical", AimDirection.y);

        owner.enemy.CastBar.StartCast(CastTime);
        Telegraph(CastTime + ActionTime, true, true);
    }

    public override void ActionState(EnemyStateMachine owner)
    {
        dashTimer = 0f;
        owner.Buffs.phase.StartPhase(ActionTime);
        owner.Buffs.immoveable.StartImmovable(ActionTime);

        Animate(owner, skillType, State.Action);
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
