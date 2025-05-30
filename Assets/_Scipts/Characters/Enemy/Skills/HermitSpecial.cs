using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class HermitSpecial : EnemyAbility
{
    [SerializeField] GameObject attackPrefab;
    [SerializeField] GameObject telegraphPrefab;
    [SerializeField] int abilityDamage;
    [SerializeField] float attackRange;

    [Header("Time")]
    [SerializeField] float castTime;
    [SerializeField] float impactTime;
    [SerializeField] float recoveryTime;
    [SerializeField] float coolDown;

    [Header("Knockback")]
    [SerializeField] float knockBackAmount;
    [SerializeField] float knockBackDuration;

    [Header("Dash")]
    [SerializeField] float dashForce;

    float modifiedCooldown;
    Vector2 spawnPosition;
    Vector2 aimDirection;
    Quaternion aimRotation;
    Vector2 vectorToTarget;

    public override void AbilityStart(EnemyStateMachine owner)
    {
        owner.CanSpecial = false;
        owner.IsAttacking = true;

        // Set Variables
        aimDirection = (owner.Target.position - transform.position).normalized;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        aimRotation = Quaternion.Euler(0, 0, angle);
        spawnPosition = owner.transform.position;
        modifiedCooldown = coolDown / owner.enemy.CurrentCDR;

        float distanceToTarget = Vector2.Distance(transform.position, owner.Target.position);
        // Check if the target is within Range
        if (distanceToTarget > owner.SpecialRadius)
        {
            // If the target is beyond Range, set the target position to the maximum range
            vectorToTarget = (Vector2)transform.position + aimDirection * owner.SpecialRadius;
        }
        else
        {
            // If the target is within Range, set the target position to the target's position
            vectorToTarget = owner.Target.position;
        }

        // Stop Movement
        owner.EnemyRB.linearVelocity = Vector2.zero;

        // Animate
        owner.EnemyAnimator.Play("Special Cast");
        owner.EnemyAnimator.SetFloat("Horizontal", aimDirection.x);
        owner.EnemyAnimator.SetFloat("Vertical", aimDirection.y);

        // Dash
        StartCoroutine(DashDuration(owner));

        // Telegraph
        SpawnTelegraph(vectorToTarget, aimRotation, castTime + impactTime);
        owner.enemy.CastBar.StartCast(castTime, owner.enemy.CurrentAttackSpeed);

        // Timers
        StartCoroutine(owner.CastTime(EnemyStateMachine.SkillType.Special, castTime, impactTime, recoveryTime, this));
        StartCoroutine(owner.CoolDownTime(EnemyStateMachine.SkillType.Special, modifiedCooldown));
    }

    public override void AbilityUpdate(EnemyStateMachine owner)
    {
        owner.HandlePotentialInterrupt();
    }

    public override void AbilityFixedUpdate(EnemyStateMachine owner)
    {
        if (owner.CanDash)
        {
            owner.transform.position = Vector2.Lerp(transform.position, vectorToTarget, Time.fixedDeltaTime * dashForce);
        }
    }

    IEnumerator DashDuration(EnemyStateMachine owner)
    {
        yield return new WaitForSeconds(castTime);

        owner.Buffs.phase.StartPhase(impactTime + .2f);
        owner.Buffs.immoveable.StartImmovable(impactTime);
        owner.CanDash = true;

        yield return new WaitForSeconds(impactTime);

        owner.CanDash = false;
        if (owner.IsAttacking && !owner.enemy.isDead)
        {
            SpawnAttack(vectorToTarget, aimRotation, aimDirection, owner.NetworkObject);
        }
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
