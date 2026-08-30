using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(PlayerStateMachine owner) : base(owner) { }

    public override void EnterState()
    {
        owner.Animator.PlayAnimation("Idle", owner.customization.net_ChestIndex.Value, owner.customization.net_LegsIndex.Value, owner.customization.WeaponAnimType);

        // Set animator parameters to match the player's facing direction
        //owner.PlayerHeadAnimator.Play("Idle", -1, 0);
        //owner.BodyAnimator.Play("Idle", -1, 0);
        //owner.ChestAnimator.Play("Idle_" + owner.customization.net_ChestIndex.Value, -1, 0);
        //owner.LegsAnimator.Play("Idle_" + owner.customization.net_LegsIndex.Value, -1, 0);
        //if (owner.Equipment.IsWeaponEquipped) owner.WeaponAnimator.Play(owner.customization.WeaponAnimType + " Idle", -1, 0);
    }

    public override void UpdateState()
    {
        owner.Roll();
        owner.BasicAbility();
        owner.OffensiveAbility();
        owner.MobilityAbility();
        owner.DefensiveAbility();
        owner.UtilityAbility();
        owner.UltimateAbility();
    }

    public override void FixedUpdateState()
    {
        // Transition to Move State
        if (owner.Input.MoveInput != Vector2.zero)
        {
            if (!owner.CrowdControl.immobilize.IsImmobilized)
            {
                owner.SetState(new PlayerRunState(owner));
            }
        }
    }
}
