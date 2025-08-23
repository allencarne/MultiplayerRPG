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

    [Header("Slide")]
    [SerializeField] float slideForce;
    [SerializeField] float slideDuration;

    [Header("Knockback")]
    [SerializeField] float knockBackAmount;
    [SerializeField] float knockBackDuration;

    float modifiedCastTime;
    float modifiedRecoveryTime;
    Vector2 aimDirection;
    Quaternion aimRotation;

    public override void StartAbility(PlayerStateMachine owner)
    {
        owner.CanBasic = false;
        owner.IsAttacking = true;

        // Direction
        modifiedCastTime = castTime / owner.player.CurrentAttackSpeed.Value;
        modifiedRecoveryTime = recoveryTime / owner.player.CurrentAttackSpeed.Value;
        aimDirection = owner.Aimer.right;
        aimRotation = owner.Aimer.rotation;
        Vector2 snappedDirection = owner.SnapDirection(aimDirection);

        // Animate
        owner.AnimateCast(snappedDirection);

        // Cast Bar
        owner.StartCast(castTime);

        // Slide
        owner.StartSlide(true);

        // Timers
        owner.StartCast(modifiedCastTime, modifiedRecoveryTime, this);
        StartCoroutine(owner.CoolDownTime(PlayerStateMachine.SkillType.Basic, CoolDown));
    }

    public override void UpdateAbility(PlayerStateMachine owner)
    {

    }

    public override void FixedUpdateAbility(PlayerStateMachine owner)
    {
        if (owner.IsSliding)
        {
            owner.PlayerRB.linearVelocity = aimDirection * slideForce;
            StartCoroutine(owner.SlideDuration(aimDirection,slideForce,slideDuration));
        }
    }

    public override void Impact(PlayerStateMachine owner)
    {
        if (IsServer)
        {
            SpawnAttack();
        }
        else
        {
            AttackServerRpc(aimDirection, aimRotation);
        }
    }

    void SpawnAttack()
    {
        // Info
        NetworkObject attacker = GetComponentInParent<NetworkObject>();
        Player player = GetComponentInParent<Player>();
        Vector2 offset = aimDirection.normalized * attackRange;
        Vector2 spawnPosition = transform.position;

        // Spawn
        GameObject attackInstance = Instantiate(attackPrefab, spawnPosition + offset, aimRotation);
        NetworkObject attackNetObj = attackInstance.GetComponent<NetworkObject>();
        attackNetObj.Spawn();

        DamageOnTrigger damageOnTrigger = attackInstance.GetComponent<DamageOnTrigger>();
        if (damageOnTrigger != null)
        {
            damageOnTrigger.IsBasic = true;
            damageOnTrigger.attacker = attacker;
            damageOnTrigger.AbilityDamage = abilityDamage;
            damageOnTrigger.CharacterDamage = player.CurrentDamage.Value;
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
    void AttackServerRpc(Vector2 sentAimDirection, Quaternion sentAimRotation)
    {
        aimDirection = sentAimDirection;
        aimRotation = sentAimRotation;
        SpawnAttack();
    }
}
