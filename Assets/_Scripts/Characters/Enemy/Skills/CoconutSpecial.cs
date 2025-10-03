using UnityEngine;

public class CoconutSpecial : EnemyAbility
{
    public override void AbilityStart(EnemyStateMachine owner)
    {
        owner.CanSpecial = false;
        owner.IsAttacking = true;
        owner.currentAbility = this;
        skillType = SkillType.Special;

        float modifiedCastTime = CastTime / owner.enemy.CurrentAttackSpeed;

        ChangeState(State.Cast, modifiedCastTime);
        CastState(owner);
    }

    public override void AbilityUpdate(EnemyStateMachine owner)
    {
        if (currentState == State.Done) return;

        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0f)
        {
            HandleStateTransition(owner);
        }
    }

    public override void AbilityFixedUpdate(EnemyStateMachine owner)
    {

    }

    private void HandleStateTransition(EnemyStateMachine owner)
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
                DoneState(false, owner);
                break;
        }
    }

    void CastState(EnemyStateMachine owner)
    {
        owner.EnemyAnimator.Play("Basic Cast");
        owner.enemy.CastBar.StartCast(CastTime, owner.enemy.CurrentAttackSpeed);
    }

    void ImpactState(EnemyStateMachine owner)
    {
        // Animate Impact
        owner.EnemyAnimator.Play("Basic Impact");
    }

    void RecoveryState(EnemyStateMachine owner)
    {
        // Animate Recovery
        owner.EnemyAnimator.Play("Basic Recovery");
        owner.enemy.CastBar.StartRecovery(RecoveryTime, owner.enemy.CurrentAttackSpeed);
    }
}
