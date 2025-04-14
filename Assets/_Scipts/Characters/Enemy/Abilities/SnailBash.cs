using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class SnailBash : EnemyAbility
{
    [SerializeField] GameObject attackPrefab;
    [SerializeField] GameObject telegraphPrefab;
    [SerializeField] int abilityDamage;
    [SerializeField] float attackRange;

    [Header("Time")]
    [SerializeField] float castTime;
    [SerializeField] float recoveryTime;
    [SerializeField] float coolDown;

    [Header("Knockback")]
    [SerializeField] float knockBackAmount;
    [SerializeField] float knockBackDuration;

    bool canImpact = false;
    Vector2 aimDirection;
    Quaternion aimRotation;
    Coroutine impactCoroutine;

    public override void AbilityStart(EnemyStateMachine owner)
    {
        owner.CanBasic = false;
        owner.IsAttacking = true;

        float modifiedCastTime = castTime / owner.enemy.CurrentAttackSpeed;

        // Get the direction from enemy to the target
        aimDirection = (owner.Target.position - transform.position).normalized;

        // Convert that direction to an angle in degrees
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

        // Create a rotation around the Z axis
        aimRotation = Quaternion.Euler(0, 0, angle);

        owner.EnemyAnimator.Play("Basic Cast");
        owner.EnemyAnimator.SetFloat("Horizontal", aimDirection.x);
        owner.EnemyAnimator.SetFloat("Vertical", aimDirection.y);

        // Cast Bar
        if (IsServer)
        {
            SpawnTelegraph(owner.transform.position, aimRotation, modifiedCastTime);
            owner.enemy.CastBar.StartCast(castTime, owner.enemy.CurrentAttackSpeed);
        }
        else
        {
            TelegraphServerRPC(owner.transform.position, aimRotation, modifiedCastTime);
            owner.enemy.CastBar.StartCastServerRpc(castTime, owner.enemy.CurrentAttackSpeed);
        }

        // Timers
        impactCoroutine = owner.StartCoroutine(AttackImpact(owner));
        owner.StartCoroutine(CoolDownTime(owner));
    }

    public override void AbilityUpdate(EnemyStateMachine owner)
    {
        owner.HandlePotentialInterrupt(impactCoroutine);

        if (canImpact)
        {
            canImpact = false;

            owner.EnemyAnimator.Play("Basic Recovery");

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
        float modifiedCastTime = castTime / owner.enemy.CurrentAttackSpeed;

        yield return new WaitForSeconds(modifiedCastTime);

        owner.EnemyAnimator.Play("Basic Impact");

        if (IsServer)
        {
            SpawnAttack(owner.transform.position, aimRotation, aimDirection, owner.NetworkObject);
        }
        else
        {
            AttackServerRpc(owner.transform.position, aimRotation, aimDirection);
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
        //OnBasicCoolDownStarted?.Invoke();

        // Adjust cooldown time based on cooldown reduction
        float modifiedCooldown = coolDown / owner.enemy.CurrentCDR;

        yield return new WaitForSeconds(modifiedCooldown);

        owner.CanBasic = true;
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
            damageOnTrigger.Damage = abilityDamage;
        }

        KnockbackOnTrigger knockbackOnTrigger = attackInstance.GetComponent<KnockbackOnTrigger>();
        if (knockbackOnTrigger != null)
        {
            knockbackOnTrigger.attacker = attacker;

            knockbackOnTrigger.Amount = knockBackAmount;
            knockbackOnTrigger.Duration = knockBackDuration;
            knockbackOnTrigger.Direction = aimDirection.normalized;
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

        FillTelegraph _fillTelegraph = attackInstance.GetComponent<FillTelegraph>();
        if (_fillTelegraph != null)
        {
            _fillTelegraph.FillSpeed.Value = modifiedCastTime;
        }
    }

    [ServerRpc]
    void TelegraphServerRPC(Vector2 spawnPosition, Quaternion spawnRotation, float modifiedCastTime)
    {
        SpawnTelegraph(spawnPosition, spawnRotation, modifiedCastTime);
    }
}
