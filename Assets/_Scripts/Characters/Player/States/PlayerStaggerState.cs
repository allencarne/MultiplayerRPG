
using System.Collections;
using UnityEngine;

public class PlayerStaggerState : PlayerState
{
    public override void StartState(PlayerStateMachine owner)
    {
        if (!owner.IsOwner) return;

        // Stop all animations
        owner.PlayerHeadAnimator.speed = 0;
        owner.BodyAnimator.speed = 0;
        owner.ChestAnimator.speed = 0;
        owner.LegsAnimator.speed = 0;
        owner.WeaponAnimator.speed = 0;

        // Player Spawn Animation to Hide Clothes
        owner.ChestAnimator.Play("Spawn");
        owner.LegsAnimator.Play("Spawn");
    }

    public override void UpdateState(PlayerStateMachine owner)
    {
        // Check if the player is the owner and not dead
        if (!owner.IsOwner) return;
        if (owner.Stats.isDead) return;

        // Check if the player is no longer crowd controlled
        if (!owner.CrowdControl.IsCrowdControlled)
        {
            // Resume all animations
            owner.PlayerHeadAnimator.speed = 1;
            owner.BodyAnimator.speed = 1;
            owner.ChestAnimator.speed = 1;
            owner.LegsAnimator.speed = 1;
            owner.WeaponAnimator.speed = 1;

            // Transition to Idle state
            owner.SetState(PlayerStateMachine.State.Idle);
        }
    }

    public override void FixedUpdateState(PlayerStateMachine owner)
    {

    }
}
