using Unity.Netcode;
using UnityEngine;

public class NPCAttackState : NPCState
{
    [SerializeField] GameObject attackPrefab;
    [SerializeField] int abilityDamage;
    [SerializeField] float attackRange;
    [SerializeField] float CoolDown;

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

    public override void StartState(NPCStateMachine owner)
    {
        owner.CanBasic = false;
        owner.IsAttacking = true;

        owner.NpcRB.linearVelocity = Vector2.zero;

        modifiedCastTime = castTime / owner.npc.CurrentAttackSpeed;
        modifiedRecoveryTime = recoveryTime / owner.npc.CurrentAttackSpeed;
        Vector2 snappedDirection = owner.SnapDirection(aimDirection);

        // Animate
        owner.AnimateCast(snappedDirection);

        // Cast Bar
        owner.StartCastBar(castTime);

        // Slide
        owner.StartSlide();

        // Timers
        owner.StartCast(modifiedCastTime, modifiedRecoveryTime, this);
        StartCoroutine(owner.CoolDownTime(NPCStateMachine.SkillType.Basic, CoolDown));
    }

    public override void UpdateState(NPCStateMachine owner)
    {

    }

    public override void FixedUpdateState(NPCStateMachine owner)
    {
        if (owner.IsSliding)
        {
            owner.NpcRB.linearVelocity = aimDirection * slideForce;
            StartCoroutine(owner.SlideDuration(aimDirection, slideForce, slideDuration));
        }
    }

    public override void Impact(NPCStateMachine owner)
    {
        if (owner.IsServer)
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
        NPC npc = GetComponentInParent<NPC>();
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
            damageOnTrigger.CharacterDamage = npc.CurrentDamage;
            damageOnTrigger.IgnoreNPC = true;
        }

        KnockbackOnTrigger knockbackOnTrigger = attackInstance.GetComponent<KnockbackOnTrigger>();
        if (knockbackOnTrigger != null)
        {
            knockbackOnTrigger.attacker = attacker;

            knockbackOnTrigger.Amount = knockBackAmount;
            knockbackOnTrigger.Duration = knockBackDuration;
            knockbackOnTrigger.Direction = aimDirection.normalized;
            knockbackOnTrigger.IgnoreNPC = true;
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
