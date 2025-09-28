using Unity.Netcode;
using UnityEngine;

public class Tumble : EnemyAbility
{
    [Header("Slide")]
    [SerializeField] float slideForce;

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
        if (currentState == State.Action)
        {
            owner.EnemyRB.linearVelocity = AimDirection * slideForce;
        }
    }

    void StateTransition(EnemyStateMachine owner)
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
        owner.EnemyAnimator.Play("Special Cast");
        owner.EnemyAnimator.SetFloat("Horizontal", AimDirection.x);
        owner.EnemyAnimator.SetFloat("Vertical", AimDirection.y);

        owner.enemy.CastBar.StartCast(CastTime, owner.enemy.CurrentAttackSpeed);
        Telegraph(true, true);
    }

    void ActionState(EnemyStateMachine owner)
    {
        owner.EnemyAnimator.Play("Special Impact");
    }

    void ImpactState(EnemyStateMachine owner)
    {
        owner.EnemyAnimator.Play("Special Impact");
        Attack(owner.NetworkObject, true, true);
    }

    void RecoveryState(EnemyStateMachine owner)
    {
        owner.EnemyAnimator.Play("Special Recovery");
        owner.enemy.CastBar.StartRecovery(RecoveryTime, owner.enemy.CurrentAttackSpeed);
    }
}
