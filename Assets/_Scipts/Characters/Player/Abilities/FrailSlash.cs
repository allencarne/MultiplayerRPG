using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class FrailSlash : PlayerAbility
{
    [SerializeField] GameObject attackPrefab;
    [SerializeField] int abilityDamage;

    [Header("Time")]
    [SerializeField] float castTime;
    [SerializeField] float recoveryTime;
    [SerializeField] float coolDown;

    bool canImpact = false;
    Vector2 aimDirection;
    Quaternion attackRot;

    public override void StartAbility(PlayerStateMachine owner)
    {
        owner.CanBasic = false;
        aimDirection = owner.Aimer.right;
        attackRot = owner.Aimer.rotation;
        Vector2 snappedDirection = owner.SnapDirection(aimDirection);

        // Body
        owner.BodyAnimator.Play("Sword_Attack_C");
        owner.BodyAnimator.SetFloat("Horizontal", snappedDirection.x);
        owner.BodyAnimator.SetFloat("Vertical", snappedDirection.y);

        // Sword
        owner.SwordAnimator.Play("Sword_Attack_C");
        //stateMachine.SwordAnimator.SetFloat("Horizontal", direction.x);
        //stateMachine.SwordAnimator.SetFloat("Vertical", direction.y);

        // Timers
        owner.StartCoroutine(AttackImpact(owner));
        owner.StartCoroutine(CoolDownTime(owner));
    }

    public override void UpdateAbility(PlayerStateMachine owner)
    {
        if (canImpact)
        {
            canImpact = false;

            owner.BodyAnimator.Play("Sword_Attack_R");
            owner.SwordAnimator.Play("Sword_Attack_R");

            owner.StartCoroutine(RecoveryTime(owner));
        }
    }

    public override void FixedUpdateAbility(PlayerStateMachine owner)
    {

    }

    IEnumerator AttackImpact(PlayerStateMachine owner)
    {
        float modifiedCastTime = castTime / owner.player.CurrentAttackSpeed;

        yield return new WaitForSeconds(modifiedCastTime);

        owner.BodyAnimator.Play("Sword_Attack_I");
        owner.SwordAnimator.Play("Sword_Attack_I");

        owner.StartCoroutine(ImpactTime());

        if (IsServer)
        {
            SpawnAttack(owner.transform.position, attackRot, owner.OwnerClientId);
        }
        else
        {
            AttackServerRpc(owner.transform.position, attackRot, owner.OwnerClientId);
        }
    }

    [ServerRpc]
    void AttackServerRpc(Vector2 spawnPosition, Quaternion spawnRotation, ulong attackerID)
    {
        SpawnAttack(spawnPosition, spawnRotation, attackerID);
    }

    void SpawnAttack(Vector2 spawnPosition, Quaternion spawnRotation, ulong attackerID)
    {
        GameObject attackInstance = Instantiate(attackPrefab, spawnPosition, spawnRotation);
        NetworkObject attackNetObj = attackInstance.GetComponent<NetworkObject>();

        attackNetObj.SpawnWithOwnership(attackerID);

        DamageOnTrigger damageOnTrigger = attackInstance.GetComponent<DamageOnTrigger>();
        if (damageOnTrigger != null)
        {
            damageOnTrigger.AttackerClientId = attackerID;
            damageOnTrigger.Damage = abilityDamage;
        }
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

        owner.isAttacking = false;
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
}
