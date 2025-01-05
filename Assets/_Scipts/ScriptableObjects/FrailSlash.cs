using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "ScriptableObjects/Abilities/Beginner/FrailSlash")]
public class FrailSlash : ScriptableObject, IAbilityBehaviour
{
    public UnityEvent OnBasicCoolDownStarted;

    [Header("Setup")]
    public Sprite icon;
    [SerializeField] GameObject attackPrefab;
    [SerializeField] GameObject hitEffect;

    [Header("Time")]
    [SerializeField] float castTime;
    [SerializeField] float recoveryTime;
    public float CoolDown;

    [Header("Stats")]
    [SerializeField] int damage;

    bool canImpact = false;

    public void BehaviourUpdate(PlayerStateMachine stateMachine)
    {
        if (stateMachine.CanBasic && !stateMachine.isAttacking)
        {
            stateMachine.isAttacking = true;
            stateMachine.CanBasic = false;

            Vector2 aimerDirection = stateMachine.Aimer.right;
            Vector2 snappedDirection = stateMachine.SnapDirection(aimerDirection);

            // Body
            stateMachine.BodyAnimator.Play("Sword_Attack_C");
            stateMachine.BodyAnimator.SetFloat("Horizontal", snappedDirection.x);
            stateMachine.BodyAnimator.SetFloat("Vertical", snappedDirection.y);

            // Sword
            stateMachine.SwordAnimator.Play("Sword_Attack_C");
            //stateMachine.SwordAnimator.SetFloat("Horizontal", direction.x);
            //stateMachine.SwordAnimator.SetFloat("Vertical", direction.y);

            // Timers
            stateMachine.StartCoroutine(AttackImpact(stateMachine));
            stateMachine.StartCoroutine(CoolDownTime(stateMachine));
        }

        if (canImpact)
        {
            canImpact = false;

            stateMachine.BodyAnimator.Play("Sword_Attack_R");
            stateMachine.SwordAnimator.Play("Sword_Attack_R");

            stateMachine.StartCoroutine(RecoveryTime(stateMachine));
        }
    }

    IEnumerator AttackImpact(PlayerStateMachine stateMachine)
    {
        float modifiedCastTime = castTime / stateMachine.Player.CurrentAttackSpeed;

        yield return new WaitForSeconds(modifiedCastTime);

        stateMachine.BodyAnimator.Play("Sword_Attack_I");
        stateMachine.SwordAnimator.Play("Sword_Attack_I");

        stateMachine.StartCoroutine(ImpactDelay());

        // Request the server to spawn the attack prefab
        RequestSpawnAttackServerRpc(stateMachine.transform.position, stateMachine.transform.rotation);
    }

    IEnumerator ImpactDelay()
    {
        yield return new WaitForSeconds(.1f);

        canImpact = true;
    }

    IEnumerator RecoveryTime(PlayerStateMachine stateMachine)
    {
        float modifiedRecoveryTime = recoveryTime / stateMachine.Player.CurrentAttackSpeed;

        yield return new WaitForSeconds(modifiedRecoveryTime);

        stateMachine.isAttacking = false;
        stateMachine.SetState(new PlayerIdleState(stateMachine));
    }

    IEnumerator CoolDownTime(PlayerStateMachine stateMachine)
    {
        OnBasicCoolDownStarted?.Invoke();

        // Adjust cooldown time based on cooldown reduction
        float modifiedCooldown = CoolDown / stateMachine.Player.CurrentCDR;

        yield return new WaitForSeconds(modifiedCooldown);

        stateMachine.CanBasic = true;
    }

    [ServerRpc]
    private void RequestSpawnAttackServerRpc(Vector3 position, Quaternion rotation)
    {
        // Ensure only the server spawns the prefab
        SpawnAttackPrefab(position, rotation);
    }

    private void SpawnAttackPrefab(Vector3 position, Quaternion rotation)
    {
        // Spawn the prefab and synchronize it across the network
        GameObject spawnedObject = Instantiate(attackPrefab, position, rotation);
        NetworkObject networkObject = spawnedObject.GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            networkObject.Spawn(); // Sync across the network
        }
    }
}
