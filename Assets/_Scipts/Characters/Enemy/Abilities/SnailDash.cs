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
    Vector2 spawnPosition;
    Vector2 aimDirection;
    Quaternion aimRotation;
    bool isSliding;
    bool canImpact;

    public override void AbilityStart(EnemyStateMachine owner)
    {
        owner.CanSpecial = false;
        owner.IsAttacking = true;

        // Aim
        modifiedCastTime = castTime / owner.enemy.CurrentAttackSpeed;
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

        // Cast Bar
        if (IsServer)
        {
            SpawnTelegraph(spawnPosition, aimRotation, modifiedCastTime);
            owner.enemy.CastBar.StartCast(castTime, owner.enemy.CurrentAttackSpeed);
        }
        else
        {
            TelegraphServerRPC(spawnPosition, aimRotation, modifiedCastTime);
            owner.enemy.CastBar.StartCastServerRpc(castTime, owner.enemy.CurrentAttackSpeed);
        }

        // Timers
        owner.ImpactCoroutine = owner.StartCoroutine(AttackImpact(owner));
        owner.StartCoroutine(CoolDownTime(owner));
    }

    public override void AbilityUpdate(EnemyStateMachine owner)
    {
        owner.HandlePotentialInterrupt(owner.ImpactCoroutine);

        if (canImpact)
        {
            canImpact = false;

            owner.EnemyAnimator.Play("Special Recovery");

            // Start Recovery
            if (IsServer)
            {
                owner.enemy.CastBar.StartRecovery(recoveryTime, owner.enemy.CurrentAttackSpeed);
            }
            else
            {
                owner.enemy.CastBar.StartRecoveryServerRpc(recoveryTime, owner.enemy.CurrentAttackSpeed);
            }

            owner.StartCoroutine(RecoveryTime(owner));
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

    IEnumerator AttackImpact(EnemyStateMachine owner)
    {
        yield return new WaitForSeconds(modifiedCastTime);

        owner.EnemyAnimator.Play("Special Impact");

        // Buff
        owner.Buffs.Immoveable(slideDuration);

        // Slide
        owner.Buffs.Phasing(modifiedCastTime + .2f);
        isSliding = true;

        if (IsServer)
        {
            SpawnAttack(spawnPosition, aimRotation, aimDirection, owner.NetworkObject);
        }
        else
        {
            AttackServerRpc(spawnPosition, aimRotation, aimDirection);
        }

        owner.StartCoroutine(ImpactTime());
    }

    IEnumerator ImpactTime()
    {
        yield return new WaitForSeconds(.1f);

        canImpact = true;
    }

    IEnumerator RecoveryTime(EnemyStateMachine owner)
    {
        float modifiedRecoveryTime = recoveryTime / owner.enemy.CurrentAttackSpeed;

        yield return new WaitForSeconds(modifiedRecoveryTime);

        owner.IsAttacking = false;
        owner.SetState(EnemyStateMachine.State.Idle);
    }

    IEnumerator CoolDownTime(EnemyStateMachine owner)
    {
        // Adjust cooldown time based on cooldown reduction
        float modifiedCooldown = coolDown / owner.enemy.CurrentCDR;

        yield return new WaitForSeconds(modifiedCooldown);

        owner.CanSpecial = true;
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
            _damage.attacker = attacker;
            _damage.Damage = abilityDamage;
            _damage.IgnoreEnemy = true;
        }

        SlowOnTrigger _slow = attackInstance.GetComponent<SlowOnTrigger>();
        if (_slow != null)
        {
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

    [ServerRpc]
    void AttackServerRpc(Vector2 spawnPosition, Quaternion spawnRotation, Vector2 aimDirection)
    {
        NetworkObject attacker = GetComponentInParent<NetworkObject>();

        SpawnAttack(spawnPosition, spawnRotation, aimDirection, attacker);
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

    [ServerRpc]
    void TelegraphServerRPC(Vector2 spawnPosition, Quaternion spawnRotation, float modifiedCastTime)
    {
        SpawnTelegraph(spawnPosition, spawnRotation, modifiedCastTime);
    }
}
