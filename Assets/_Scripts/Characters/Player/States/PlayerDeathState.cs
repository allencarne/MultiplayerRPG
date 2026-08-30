using System.Collections;
using UnityEngine;

public class PlayerDeathState : PlayerState
{
    public PlayerDeathState(PlayerStateMachine owner) : base(owner) { }

    public override void EnterState()
    {
        if (!owner.IsOwner) return;

        // Prevents attacking while dead
        owner.IsAttacking = false;

        // Clear all buffs and debuffs
        owner.Buffs.PurgeAllDebuffs();
        owner.DeBuffs.CleanseAllDebuffs();

        // Play death animation
        owner.Animator.PlayAnimation("Death", owner.customization.net_ChestIndex.Value, owner.customization.net_LegsIndex.Value, owner.customization.WeaponAnimType);

        //owner.PlayerHeadAnimator.Play("Death");
        //owner.BodyAnimator.Play("Death");
        //owner.ChestAnimator.Play("Death_" + owner.customization.net_ChestIndex.Value);
        //owner.LegsAnimator.Play("Death_" + owner.customization.net_LegsIndex.Value);
        //if (owner.Equipment.IsWeaponEquipped) owner.WeaponAnimator.Play(owner.customization.WeaponAnimType + " Death", -1, 0);

        // Just the head Face Right
        owner.Animator.HeadAnimator.SetFloat("Horizontal", 1);
        owner.customization.net_FacingDirection.Value = new Vector2(1, 0);

        // Reset the cast bar
        owner.player.CastBar.ResetCastBar();

        // Disable the collider to prevent interactions while dead
        owner.RequestSetColliderServerRpc(false);

        // Start the respawn delay coroutine
        owner.StartCoroutine(Delay(owner));
    }

    IEnumerator Delay(PlayerStateMachine owner)
    {
        yield return new WaitForSeconds(5);

        owner.RequestRespawnServerRpc();
        owner.RequestSetColliderServerRpc(true);
        owner.transform.position = Vector2.zero;
        owner.SetState(new PlayerSpawnState(owner));
    }
}
