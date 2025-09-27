using Unity.Netcode;
using UnityEngine;

public class SproutSlap : EnemyAbility
{
    [Header("Knockback")]
    [SerializeField] float knockBackAmount;
    [SerializeField] float knockBackDuration;
    float modifiedCastTime;
    Vector2 spawnPosition;
    Vector2 aimDirection;
    Quaternion aimRotation;

    public override void AbilityStart(EnemyStateMachine owner)
    {
        owner.InitializeAbility(skillType,this);

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
        if (stateTimer <= 0f) HandleStateTransition(owner);
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
                DoneState(owner);
                break;
        }
    }

    void CastState(EnemyStateMachine owner)
    {
        owner.EnemyAnimator.Play("Basic Cast");
        owner.enemy.CastBar.StartCast(CastTime, owner.enemy.CurrentAttackSpeed);

        SpawnTelegraph(spawnPosition, aimRotation, modifiedCastTime);
    }

    void ImpactState(EnemyStateMachine owner)
    {
        // Animate Impact
        owner.EnemyAnimator.Play("Basic Impact");

        SpawnAttack(spawnPosition, aimDirection, owner.NetworkObject);
    }

    void RecoveryState(EnemyStateMachine owner)
    {
        // Animate Recovery
        owner.EnemyAnimator.Play("Basic Recovery");
        owner.enemy.CastBar.StartRecovery(RecoveryTime, owner.enemy.CurrentAttackSpeed);
    }

    void SpawnTelegraph(Vector2 spawnPosition, Quaternion spawnRotation, float modifiedCastTime)
    {
        Vector2 offset = aimDirection.normalized * AttackRange_;

        GameObject attackInstance = Instantiate(TelegraphPrefab_, spawnPosition + offset, spawnRotation);
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
        Vector2 offset = aimDirection.normalized * AttackRange_;

        GameObject attackInstance = Instantiate(AttackPrefab_, spawnPosition + offset, Quaternion.identity);
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