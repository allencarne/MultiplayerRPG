using UnityEngine;

public class PlayerRunState : PlayerState
{
    public PlayerRunState(PlayerStateMachine playerStateMachine) : base(playerStateMachine) { }

    public override void Update()
    {
        stateMachine.BodyAnimator.Play("Run");
        stateMachine.EyeAnimator.Play("Run");
        stateMachine.HairAnimator.Play("Run_" + stateMachine.Player.hairIndex);

        // Transitions
        stateMachine.BasicAbility(stateMachine.InputHandler.BasicAbilityInput);
        stateMachine.OffensiveAbility(stateMachine.InputHandler.OffensiveAbilityInput);
        stateMachine.MobilityAbility(stateMachine.InputHandler.MobilityAbilityInput);
    }

    public override void FixedUpdate()
    {
        HandleMovement(stateMachine.InputHandler.MoveInput);

        // If we are no longer moving - Transition to Idle State
        if (stateMachine.InputHandler.MoveInput == Vector2.zero)
        {
            stateMachine.SetState(new PlayerIdleState(stateMachine));
        }
    }

    void HandleMovement(Vector2 moveInput)
    {
        Vector2 movement = moveInput.normalized * stateMachine.Player.moveSpeed;
        stateMachine.Rigidbody.linearVelocity = movement;

        if (movement != Vector2.zero)
        {
            Vector2 snappedDirection = stateMachine.SnapDirection(movement);

            stateMachine.BodyAnimator.SetFloat("Horizontal", snappedDirection.x);
            stateMachine.BodyAnimator.SetFloat("Vertical", snappedDirection.y);

            stateMachine.EyeAnimator.SetFloat("Horizontal", snappedDirection.x);
            stateMachine.EyeAnimator.SetFloat("Vertical", snappedDirection.y);

            stateMachine.HairAnimator.SetFloat("Horizontal", snappedDirection.x);
            stateMachine.HairAnimator.SetFloat("Vertical", snappedDirection.y);
        }
    }
}