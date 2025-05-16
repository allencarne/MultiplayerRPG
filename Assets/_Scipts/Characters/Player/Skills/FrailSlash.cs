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

    float modifiedCastTime;
    float modifiedRecoveryTime;
    bool isSliding = false;
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
        owner.BodyAnimator.SetFloat("Horizontal", snappedDirection.x);
        owner.BodyAnimator.SetFloat("Vertical", snappedDirection.y);
        owner.BodyAnimator.Play("Sword_Attack_C");

        owner.SwordAnimator.SetFloat("Horizontal", snappedDirection.x);
        owner.SwordAnimator.SetFloat("Vertical", snappedDirection.y);
        owner.SwordAnimator.Play("Sword_Attack_C");

        owner.EyesAnimator.SetFloat("Horizontal", snappedDirection.x);
        owner.EyesAnimator.SetFloat("Vertical", snappedDirection.y);
        owner.EyesAnimator.Play("Sword_Attack_C");

        owner.HairAnimator.SetFloat("Horizontal", snappedDirection.x);
        owner.HairAnimator.SetFloat("Vertical", snappedDirection.y);
        owner.HairAnimator.Play("Sword_Attack_C_" + owner.player.hairIndex);

        // Cast Bar
        if (IsServer)
        {
            owner.player.CastBar.StartCast(castTime, owner.player.CurrentAttackSpeed.Value);
        }
        else
        {
            owner.player.CastBar.StartCastServerRpc(castTime, owner.player.CurrentAttackSpeed.Value);
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
        StartCoroutine(owner.CastTime(modifiedCastTime, modifiedRecoveryTime, this));
        StartCoroutine(owner.CoolDownTime(PlayerStateMachine.SkillType.Basic, coolDown));
    }

    public override void UpdateAbility(PlayerStateMachine owner)
    {
        owner.HandlePotentialInterrupt();
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
    void AttackServerRpc(Vector2 spawnPosition, Quaternion spawnRotation, Vector2 aimDirection, ulong attackerID, int attackerDamage)
    {
        SpawnAttack(spawnPosition, spawnRotation, aimDirection, attackerID, attackerDamage);
    }
}
