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
    [SerializeField] float attackDuration;
    [SerializeField] float attackForce;

    [Header("Time")]
    [SerializeField] float castTime;
    [SerializeField] float recoveryTime;
    [SerializeField] float coolDown;

    [Header("Knockback")]
    [SerializeField] float knockBackAmount;
    [SerializeField] float knockBackDuration;

    float modifiedCastTime;
    Vector2 spawnPosition;
    Vector2 aimDirection;
    Quaternion aimRotation;
    bool canImpact = false;

    public override void AbilityStart(EnemyStateMachine owner)
    {
        owner.CanUltimate = false;
        owner.IsAttacking = true;

        // Set Veriables
        modifiedCastTime = castTime / owner.enemy.CurrentAttackSpeed;
        aimDirection = (owner.Target.position - transform.position).normalized;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        aimRotation = Quaternion.Euler(0, 0, angle);
        spawnPosition = owner.transform.position;

        // Stop Movement
        owner.EnemyRB.linearVelocity = Vector2.zero;

        // Animate
        owner.EnemyAnimator.Play("Ultimate Cast");
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

            owner.EnemyAnimator.Play("Ultimate Recovery");

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

    }

    IEnumerator AttackImpact(EnemyStateMachine owner)
    {
        yield return new WaitForSeconds(modifiedCastTime);

        owner.EnemyAnimator.Play("Ultimate Impact");

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
        float modifiedCooldown = coolDown / owner.enemy.CurrentCDR;

        yield return new WaitForSeconds(modifiedCooldown);

        owner.CanUltimate = true;
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

        StartCoroutine(DespawnAfterDuration(attackNetObj, attackDuration));
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
