using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class FlickerShot : PlayerAbility
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

    float modifiedCastTime;
    float modifiedRecoveryTime;
    Vector2 aimDirection;
    Quaternion aimRotation;

    public override void StartAbility(PlayerStateMachine owner)
    {
        owner.CanOffensive = false;
        owner.IsAttacking = true;

        // Set Variables
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
        owner.StartSlide();

        // Timers
        StartCoroutine(owner.CastTime(modifiedCastTime, modifiedRecoveryTime, this));
        StartCoroutine(owner.CoolDownTime(PlayerStateMachine.SkillType.Offensive, coolDown));
    }

    public override void UpdateAbility(PlayerStateMachine owner)
    {
        owner.HandlePotentialInterrupt();
    }

    public override void FixedUpdateAbility(PlayerStateMachine owner)
    {
        if (owner.IsSliding)
        {
            owner.PlayerRB.linearVelocity = aimDirection * slideForce;
            StartCoroutine(owner.SlideDuration(aimDirection, slideForce, slideDuration));
        }
    }

    public override void Impact(PlayerStateMachine owner)
    {
        if (IsServer)
        {
            SpawnAttack(transform.position, aimRotation, aimDirection, OwnerClientId, owner.player.CurrentDamage.Value);
        }
        else
        {
            AttackServerRpc(transform.position, aimRotation, aimDirection, OwnerClientId, owner.player.CurrentDamage.Value);
        }
    }

    void SpawnAttack(Vector2 spawnPosition, Quaternion spawnRotation, Vector2 aimDirection, ulong attackerID, int attackerDamage)
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
            damageOnTrigger.AbilityDamage = abilityDamage;
            damageOnTrigger.CharacterDamage = attackerDamage;
        }
    }

    [ServerRpc]
    void AttackServerRpc(Vector2 spawnPosition, Quaternion spawnRotation, Vector2 aimDirection, ulong attackerID, int attackerDamage)
    {
        SpawnAttack(spawnPosition, spawnRotation, aimDirection, attackerID, attackerDamage);
    }
}
