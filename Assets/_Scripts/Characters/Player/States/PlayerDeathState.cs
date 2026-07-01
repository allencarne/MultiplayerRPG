using System.Collections;
using UnityEngine;

public class PlayerDeathState : PlayerState
{
    public override void StartState(PlayerStateMachine owner)
    {
        if (!owner.IsOwner) return;

        // Prevents attacking while dead
        owner.IsAttacking = false;

        // Clear all buffs and debuffs
        owner.Buffs.PurgeAllDebuffs();
        owner.DeBuffs.CleanseAllDebuffs();

        // Play death animation
        owner.PlayerHeadAnimator.Play("Death");
        owner.BodyAnimator.Play("Death");
        owner.ChestAnimator.Play("Death_" + owner.customization.net_ChestIndex.Value);
        owner.LegsAnimator.Play("Death_" + owner.customization.net_LegsIndex.Value);
        if (owner.Equipment.IsWeaponEquipped) owner.WeaponAnimator.Play(owner.customization.WeaponAnimType + " Death", -1, 0);

        // Face Down
        owner.PlayerHeadAnimator.SetFloat("Horizontal", 1);
        owner.customization.net_FacingDirection.Value = new Vector2(1, 0);

        // Reset the cast bar
        owner.player.CastBar.ResetCastBar();

        // Disable the collider to prevent interactions while dead
        owner.RequestSetColliderServerRpc(false);

        // Start the respawn delay coroutine
        StartCoroutine(Delay(owner));
    }

    public override void FixedUpdateState(PlayerStateMachine owner)
    {

    }
    public override void UpdateState(PlayerStateMachine owner)
    {

    }

    IEnumerator Delay(PlayerStateMachine owner)
    {
        yield return new WaitForSeconds(4);
        owner.RequestSetSpritesServerRpc(false);
        owner.transform.position = Vector2.zero;

        yield return new WaitForSeconds(1);

        owner.RequestRespawnServerRpc();
        owner.RequestSetColliderServerRpc(true);
        owner.RequestSetSpritesServerRpc(true);
        owner.SetState(PlayerStateMachine.State.Spawn);
    }
}
