using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class FlickerShot : PlayerAbility
{
    [SerializeField] GameObject attackPrefab;
    [SerializeField] int abilityDamage;
    [SerializeField] float attackRange;
    [SerializeField] float attackDuration;
    [SerializeField] float attackForce;

    [Header("Time")]
    [SerializeField] float castTime;
    [SerializeField] float recoveryTime;
    [SerializeField] float coolDown;

    Vector2 aimDirection;
    Quaternion aimRotation;

    public override void StartAbility(PlayerStateMachine owner)
    {
        owner.CanOffensive = false;
        owner.IsAttacking = true;

        // Set Variables
        aimDirection = owner.Aimer.right;
        aimRotation = owner.Aimer.rotation;
        Vector2 snappedDirection = owner.SnapDirection(aimDirection);

        // Animate
        owner.AnimateCast(snappedDirection);

        // Cast Bar
        owner.StartCast(castTime);

        // Timers
        StartCoroutine(owner.CastTime(castTime, recoveryTime, this));
        StartCoroutine(owner.CoolDownTime(PlayerStateMachine.SkillType.Offensive, coolDown));
    }

    public override void UpdateAbility(PlayerStateMachine owner)
    {
        owner.HandlePotentialInterrupt();
    }

    public override void FixedUpdateAbility(PlayerStateMachine owner)
    {

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

        Rigidbody2D attackRB = attackInstance.GetComponent<Rigidbody2D>();
        attackRB.AddForce(aimDirection * attackForce, ForceMode2D.Impulse);

        DamageOnTrigger damageOnTrigger = attackInstance.GetComponent<DamageOnTrigger>();
        if (damageOnTrigger != null)
        {
            damageOnTrigger.attacker = Attacker;
            damageOnTrigger.AbilityDamage = abilityDamage;
            damageOnTrigger.CharacterDamage = attackerDamage;
        }

        DespawnDelay despawnDelay = attackInstance.GetComponent<DespawnDelay>();
        if (despawnDelay != null)
        {
            despawnDelay.StartCoroutine(despawnDelay.DespawnAfterDuration(attackDuration));
        }
    }

    [ServerRpc]
    void AttackServerRpc(Vector2 spawnPosition, Quaternion spawnRotation, Vector2 aimDirection, ulong attackerID, int attackerDamage)
    {
        SpawnAttack(spawnPosition, spawnRotation, aimDirection, attackerID, attackerDamage);
    }
}
