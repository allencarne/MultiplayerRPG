using Unity.Netcode;
using UnityEngine;

public class NutQuake : EnemyAbility
{
    public override void AbilityStart(EnemyStateMachine owner)
    {
        InitializeAbility(skillType, owner);
        owner.EnemyRB.linearVelocity = Vector2.zero;
        ModifiedCastTime = CastTime / owner.enemy.CurrentAttackSpeed;
        SpawnPosition = owner.transform.position;

        // Aim
        AimDirection = (owner.Target.position - transform.position).normalized;

        ChangeState(State.Cast, ModifiedCastTime);
        CastState(owner);
    }

    public override void AbilityUpdate(EnemyStateMachine owner)
    {
        if (currentState == State.Done) return;

        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0f) HandleStateTransition(owner);
    }

    public override void AbilityFixedUpdate(EnemyStateMachine owner)
    {

    }

    void HandleStateTransition(EnemyStateMachine owner)
    {
        switch (currentState)
        {
            case State.Cast:
                ActionState(owner);
                ChangeState(State.Action, ActionTime);
                break;
            case State.Action:
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
        owner.EnemyAnimator.Play("Ultimate Cast");
        owner.EnemyAnimator.SetFloat("Horizontal", AimDirection.x);
        owner.EnemyAnimator.SetFloat("Vertical", AimDirection.y);

        owner.enemy.CastBar.StartCast(castTime: CastTime, owner.enemy.CurrentAttackSpeed);
        Telegraph(false, false);
    }

    void ActionState(EnemyStateMachine owner)
    {
        owner.EnemyAnimator.Play("Ultimate Action");
    }

    void ImpactState(EnemyStateMachine owner)
    {
        owner.EnemyAnimator.Play("Ultimate Impact");
        Attack(owner.NetworkObject, false, false);
    }

    void RecoveryState(EnemyStateMachine owner)
    {
        owner.EnemyAnimator.Play("Ultimate Recovery");
        owner.enemy.CastBar.StartRecovery(RecoveryTime, owner.enemy.CurrentAttackSpeed);
    }
}
