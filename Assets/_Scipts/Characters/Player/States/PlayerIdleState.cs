using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public override void StartState(PlayerStateMachine owner)
    {
        owner.SwordAnimator.Play("Idle");
        owner.BodyAnimator.Play("Idle");
        owner.EyesAnimator.Play("Idle");
        owner.HairAnimator.Play("Idle_" + owner.player.hairIndex);
    }

    public override void UpdateState(PlayerStateMachine owner)
    {
        // Transitions
        owner.Roll(owner.InputHandler.RollInput);
        owner.BasicAbility(owner.InputHandler.BasicAbilityInput);
        owner.OffensiveAbility(owner.InputHandler.OffensiveAbilityInput);
        owner.MobilityAbility(owner.InputHandler.MobilityAbilityInput);
    }

    public override void FixedUpdateState(PlayerStateMachine owner)
    {
        // Transition to Move State
        if (owner.InputHandler.MoveInput != Vector2.zero)
        {
            owner.SetState(PlayerStateMachine.State.Run);
        }
    }
}
