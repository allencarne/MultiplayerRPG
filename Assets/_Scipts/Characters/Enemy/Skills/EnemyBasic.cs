using Unity.Netcode;
using UnityEngine;

public class EnemyBasic : EnemyAbility
{
    [SerializeField] GameObject attackPrefab;
    [SerializeField] GameObject telegraphPrefab;
    [SerializeField] int abilityDamage;
    [SerializeField] float attackRange;

    [Header("Time")]
    [SerializeField] float castTime;
    float impactTime = .1f;
    [SerializeField] float recoveryTime;
    [SerializeField] float coolDown;

    [Header("Knockback")]
    [SerializeField] float knockBackAmount;
    [SerializeField] float knockBackDuration;

    float modifiedCastTime;
    float modifiedRecoveryTime;
    Vector2 spawnPosition;
    Vector2 aimDirection;
    Quaternion aimRotation;

    public override void AbilityStart(EnemyStateMachine owner)
    {
        owner.CanBasic = false;
        owner.IsAttacking = true;

        // Set Variables
        modifiedCastTime = castTime / owner.enemy.CurrentAttackSpeed;
        modifiedRecoveryTime = recoveryTime / owner.enemy.CurrentAttackSpeed;
        aimDirection = (owner.Target.position - transform.position).normalized;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        aimRotation = Quaternion.Euler(0, 0, angle);
        spawnPosition = owner.transform.position;

        // Stop Movement
        owner.EnemyRB.linearVelocity = Vector2.zero;

        // Animate
        owner.EnemyAnimator.Play("Basic Cast");
        owner.EnemyAnimator.SetFloat("Horizontal", aimDirection.x);
        owner.EnemyAnimator.SetFloat("Vertical", aimDirection.y);

        // Telegraph
        SpawnTelegraph(spawnPosition, aimRotation, modifiedCastTime);
        owner.enemy.CastBar.StartCast(castTime, owner.enemy.CurrentAttackSpeed);

        // Timers
        StartCoroutine(owner.CastTime(EnemyStateMachine.SkillType.Basic, modifiedCastTime, impactTime, modifiedRecoveryTime, this));
        StartCoroutine(owner.CoolDownTime(EnemyStateMachine.SkillType.Basic, coolDown));
    }

    public override void AbilityUpdate(EnemyStateMachine owner)
    {
        owner.HandlePotentialInterrupt();
    }

    public override void AbilityFixedUpdate(EnemyStateMachine owner)
    {

    }

    public override void Impact(EnemyStateMachine owner)
    {
        SpawnAttack(spawnPosition, aimRotation, aimDirection, owner.NetworkObject);
    }

    void SpawnTelegraph(Vector2 spawnPosition, Quaternion spawnRotation, float modifiedCastTime)
    {
        Vector2 offset = aimDirection.normalized * attackRange;

        GameObject attackInstance = Instantiate(telegraphPrefab, spawnPosition + offset, spawnRotation);
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

    void SpawnAttack(Vector2 spawnPosition, Quaternion spawnRotation, Vector2 aimDirection, NetworkObject attacker)
    {
        Vector2 offset = aimDirection.normalized * attackRange;

        GameObject attackInstance = Instantiate(attackPrefab, spawnPosition + offset, spawnRotation);
        NetworkObject attackNetObj = attackInstance.GetComponent<NetworkObject>();

        attackNetObj.Spawn();

        DamageOnTrigger damageOnTrigger = attackInstance.GetComponent<DamageOnTrigger>();
        if (damageOnTrigger != null)
        {
            damageOnTrigger.attacker = attacker;
            damageOnTrigger.AbilityDamage = abilityDamage;
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
