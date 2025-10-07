using UnityEngine;

public class ShellSlam : EnemyAbility
{
    float dashTimer;
    Vector2 targetLandingPos;

    public override void AbilityStart(EnemyStateMachine owner)
    {
        InitializeAbility(skillType, owner);
        owner.EnemyRB.linearVelocity = Vector2.zero;
        ModifiedCastTime = CastTime / owner.enemy.CurrentAttackSpeed;
        SpawnPosition = owner.transform.position;

        // Aim
        Vector2 targetPos = owner.Target.position;
        Vector2 direction = (targetPos - SpawnPosition).normalized;
        float distance = Vector2.Distance(targetPos, SpawnPosition);
        float clampedDistance = Mathf.Min(distance, AttackRange_);
        targetLandingPos = SpawnPosition + direction * clampedDistance;
        AimDirection = direction;
        AimOffset = targetLandingPos - SpawnPosition;
        float angle = Mathf.Atan2(AimDirection.y, AimDirection.x) * Mathf.Rad2Deg;
        AimRotation = Quaternion.Euler(0, 0, angle);

        ChangeState(State.Cast, ModifiedCastTime);
        CastState(owner);
    }

    public override void AbilityUpdate(EnemyStateMachine owner)
    {
        if (currentState == State.Done) return;

        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0f) StateTransition(owner, true);
    }

    public override void AbilityFixedUpdate(EnemyStateMachine owner)
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
        AnimateEnemy(owner, skillType, State.Cast);
        owner.EnemyAnimator.SetFloat("Horizontal", AimDirection.x);
        owner.EnemyAnimator.SetFloat("Vertical", AimDirection.y);

        owner.enemy.CastBar.StartCast(ModifiedCastTime, owner.enemy.CurrentAttackSpeed);
        Telegraph(true, true);
    }

    public override void ActionState(EnemyStateMachine owner)
    {
        dashTimer = 0f;
        owner.Buffs.phase.StartPhase(2);
        owner.Buffs.immoveable.StartImmovable(2);

        //AnimateEnemy(owner, skillType, State.Action);
    }

    public override void ImpactState(EnemyStateMachine owner)
    {
        AnimateEnemy(owner, skillType, State.Impact);
        Attack(owner.NetworkObject, true, false);
    }

    public override void RecoveryState(EnemyStateMachine owner)
    {
        AnimateEnemy(owner, skillType, State.Recovery);
        owner.enemy.CastBar.StartRecovery(RecoveryTime, owner.enemy.CurrentAttackSpeed);
    }
}
