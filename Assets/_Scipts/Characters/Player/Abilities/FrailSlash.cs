using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class FrailSlash : PlayerAbility
{
    [SerializeField] GameObject attackPrefab;
    [SerializeField] int abilityDamage;
    [SerializeField] float attackRange;

    [Header("Time")]
    [SerializeField] float castTime;
    [SerializeField] float recoveryTime;
    [SerializeField] float coolDown;

    [Header("Slide")]
    [SerializeField] float slideForce;
    [SerializeField] float slideDuration;

    [Header("Knockback")]
    [SerializeField] float knockBackAmount;
    [SerializeField] float knockBackDuration;

    bool canImpact = false;
    bool isSliding = false;
    Vector2 aimDirection;
    Quaternion aimRotation;
    Coroutine impactCoroutine;

    public override void StartAbility(PlayerStateMachine owner)
    {
        owner.CanBasic = false;
        owner.IsAttacking = true;

        // Direction
        aimDirection = owner.Aimer.right;
        aimRotation = owner.Aimer.rotation;
        Vector2 snappedDirection = owner.SnapDirection(aimDirection);

        // Animate
        owner.BodyAnimator.SetFloat("Horizontal", snappedDirection.x);
        owner.BodyAnimator.SetFloat("Vertical", snappedDirection.y);
        owner.BodyAnimator.Play("Sword_Attack_C");

        owner.SwordAnimator.SetFloat("Horizontal", snappedDirection.x);
        owner.SwordAnimator.SetFloat("Vertical", snappedDirection.y);
        owner.SwordAnimator.Play("Sword_Attack_C");

        owner.EyesAnimator.SetFloat("Horizontal", snappedDirection.x);
        owner.EyesAnimator.SetFloat("Vertical", snappedDirection.y);
        owner.EyesAnimator.Play("Sword_Attack_C");

        // Cast Bar
        if (IsServer)
        {
            owner.player.CastBar.StartCast(castTime, owner.player.CurrentAttackSpeed);
        }
        else
        {
            owner.player.CastBar.StartCastServerRpc(castTime, owner.player.CurrentAttackSpeed);
        }

        // Slide
        if (owner.InputHandler.MoveInput != Vector2.zero)
        {
            isSliding = true;
        }
        else
        {
            owner.PlayerRB.linearVelocity = Vector2.zero;
        }

        // Timers
        impactCoroutine = owner.StartCoroutine(AttackImpact(owner));
        owner.StartCoroutine(CoolDownTime(owner));
    }

    public override void UpdateAbility(PlayerStateMachine owner)
    {
        owner.HandlePotentialInterrupt(impactCoroutine);

        if (canImpact)
        {
            canImpact = false;

            owner.BodyAnimator.Play("Sword_Attack_R");
            owner.SwordAnimator.Play("Sword_Attack_R");
            owner.EyesAnimator.Play("Sword_Attack_R");

            // Start Recovery
            if (IsServer)
            {
                owner.player.CastBar.StartRecovery(recoveryTime, owner.player.CurrentAttackSpeed);
            }
            else
            {
                owner.player.CastBar.StartRecoveryServerRpc(recoveryTime, owner.player.CurrentAttackSpeed);
            }

            owner.StartCoroutine(RecoveryTime(owner));
        }
    }

    public override void FixedUpdateAbility(PlayerStateMachine owner)
    {
        if (isSliding)
        {
            owner.PlayerRB.linearVelocity = aimDirection * slideForce;

            StartCoroutine(SlideDuration(owner));
        }
    }

    IEnumerator SlideDuration(PlayerStateMachine owner)
    {
        float elapsed = 0f;
        Vector2 startVelocity = aimDirection * slideForce;

        while (elapsed < slideDuration)
        {
            float t = elapsed / slideDuration;
            owner.PlayerRB.linearVelocity = Vector2.Lerp(startVelocity, Vector2.zero, t);

            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        owner.PlayerRB.linearVelocity = Vector2.zero;
        isSliding = false;
    }

    IEnumerator AttackImpact(PlayerStateMachine owner)
    {
        float modifiedCastTime = castTime / owner.player.CurrentAttackSpeed;

        yield return new WaitForSeconds(modifiedCastTime);

        owner.BodyAnimator.Play("Sword_Attack_I");
        owner.SwordAnimator.Play("Sword_Attack_I");
        owner.EyesAnimator.Play("Sword_Attack_I");

        if (IsServer)
        {
            SpawnAttack(owner.transform.position, aimRotation, aimDirection,owner.OwnerClientId);
        }
        else
        {
            AttackServerRpc(owner.transform.position, aimRotation, aimDirection, owner.OwnerClientId);
        }

        owner.StartCoroutine(ImpactTime());
    }

    IEnumerator ImpactTime()
    {
        yield return new WaitForSeconds(.1f);

        canImpact = true;
    }

    IEnumerator RecoveryTime(PlayerStateMachine owner)
    {
        float modifiedRecoveryTime = recoveryTime / owner.player.CurrentAttackSpeed;

        yield return new WaitForSeconds(modifiedRecoveryTime);

        owner.IsAttacking = false;
        owner.SetState(PlayerStateMachine.State.Idle);
    }

    IEnumerator CoolDownTime(PlayerStateMachine owner)
    {
        //OnBasicCoolDownStarted?.Invoke();

        // Adjust cooldown time based on cooldown reduction
        float modifiedCooldown = coolDown / owner.player.CurrentCDR;

        yield return new WaitForSeconds(modifiedCooldown);

        owner.CanBasic = true;
    }

    void SpawnAttack(Vector2 spawnPosition, Quaternion spawnRotation, Vector2 aimDirection, ulong attackerID)
    {
        NetworkObject Attacker = NetworkManager.Singleton.ConnectedClients[attackerID].PlayerObject;

        Vector2 offset = aimDirection.normalized * attackRange;

        GameObject attackInstance = Instantiate(attackPrefab, spawnPosition + offset, spawnRotation);
        NetworkObject attackNetObj = attackInstance.GetComponent<NetworkObject>();

        attackNetObj.Spawn();

        DamageOnTrigger damageOnTrigger = attackInstance.GetComponent<DamageOnTrigger>();
        if (damageOnTrigger != null)
        {
            damageOnTrigger.attacker = Attacker;
            damageOnTrigger.Damage = abilityDamage;
        }

        KnockbackOnTrigger knockbackOnTrigger = attackInstance.GetComponent<KnockbackOnTrigger>();
        if (knockbackOnTrigger != null)
        {
            knockbackOnTrigger.attacker = Attacker;

            knockbackOnTrigger.Amount = knockBackAmount;
            knockbackOnTrigger.Duration = knockBackDuration;
            knockbackOnTrigger.Direction = aimDirection.normalized;
        }
    }

    [ServerRpc]
    void AttackServerRpc(Vector2 spawnPosition, Quaternion spawnRotation, Vector2 aimDirection, ulong attackerID)
    {
        SpawnAttack(spawnPosition, spawnRotation, aimDirection, attackerID);
    }
}
