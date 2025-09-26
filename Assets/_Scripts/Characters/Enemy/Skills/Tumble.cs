using Unity.Netcode;
using UnityEngine;

public class Tumble : EnemyAbility
{
    [SerializeField] float actionTime;

    [Header("Knockback")]
    [SerializeField] float knockBackAmount;
    [SerializeField] float knockBackDuration;
    float modifiedCastTime;
    Vector2 spawnPosition;
    Vector2 aimDirection;
    Quaternion aimRotation;

    [Header("Slide")]
    [SerializeField] float slideForce;

    public override void AbilityStart(EnemyStateMachine owner)
    {
        owner.CanSpecial = false;
        owner.IsAttacking = true;
        owner.currentAbility = this;

        // Stop
        owner.EnemyRB.linearVelocity = Vector2.zero;

        // Variables
        modifiedCastTime = CastTime / owner.enemy.CurrentAttackSpeed;
        spawnPosition = owner.transform.position;

        // Direction
        aimDirection = (owner.Target.position - transform.position).normalized;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        aimRotation = Quaternion.Euler(0, 0, angle);

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
        if (currentState == State.Action)
        {
            owner.EnemyRB.linearVelocity = aimDirection * slideForce;
        }
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
                ChangeState(State.Action, actionTime);
                break;
            case State.Action:
                ActionState(owner);
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
        owner.enemy.CastBar.StartCast(CastTime, owner.enemy.CurrentAttackSpeed);

        SpawnTelegraph(spawnPosition, aimRotation, modifiedCastTime);
    }

    void ActionState(EnemyStateMachine owner)
    {
        // Animate Action
        owner.EnemyAnimator.Play("Special Impact");
    }

    void ImpactState(EnemyStateMachine owner)
    {
        // Animate Impact
        owner.EnemyAnimator.Play("Special Impact");

        SpawnAttack(spawnPosition, aimRotation, owner.NetworkObject);
    }

    void RecoveryState(EnemyStateMachine owner)
    {
        // Animate Recovery
        owner.EnemyAnimator.Play("Special Recovery");
        owner.enemy.CastBar.StartRecovery(RecoveryTime, owner.enemy.CurrentAttackSpeed);
    }

    void DoneState(EnemyStateMachine owner)
    {
        StartCoroutine(owner.CoolDown(skillType, CoolDown));
        currentState = State.Done;
        owner.IsAttacking = false;
        owner.currentAbility = null;
        owner.SetState(EnemyStateMachine.State.Idle);
    }

    void SpawnTelegraph(Vector2 spawnPosition, Quaternion spawnRotation, float modifiedCastTime)
    {
        Vector2 offset = aimDirection.normalized * AttackRange_;

        GameObject attackInstance = Instantiate(TelegraphPrefab_, spawnPosition + offset, spawnRotation);
        NetworkObject attackNetObj = attackInstance.GetComponent<NetworkObject>();

        attackNetObj.Spawn();

        Telegraph _fillTelegraph = attackInstance.GetComponent<Telegraph>();
        if (_fillTelegraph != null)
        {
            _fillTelegraph.FillSpeed = modifiedCastTime;
            _fillTelegraph.crowdControl = gameObject.GetComponentInParent<CrowdControl>();
            _fillTelegraph.enemy = gameObject.GetComponentInParent<Enemy>();
        }
    }

    void SpawnAttack(Vector2 spawnPosition, Quaternion spawnRotation, NetworkObject attacker)
    {
        Vector2 offset = aimDirection.normalized * AttackRange_;

        GameObject attackInstance = Instantiate(AttackPrefab_, spawnPosition + offset, spawnRotation);
        NetworkObject attackNetObj = attackInstance.GetComponent<NetworkObject>();

        attackNetObj.Spawn();

        DamageOnTrigger damageOnTrigger = attackInstance.GetComponent<DamageOnTrigger>();
        if (damageOnTrigger != null)
        {
            damageOnTrigger.attacker = attacker;
            damageOnTrigger.AbilityDamage = AbilityDamage_;
            damageOnTrigger.IgnoreEnemy = true;
        }

        KnockbackOnTrigger knockbackOnTrigger = attackInstance.GetComponent<KnockbackOnTrigger>();
        if (knockbackOnTrigger != null)
        {
            knockbackOnTrigger.attacker = attacker;
            knockbackOnTrigger.Amount = knockBackAmount;
            knockbackOnTrigger.Duration = knockBackDuration;
            knockbackOnTrigger.Direction = aimDirection.normalized;
            knockbackOnTrigger.IgnoreEnemy = true;
        }

        DespawnDelay despawnDelay = attackInstance.GetComponent<DespawnDelay>();
        if (despawnDelay != null)
        {
            despawnDelay.StartCoroutine(despawnDelay.DespawnAfterDuration(actionTime));
        }
    }
}
