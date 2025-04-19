using System.Collections;
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

    [Header("Knockback")]
    [SerializeField] float knockBackAmount;
    [SerializeField] float knockBackDuration;

    [Header("Slide")]
    [SerializeField] float slideForce;
    [SerializeField] float slideDuration;

    Vector2 aimDirection;
    bool isSliding;
    bool canImpact;
    Coroutine impactCoroutine;

    public override void AbilityStart(EnemyStateMachine owner)
    {
        owner.CanSpecial = false;
        owner.IsAttacking = true;

        // Aim
        aimDirection = (owner.Target.position - transform.position).normalized;

        // Animate
        owner.EnemyAnimator.Play("Mobility Cast");
        owner.EnemyAnimator.SetFloat("Horizontal", aimDirection.x);
        owner.EnemyAnimator.SetFloat("Vertical", aimDirection.y);

        // Slide
        owner.EnemyRB.linearVelocity = Vector2.zero;
        isSliding = true;

        // Timers
        impactCoroutine = owner.StartCoroutine(AttackImpact(owner));
        owner.StartCoroutine(CoolDownTime(owner));
    }

    public override void AbilityUpdate(EnemyStateMachine owner)
    {
        if (canImpact)
        {
            canImpact = false;

            owner.EnemyAnimator.Play("Mobility Recovery");

            // Start Recovery
            if (IsServer)
            {
                //owner.player.CastBar.StartRecovery(recoveryTime, owner.player.CurrentAttackSpeed);
            }
            else
            {
                //owner.player.CastBar.StartRecoveryServerRpc(recoveryTime, owner.player.CurrentAttackSpeed);
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
        float modifiedCastTime = castTime / owner.enemy.CurrentAttackSpeed;

        yield return new WaitForSeconds(modifiedCastTime);

        owner.EnemyAnimator.Play("Mobility Impact");

        if (IsServer)
        {
            //SpawnAttack(owner.transform.position, aimRotation, aimDirection, owner.OwnerClientId);
        }
        else
        {
            //AttackServerRpc(owner.transform.position, aimRotation, aimDirection, owner.OwnerClientId);
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
}
