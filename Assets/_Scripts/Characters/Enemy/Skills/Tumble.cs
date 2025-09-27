using Unity.Netcode;
using UnityEngine;

public class Tumble : EnemyAbility
{
    [Header("Knockback")]
    [SerializeField] float knockBackAmount;
    [SerializeField] float knockBackDuration;

    [Header("Slide")]
    [SerializeField] float slideForce;

    public override void AbilityStart(EnemyStateMachine owner)
    {
        owner.InitializeAbility(skillType, this);

        // Stop
        owner.EnemyRB.linearVelocity = Vector2.zero;

        // Cast Time
        ModifiedCastTime = CastTime / owner.enemy.CurrentAttackSpeed;

        // Spawn Position
        SpawnPosition = owner.transform.position;

        // Direction
        AimDirection = (owner.Target.position - transform.position).normalized;
        float angle = Mathf.Atan2(AimDirection.y, AimDirection.x) * Mathf.Rad2Deg;
        AimRotation = Quaternion.Euler(0, 0, angle);

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
        if (currentState == State.Action)
        {
            owner.EnemyRB.linearVelocity = AimDirection * slideForce;
        }
    }

    private void HandleStateTransition(EnemyStateMachine owner)
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
        SpawnTelegraph(SpawnPosition, AimRotation, ModifiedCastTime + ActionTime);
    }

    void ActionState(EnemyStateMachine owner)
    {
        owner.EnemyAnimator.Play("Special Impact");
    }

    void ImpactState(EnemyStateMachine owner)
    {
        owner.EnemyAnimator.Play("Special Impact");
        SpawnAttack(SpawnPosition, AimRotation, owner.NetworkObject);
    }

    void RecoveryState(EnemyStateMachine owner)
    {
        owner.EnemyAnimator.Play("Special Recovery");
        owner.enemy.CastBar.StartRecovery(RecoveryTime, owner.enemy.CurrentAttackSpeed);
    }

    void SpawnTelegraph(Vector2 spawnPosition, Quaternion spawnRotation, float modifiedCastTime)
    {
        Vector2 offset = AimDirection.normalized * AttackRange_;

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
        Vector2 offset = AimDirection.normalized * AttackRange_;

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
            knockbackOnTrigger.Direction = AimDirection.normalized;
            knockbackOnTrigger.IgnoreEnemy = true;
        }

        DespawnDelay despawnDelay = attackInstance.GetComponent<DespawnDelay>();
        if (despawnDelay != null)
        {
            despawnDelay.StartCoroutine(despawnDelay.DespawnAfterDuration(ActionTime));
        }
    }
}
