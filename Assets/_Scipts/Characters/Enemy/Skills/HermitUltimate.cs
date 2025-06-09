using Unity.Netcode;
using UnityEngine;

public class HermitUltimate : EnemyAbility
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

    float modifiedCooldown;
    Vector2 spawnPosition;
    Vector2 aimDirection;
    Quaternion aimRotation;

    public override void AbilityStart(EnemyStateMachine owner)
    {
        owner.CanUltimate = false;
        owner.IsAttacking = true;

        // Set Variables
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
        StartCoroutine(owner.CastTime(EnemyStateMachine.SkillType.Ultimate, castTime, impactTime, recoveryTime, this));
        StartCoroutine(owner.CoolDownTime(EnemyStateMachine.SkillType.Ultimate, modifiedCooldown));
    }

    public override void AbilityUpdate(EnemyStateMachine owner)
    {

    }

    public override void AbilityFixedUpdate(EnemyStateMachine owner)
    {

    }

    public override void Impact(EnemyStateMachine owner)
    {
        owner.Buffs.phase.StartPhase(5);
        owner.Buffs.protection.StartProtection(2, 5);

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

        DestroyOnDeath death = attackInstance.GetComponent<DestroyOnDeath>();
        if (death != null)
        {
            death.enemy = GetComponentInParent<Enemy>();
        }
    }
}
