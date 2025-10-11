using UnityEngine;

public class ClawBarrage : EnemyAbility
{
    public override void AbilityStart(EnemyStateMachine owner)
    {
        InitializeAbility(skillType, owner);
        owner.EnemyRB.linearVelocity = Vector2.zero;
        ModifiedCastTime = CastTime / owner.enemy.CurrentAttackSpeed;
        SpawnPosition = owner.transform.position;

        // Aim
        AimDirection = (owner.Target.position - transform.position).normalized;
        float angle = Mathf.Atan2(AimDirection.y, AimDirection.x) * Mathf.Rad2Deg;
        AimRotation = Quaternion.Euler(0, 0, angle);
        AimOffset = AimDirection.normalized * AttackRange_;

        ChangeState(State.Cast, ModifiedCastTime);
        CastState(owner);
    }

    public override void AbilityUpdate(EnemyStateMachine owner)
    {
        if (currentState == State.Done) return;

        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0f) StateTransition(owner);
    }

    public override void AbilityFixedUpdate(EnemyStateMachine owner)
    {

    }

    public override void CastState(EnemyStateMachine owner)
    {
        AnimateEnemy(owner, skillType, State.Cast);
        owner.EnemyAnimator.SetFloat("Horizontal", AimDirection.x);
        owner.EnemyAnimator.SetFloat("Vertical", AimDirection.y);

        owner.enemy.CastBar.StartCast(CastTime, owner.enemy.CurrentAttackSpeed);
        Telegraph(true, false);
    }

    public override void ImpactState(EnemyStateMachine owner)
    {
        owner.Buffs.phase.StartPhase(ActionTime);
        owner.Buffs.protection.StartProtection(2, 5);

        AnimateEnemy(owner, skillType, State.Impact);
        Attack(owner.NetworkObject, true, false);
    }

    public override void RecoveryState(EnemyStateMachine owner)
    {
        AnimateEnemy(owner, skillType, State.Recovery);
        owner.enemy.CastBar.StartRecovery(RecoveryTime, owner.enemy.CurrentAttackSpeed);
    }
}
