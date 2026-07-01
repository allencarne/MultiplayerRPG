using UnityEngine;

public class PlayerRunState : PlayerState
{
    // We track the last non-zero direction to know which way to face when we stop moving
    private Vector2 lastDirection = Vector2.zero;

    public override void StartState(PlayerStateMachine owner)
    {
        // Play run animation on all animators
        owner.PlayerHeadAnimator.Play("Run", -1, 0);
        owner.BodyAnimator.Play("Run", -1, 0);
        owner.ChestAnimator.Play("Run_" + owner.customization.net_ChestIndex.Value, -1, 0);
        owner.LegsAnimator.Play("Run_" + owner.customization.net_LegsIndex.Value, -1, 0);

        if (owner.Equipment.IsWeaponEquipped)
        {
            owner.WeaponAnimator.Play(owner.customization.WeaponAnimType + " Run", -1, 0);
        }

        // Set last direction to zero so we update it immediately on the first frame
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

        // If we become immobilized, stop moving and switch to idle
        if (owner.CrowdControl.immobilize.IsImmobilized)
        {
            owner.SetState(PlayerStateMachine.State.Idle);
        }
    }

    public override void FixedUpdateState(PlayerStateMachine owner)
    {
        HandleMovement(owner, owner.Input.MoveInput);

        // If we stop giving movement input, switch to idle but keep facing the same direction
        if (owner.Input.MoveInput == Vector2.zero)
        {
            owner.SetState(PlayerStateMachine.State.Idle);
        }
    }

    void HandleMovement(PlayerStateMachine owner, Vector2 moveInput)
    {
        // Normalize input to prevent faster diagonal movement, then multiply by speed
        Vector2 movement = moveInput.normalized * owner.Stats.TotalSpeed;

        // Apply movement to Rigidbody
        owner.PlayerRB.linearVelocity = movement;

        // If we're moving, determine which direction to face for animation and update animators and facing direction
        if (movement != Vector2.zero)
        {
            // Get the animation direction based on input and control scheme
            Vector2 animDirection = GetAnimationDirection(moveInput, UsingGamepad(owner));

            // If the animation direction has changed, update animators and facing direction
            if (animDirection != lastDirection)
            {
                UpdateAllAnimators(owner, animDirection);
                lastDirection = animDirection;

                owner.customization.net_FacingDirection.Value = animDirection;
                owner.playerHead.SetHead(animDirection);
            }
        }
    }

    void UpdateAllAnimators(PlayerStateMachine owner, Vector2 direction)
    {
        owner.PlayerHeadAnimator.SetFloat("Horizontal", direction.x);
        owner.PlayerHeadAnimator.SetFloat("Vertical", direction.y);
        owner.BodyAnimator.SetFloat("Horizontal", direction.x);
        owner.BodyAnimator.SetFloat("Vertical", direction.y);
        owner.ChestAnimator.SetFloat("Horizontal", direction.x);
        owner.ChestAnimator.SetFloat("Vertical", direction.y);
        owner.LegsAnimator.SetFloat("Horizontal", direction.x);
        owner.LegsAnimator.SetFloat("Vertical", direction.y);
        owner.WeaponAnimator.SetFloat("Horizontal", direction.x);
        owner.WeaponAnimator.SetFloat("Vertical", direction.y);
    }

    Vector2 GetAnimationDirection(Vector2 input, bool isGamepad)
    {
        if (isGamepad)
        {
            // Standard 4-direction: whichever axis is dominant wins
            if (Mathf.Abs(input.x) >= Mathf.Abs(input.y))
                return new Vector2(Mathf.Sign(input.x), 0);
            else
                return new Vector2(0, Mathf.Sign(input.y));
        }
        else
        {
            // Keyboard: horizontal takes priority when both axes are held
            if (input.x != 0)
                return new Vector2(Mathf.Sign(input.x), 0);
            else
                return new Vector2(0, Mathf.Sign(input.y));
        }
    }

    private bool UsingGamepad(PlayerStateMachine owner)
    {
        return owner.playerInput != null && owner.playerInput.currentControlScheme == "Gamepad";
    }
}
