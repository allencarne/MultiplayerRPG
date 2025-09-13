using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public override void StartState(PlayerStateMachine owner)
    {
        owner.SwordAnimator.Play("Idle");
        owner.BodyAnimator.Play("Idle");
        owner.EyesAnimator.Play("Idle");
        owner.HairAnimator.Play("Idle_" + owner.player.hairIndex);
        owner.HeadAnimator.Play("Idle_" + owner.Equipment.HeadAnimIndex);
        owner.ChestAnimator.Play("Idle_" + owner.Equipment.ChestAnimIndex);
        owner.LegsAnimator.Play("Idle_" + owner.Equipment.LegsAnimIndex);
    }

    public override void UpdateState(PlayerStateMachine owner)
    {
        // Transitions
        owner.Roll();
        owner.BasicAbility();
        owner.OffensiveAbility();
        owner.MobilityAbility();
        owner.DefensiveAbility();
        owner.UtilityAbility();
        owner.UltimateAbility();
    }

    public override void FixedUpdateState(PlayerStateMachine owner)
    {
        // Transition to Move State
        if (owner.Input.MoveInput != Vector2.zero)
        {
            if (!owner.CrowdControl.immobilize.IsImmobilized)
            {
                owner.SetState(PlayerStateMachine.State.Run);
            }
        }
    }
}
