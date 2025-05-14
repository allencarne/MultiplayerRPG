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
    [SerializeField] float recoveryTime;
    [SerializeField] float coolDown;
    [SerializeField] float abilityDuration;

    [Header("Slow")]
    [SerializeField] int slowStacks;
    [SerializeField] float slowDuration;

    [Header("Slide")]
    [SerializeField] float slideForce;
    [SerializeField] float slideDuration;

    float modifiedCastTime;
    float modifiedRecoveryTime;
    Vector2 spawnPosition;
    Vector2 aimDirection;
    Quaternion aimRotation;
    bool isSliding;

    public override void AbilityStart(EnemyStateMachine owner)
    {
        owner.CanSpecial = false;
        owner.IsAttacking = true;

        // Set Veriables
        modifiedCastTime = castTime / owner.enemy.CurrentAttackSpeed;
        modifiedRecoveryTime = recoveryTime / owner.enemy.CurrentAttackSpeed;
        aimDirection = (owner.Target.position - transform.position).normalized;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        aimRotation = Quaternion.Euler(0, 0, angle);
        spawnPosition = owner.transform.position;

        // Stop Movement
        owner.EnemyRB.linearVelocity = Vector2.zero;

        // Animate
        owner.EnemyAnimator.Play("Special Cast");
        owner.EnemyAnimator.SetFloat("Horizontal", aimDirection.x);
        owner.EnemyAnimator.SetFloat("Vertical", aimDirection.y);

        // Telegraph
        SpawnTelegraph(spawnPosition, aimRotation, modifiedCastTime);
        owner.enemy.CastBar.StartCast(castTime, owner.enemy.CurrentAttackSpeed);

        // Timers
        StartCoroutine(owner.AttackImpact(1, modifiedCastTime, modifiedRecoveryTime));
        StartCoroutine(owner.CoolDownTime(1, coolDown));
    }

    public override void AbilityUpdate(EnemyStateMachine owner)
    {
        owner.HandlePotentialInterrupt(owner.ImpactCoroutine);

        if (owner.canImpact)
        {
            owner.canImpact = false;
            isSliding = true;

            // Attack
            SpawnAttack(spawnPosition, aimRotation, aimDirection, owner.NetworkObject);

            // Buff
            owner.Buffs.Immoveable(slideDuration);

            // Slide
            owner.Buffs.Phasing(modifiedCastTime + .2f);
        }
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

        StartCoroutine(DespawnAfterDuration(attackNetObj, abilityDuration));
    }

    IEnumerator DespawnAfterDuration(NetworkObject netObj, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (netObj != null && netObj.IsSpawned)
        {
            netObj.Despawn();
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
        }
    }
}
