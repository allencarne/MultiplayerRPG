using UnityEngine;

public class PlayerRunState : PlayerState
{
    public override void StartState(PlayerStateMachine owner)
    {
        owner.SwordAnimator.Play("Run");
        owner.BodyAnimator.Play("Run");
        owner.EyesAnimator.Play("Run");
        owner.HairAnimator.Play("Run_" + owner.player.hairIndex);
        owner.HeadAnimator.Play(owner.Equipment.HeadIndex + "_Run");
        owner.ChestAnimator.Play(owner.Equipment.ChestIndex + "_Run");
        owner.LegsAnimator.Play(owner.Equipment.LegsIndex + "_Run");
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

        // Leave if we are Immobilized
        if (owner.CrowdControl.immobilize.IsImmobilized)
        {
            owner.SetState(PlayerStateMachine.State.Idle);
        }
    }

    public override void FixedUpdateState(PlayerStateMachine owner)
    {
        HandleMovement(owner, owner.Input.MoveInput);

        // Leave if no move input
        if (owner.Input.MoveInput == Vector2.zero)
        {
            owner.SetState(PlayerStateMachine.State.Idle);
        }
    }

    void HandleMovement(PlayerStateMachine owner, Vector2 moveInput)
    {
        Vector2 movement = moveInput.normalized * owner.player.CurrentSpeed.Value;
        owner.PlayerRB.linearVelocity = movement;

        if (movement != Vector2.zero)
        {
            Vector2 snappedDirection = owner.SnapDirection(movement);

            owner.SwordAnimator.SetFloat("Horizontal", snappedDirection.x);
            owner.SwordAnimator.SetFloat("Vertical", snappedDirection.y);

            owner.BodyAnimator.SetFloat("Horizontal", snappedDirection.x);
            owner.BodyAnimator.SetFloat("Vertical", snappedDirection.y);

            owner.EyesAnimator.SetFloat("Horizontal", snappedDirection.x);
            owner.EyesAnimator.SetFloat("Vertical", snappedDirection.y);

            owner.HairAnimator.SetFloat("Horizontal", snappedDirection.x);
            owner.HairAnimator.SetFloat("Vertical", snappedDirection.y);

            owner.HeadAnimator.SetFloat("Horizontal", snappedDirection.x);
            owner.HeadAnimator.SetFloat("Vertical", snappedDirection.y);

            owner.ChestAnimator.SetFloat("Horizontal", snappedDirection.x);
            owner.ChestAnimator.SetFloat("Vertical", snappedDirection.y);

            owner.LegsAnimator.SetFloat("Horizontal", snappedDirection.x);
            owner.LegsAnimator.SetFloat("Vertical", snappedDirection.y);
        }
    }
}
