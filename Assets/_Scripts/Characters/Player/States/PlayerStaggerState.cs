
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
        owner.WeaponAnimator.Play(owner.customization.WeaponAnimType + " Idle", -1, 0);
    }

    public override void UpdateState(PlayerStateMachine owner)
    {
        if (!owner.IsOwner) return;
        if (owner.Stats.isDead) return;

        if (!owner.CrowdControl.IsCrowdControlled)
        {
            owner.PlayerHeadAnimator.speed = 1;
            owner.BodyAnimator.speed = 1;
            owner.ChestAnimator.speed = 1;
            owner.LegsAnimator.speed = 1;
            owner.WeaponAnimator.speed = 1;

            owner.SetState(PlayerStateMachine.State.Idle);
        }
    }

    public override void FixedUpdateState(PlayerStateMachine owner)
    {

    }
}
