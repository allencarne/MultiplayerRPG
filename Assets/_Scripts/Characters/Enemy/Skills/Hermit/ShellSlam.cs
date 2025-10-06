using UnityEngine;

public class ShellSlam : EnemyAbility
{
    [Header("Slide")]
    [SerializeField] float slideForce;
    [SerializeField] float buffDuration;

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
        if (stateTimer <= 0f) StateTransition(owner, true);
    }

    public override void AbilityFixedUpdate(EnemyStateMachine owner)
    {
        if (currentState == State.Impact)
        {
            owner.EnemyRB.linearVelocity = AimDirection * slideForce;
        }
    }

    public override void CastState(EnemyStateMachine owner)
    {
        AnimateEnemy(owner, skillType, State.Cast);
        owner.EnemyAnimator.SetFloat("Horizontal", AimDirection.x);
        owner.EnemyAnimator.SetFloat("Vertical", AimDirection.y);

        owner.enemy.CastBar.StartCast(castTime: CastTime, owner.enemy.CurrentAttackSpeed);
        Telegraph(false, false);
    }

    public override void ActionState(EnemyStateMachine owner)
    {
        owner.Buffs.phase.StartPhase(buffDuration);
        owner.Buffs.immoveable.StartImmovable(buffDuration);

        //AnimateEnemy(owner, skillType, State.Action);
    }

    public override void ImpactState(EnemyStateMachine owner)
    {
        AnimateEnemy(owner, skillType, State.Impact);
        Attack(owner.NetworkObject, false, false);
    }

    public override void RecoveryState(EnemyStateMachine owner)
    {
        AnimateEnemy(owner, skillType, State.Recovery);
        owner.enemy.CastBar.StartRecovery(RecoveryTime, owner.enemy.CurrentAttackSpeed);
    }
}
