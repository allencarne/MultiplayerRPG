using Unity.Netcode;
using UnityEngine;

public class NutQuake : EnemyAbility
{
    [SerializeField] float actionTime;

    [Header("Knockback")]
    [SerializeField] float knockBackAmount;
    [SerializeField] float knockBackDuration;
    float modifiedCastTime;
    Vector2 spawnPosition;
    Vector2 aimDirection;

    public override void AbilityStart(EnemyStateMachine owner)
    {
        owner.CanUltimate = false;
        owner.IsAttacking = true;
        owner.currentAbility = this;

        // Stop
        owner.EnemyRB.linearVelocity = Vector2.zero;
        aimDirection = (owner.Target.position - transform.position).normalized;

        // Variables
        modifiedCastTime = CastTime / owner.enemy.CurrentAttackSpeed;
        spawnPosition = owner.transform.position;

        ChangeState(State.Cast, modifiedCastTime);
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
                ActionState(owner);
                ChangeState(State.Action, actionTime);
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

        owner.enemy.CastBar.StartCast(castTime: CastTime, owner.enemy.CurrentAttackSpeed);
        SpawnTelegraph(spawnPosition, modifiedCastTime + actionTime);
    }

    void ActionState(EnemyStateMachine owner)
    {
        owner.EnemyAnimator.Play("Ultimate Action");
    }

    void ImpactState(EnemyStateMachine owner)
    {
        // Animate Impact
        owner.EnemyAnimator.Play("Ultimate Impact");

        SpawnAttack(spawnPosition, aimDirection, owner.NetworkObject);
    }

    void RecoveryState(EnemyStateMachine owner)
    {
        // Animate Recovery
        owner.EnemyAnimator.Play("Ultimate Recovery");
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

    void SpawnTelegraph(Vector2 spawnPosition, float modifiedCastTime)
    {
        GameObject attackInstance = Instantiate(TelegraphPrefab_, spawnPosition, Quaternion.identity);
        NetworkObject attackNetObj = attackInstance.GetComponent<NetworkObject>();

        attackNetObj.Spawn();

        FillTelegraph _fillTelegraph = attackInstance.GetComponent<FillTelegraph>();
        if (_fillTelegraph != null)
        {
            _fillTelegraph.FillSpeed = modifiedCastTime;
            _fillTelegraph.crowdControl = gameObject.GetComponentInParent<CrowdControl>();
            _fillTelegraph.enemy = gameObject.GetComponentInParent<Enemy>();
        }
    }

    void SpawnAttack(Vector2 spawnPosition, Vector2 aimDirection, NetworkObject attacker)
    {
        GameObject attackInstance = Instantiate(AttackPrefab_, spawnPosition, Quaternion.identity);
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

        DestroyOnDeath death = attackInstance.GetComponent<DestroyOnDeath>();
        if (death != null)
        {
            death.enemy = GetComponentInParent<Enemy>();
        }
    }
}
