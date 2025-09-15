using UnityEngine;

public class CoconutSpecial : EnemyAbility
{
    enum State { Cast, Impact, Recovery, Done }
    State currentState;

    [Header("Time")]
    [SerializeField] float castTime;
    [SerializeField] float impactTime;
    [SerializeField] float recoveryTime;
    [SerializeField] float coolDown;
    float stateTimer;

    public override void AbilityStart(EnemyStateMachine owner)
    {
        owner.CanSpecial = false;
        owner.IsAttacking = true;

        float modifiedCastTime = castTime / owner.enemy.CurrentAttackSpeed;
        ChangeState(State.Cast, modifiedCastTime);
        CastState(owner);
    }

    public override void AbilityUpdate(EnemyStateMachine owner)
    {
        Debug.Log(currentState);

        if (currentState == State.Done) return;
        if (!owner.IsAttacking) return;

        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0f)
        {
            HandleStateTransition(owner);
        }
    }

    public override void AbilityFixedUpdate(EnemyStateMachine owner)
    {

    }

    private void ChangeState(State nextState, float duration)
    {
        currentState = nextState;
        stateTimer = duration;
    }

    private void HandleStateTransition(EnemyStateMachine owner)
    {
        switch (currentState)
        {
            case State.Cast:
                ImpactState(owner);
                ChangeState(State.Impact, impactTime);
                break;
            case State.Impact:
                RecoveryState(owner);
                ChangeState(State.Recovery, recoveryTime);
                break;
            case State.Recovery:
                DoneState(owner);
                break;
        }
    }

    void CastState(EnemyStateMachine owner)
    {
        owner.EnemyAnimator.Play("Basic Cast");
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
    }

    void DoneState(EnemyStateMachine owner)
    {
        currentState = State.Done;
        owner.IsAttacking = false;

        StartCoroutine(owner.CoolDownTime(EnemyStateMachine.SkillType.Special, coolDown));
        owner.SetState(EnemyStateMachine.State.Idle);
    }
}
