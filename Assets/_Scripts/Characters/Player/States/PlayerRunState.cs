using UnityEngine;

public class PlayerRunState : PlayerState
{
    private Vector2 lastDirection = Vector2.zero;

    public override void StartState(PlayerStateMachine owner)
    {
        owner.SwordAnimator.Play("Run", -1, 0);
        owner.BodyAnimator.Play("Run", -1, 0);
        owner.EyesAnimator.Play("Run", -1, 0);
        owner.HairAnimator.Play("Run_" + owner.player.hairIndex, -1, 0);
        owner.HeadAnimator.Play("Run_" + owner.Equipment.HeadAnimIndex, -1, 0);
        owner.ChestAnimator.Play("Run_" + owner.Equipment.ChestAnimIndex, -1, 0);
        owner.LegsAnimator.Play("Run_" + owner.Equipment.LegsAnimIndex, -1, 0);


        lastDirection = Vector2.zero;
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
        Vector2 movement = moveInput.normalized * owner.Stats.Speed.Value;
        owner.PlayerRB.linearVelocity = movement;

        if (movement != Vector2.zero)
        {
            Vector2 snappedDirection = owner.SnapDirection(movement);

            if (snappedDirection != lastDirection)
            {
                UpdateAllAnimators(owner, snappedDirection);
                lastDirection = snappedDirection;
            }
        }
    }

    void UpdateAllAnimators(PlayerStateMachine owner, Vector2 direction)
    {
        owner.SwordAnimator.SetFloat("Horizontal", direction.x);
        owner.SwordAnimator.SetFloat("Vertical", direction.y);

        owner.BodyAnimator.SetFloat("Horizontal", direction.x);
        owner.BodyAnimator.SetFloat("Vertical", direction.y);
        owner.EyesAnimator.SetFloat("Horizontal", direction.x);
        owner.EyesAnimator.SetFloat("Vertical", direction.y);
        owner.HairAnimator.SetFloat("Horizontal", direction.x);
        owner.HairAnimator.SetFloat("Vertical", direction.y);

        owner.HeadAnimator.SetFloat("Horizontal", direction.x);
        owner.HeadAnimator.SetFloat("Vertical", direction.y);
        owner.ChestAnimator.SetFloat("Horizontal", direction.x);
        owner.ChestAnimator.SetFloat("Vertical", direction.y);
        owner.LegsAnimator.SetFloat("Horizontal", direction.x);
        owner.LegsAnimator.SetFloat("Vertical", direction.y);
    }
}
