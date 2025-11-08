using UnityEngine;

public class ShellToss : EnemySkill
{
    public override void StartSkill(EnemyStateMachine owner)
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

    public override void UpdateSkill(EnemyStateMachine owner)
    {
        if (currentState == State.Done) return;

        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0f) StateTransition(owner);
    }

    public override void FixedUpdateSkill(EnemyStateMachine owner)
    {

    }

    public override void CastState(EnemyStateMachine owner)
    {
        Animate(owner, skillType, State.Cast);
        owner.EnemyAnimator.SetFloat("Horizontal", AimDirection.x);
        owner.EnemyAnimator.SetFloat("Vertical", AimDirection.y);

        owner.enemy.CastBar.StartCast(CastTime, owner.enemy.CurrentAttackSpeed);
        Telegraph(true, true);
    }

    public override void ImpactState(EnemyStateMachine owner)
    {
        Animate(owner, skillType, State.Impact);
        Attack(owner.NetworkObject, true, true);
    }

    public override void RecoveryState(EnemyStateMachine owner)
    {
        Animate(owner, skillType, State.Recovery);
        owner.enemy.CastBar.StartRecovery(RecoveryTime, owner.enemy.CurrentAttackSpeed);
    }
}