using UnityEngine;

public class PlayerRunState : PlayerState
{
    public PlayerRunState(PlayerStateMachine owner) : base(owner) { }

    // We track the last non-zero direction to know which way to face when we stop moving
    private Vector2 lastDirection = Vector2.zero;

    public override void EnterState()
    {
        owner.Animator.PlayAnimation("Run", owner.customization.net_ChestIndex.Value, owner.customization.net_LegsIndex.Value, owner.customization.WeaponAnimType);

        // Set last direction to zero so we update it immediately on the first frame
        lastDirection = Vector2.zero;
    }

    public override void UpdateState()
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
            owner.SetState(new PlayerIdleState(owner));
        }
    }

    public override void FixedUpdateState()
    {
        HandleMovement(owner, owner.Input.MoveInput);

        // If we stop giving movement input, switch to idle but keep facing the same direction
        if (owner.Input.MoveInput == Vector2.zero)
        {
            owner.SetState(new PlayerIdleState(owner));
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
            Vector2 animDirection = owner.Animator.GetAnimationDirection(moveInput, owner.Animator.UsingGamepad(owner));

            // If the animation direction has changed, update animators and facing direction
            if (animDirection != lastDirection)
            {
                owner.Animator.SetDirection(animDirection);
                lastDirection = animDirection;

                owner.customization.net_FacingDirection.Value = animDirection;
                owner.playerHead.SetHead(animDirection);
            }
        }
    }
}
