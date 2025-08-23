using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class SnailDash : EnemyAbility
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
    [SerializeField] float abilityDuration;

    [Header("Slow")]
    [SerializeField] int slowStacks;
    [SerializeField] float slowDuration;

    [Header("Slide")]
    [SerializeField] float slideForce;
    [SerializeField] float slideDuration;

    float modifiedCooldown;
    Vector2 spawnPosition;
    Vector2 aimDirection;
    Quaternion aimRotation;
    bool isSliding;

    public override void AbilityStart(EnemyStateMachine owner)
    {
        owner.CanSpecial = false;
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
        owner.EnemyAnimator.Play("Special Cast");
        owner.EnemyAnimator.SetFloat("Horizontal", aimDirection.x);
        owner.EnemyAnimator.SetFloat("Vertical", aimDirection.y);

        // Telegraph
        SpawnTelegraph(spawnPosition, aimRotation, castTime);
        owner.enemy.CastBar.StartCast(castTime, owner.enemy.CurrentAttackSpeed);

        // Timers
        owner.StartCast(EnemyStateMachine.SkillType.Special, castTime, impactTime, recoveryTime, this);
        StartCoroutine(owner.CoolDownTime(EnemyStateMachine.SkillType.Special, modifiedCooldown));
    }

    public override void AbilityUpdate(EnemyStateMachine owner)
    {

    }

    public override void Impact(EnemyStateMachine owner)
    {
        isSliding = true;

        // Attack
        SpawnAttack(spawnPosition, aimRotation, aimDirection, owner.NetworkObject);

        // Buff
        owner.Buffs.immoveable.StartImmovable(slideDuration);

        // Slide
        owner.Buffs.phase.StartPhase(castTime + .2f);
    }

    public override void AbilityFixedUpdate(EnemyStateMachine owner)
    {
        if (isSliding)
        {
            owner.EnemyRB.linearVelocity = aimDirection * slideForce;

            StartCoroutine(SlideDuration(owner));
        }
    }

    IEnumerator SlideDuration(EnemyStateMachine owner)
    {
        float elapsed = 0f;
        Vector2 startVelocity = aimDirection * slideForce;

        while (elapsed < slideDuration)
        {
            float t = elapsed / slideDuration;
            owner.EnemyRB.linearVelocity = Vector2.Lerp(startVelocity, Vector2.zero, t);

            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        owner.EnemyRB.linearVelocity = Vector2.zero;
        isSliding = false;
    }

    void SpawnAttack(Vector2 spawnPosition, Quaternion spawnRotation, Vector2 aimDirection, NetworkObject attacker)
    {
        Vector2 offset = aimDirection.normalized * attackRange;

        GameObject attackInstance = Instantiate(attackPrefab, spawnPosition + offset, spawnRotation);
        NetworkObject attackNetObj = attackInstance.GetComponent<NetworkObject>();

        attackNetObj.Spawn();

        DamageOnTrigger _damage = attackInstance.GetComponent<DamageOnTrigger>();
        if (_damage != null)
        {
            _damage.IgnoreEnemy = true;
            _damage.attacker = attacker;
            _damage.AbilityDamage = abilityDamage;
        }

        SlowOnTrigger _slow = attackInstance.GetComponent<SlowOnTrigger>();
        if (_slow != null)
        {
            _slow.IgnoreEnemy = true;
            _slow.attacker = attacker;
            _slow.Stacks = slowStacks;
            _slow.Duration = slowDuration;
        }

        DespawnDelay despawnDelay = attackInstance.GetComponent<DespawnDelay>();
        if (despawnDelay != null)
        {
            despawnDelay.StartCoroutine(despawnDelay.DespawnAfterDuration(abilityDuration));
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
