using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class SnailShell : EnemyAbility
{
    [Header("Attack")]
    [SerializeField] GameObject attackPrefab;
    [SerializeField] GameObject telegraphPrefab;
    [SerializeField] int abilityDamage;
    [SerializeField] float attackRange;

    [Header("Projectile")]
    [SerializeField] float attackDuration;
    [SerializeField] float attackForce;

    [Header("Time")]
    [SerializeField] float castTime;
    float impactTime = .1f;
    [SerializeField] float recoveryTime;
    [SerializeField] float coolDown;

    [Header("Knockback")]
    [SerializeField] float knockBackAmount;
    [SerializeField] float knockBackDuration;

    float modifiedCooldown;
    Vector2 spawnPosition;
    Vector2 aimDirection;
    Quaternion aimRotation;

    public override void AbilityStart(EnemyStateMachine owner)
    {
        owner.CanUltimate = false;
        owner.IsAttacking = true;

        // Set Veriables
        aimDirection = (owner.Target.position - transform.position).normalized;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        aimRotation = Quaternion.Euler(0, 0, angle);
        spawnPosition = owner.transform.position;
        modifiedCooldown = coolDown / owner.enemy.CurrentCDR;

        // Stop Movement
        owner.EnemyRB.linearVelocity = Vector2.zero;

        // Animate
        owner.EnemyAnimator.Play("Ultimate Cast");
        owner.EnemyAnimator.SetFloat("Horizontal", aimDirection.x);
        owner.EnemyAnimator.SetFloat("Vertical", aimDirection.y);

        // Telegraph
        SpawnTelegraph(spawnPosition, aimRotation, castTime);
        owner.enemy.CastBar.StartCast(castTime, owner.enemy.CurrentAttackSpeed);

        // Timers
        owner.StartCast(EnemyStateMachine.SkillType.Ultimate, castTime, impactTime, recoveryTime, this);
        StartCoroutine(owner.CoolDownTime(EnemyStateMachine.SkillType.Ultimate, modifiedCooldown));
    }

    public override void AbilityUpdate(EnemyStateMachine owner)
    {

    }

    public override void Impact(EnemyStateMachine owner)
    {
        SpawnAttack(spawnPosition, aimRotation, aimDirection, owner.NetworkObject);
    }

    public override void AbilityFixedUpdate(EnemyStateMachine owner)
    {

    }

    void SpawnAttack(Vector2 spawnPosition, Quaternion spawnRotation, Vector2 aimDirection, NetworkObject attacker)
    {
        Vector2 offset = aimDirection.normalized * attackRange;

        GameObject attackInstance = Instantiate(attackPrefab, spawnPosition + offset, spawnRotation);
        NetworkObject attackNetObj = attackInstance.GetComponent<NetworkObject>();

        attackNetObj.Spawn();

        Rigidbody2D attackRB = attackInstance.GetComponent<Rigidbody2D>();
        attackRB.AddForce(aimDirection * attackForce, ForceMode2D.Impulse);

        DamageOnTrigger damageOnTrigger = attackInstance.GetComponent<DamageOnTrigger>();
        if (damageOnTrigger != null)
        {
            damageOnTrigger.attacker = attacker;
            damageOnTrigger.AbilityDamage = abilityDamage;
            damageOnTrigger.IgnoreEnemy = true;
            damageOnTrigger.IsBreakable = true;
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
            despawnDelay.StartCoroutine(despawnDelay.DespawnAfterDuration(attackDuration));
        }
    }

    void SpawnTelegraph(Vector2 spawnPosition, Quaternion spawnRotation, float modifiedCastTime)
    {
        Vector2 offset = aimDirection.normalized * attackRange;

        GameObject attackInstance = Instantiate(telegraphPrefab, spawnPosition + offset, spawnRotation);
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
}
