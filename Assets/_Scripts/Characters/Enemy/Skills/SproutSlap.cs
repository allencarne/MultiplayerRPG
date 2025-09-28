using Unity.Netcode;
using UnityEngine;

public class SproutSlap : EnemyAbility
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

    void StateTransition(EnemyStateMachine owner)
    {
        switch (currentState)
        {
            case State.Cast:
                ImpactState(owner);
                ChangeState(State.Impact, ImpactTime);
                break;
            case State.Impact:
                RecoveryState(owner);
                ChangeState(State.Recovery, RecoveryTime);
                break;
            case State.Recovery:
                DoneState(owner);
                break;
        }
    }

    void CastState(EnemyStateMachine owner)
    {
        owner.EnemyAnimator.Play("Basic Cast");
        owner.EnemyAnimator.SetFloat("Horizontal", AimDirection.x);
        owner.EnemyAnimator.SetFloat("Vertical", AimDirection.y);

        owner.enemy.CastBar.StartCast(CastTime, owner.enemy.CurrentAttackSpeed);
        Telegraph(true, false);
    }

    void ImpactState(EnemyStateMachine owner)
    {
        owner.EnemyAnimator.Play("Basic Impact");
        Attack(owner.NetworkObject, true, false);
    }

    void RecoveryState(EnemyStateMachine owner)
    {
        owner.EnemyAnimator.Play("Basic Recovery");
        owner.enemy.CastBar.StartRecovery(RecoveryTime, owner.enemy.CurrentAttackSpeed);
    }
}