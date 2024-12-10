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
            stateMachine.BodyAnimator.SetFloat("Horizontal", movement.x);
            stateMachine.BodyAnimator.SetFloat("Vertical", movement.y);

            stateMachine.EyeAnimator.SetFloat("Horizontal", movement.x);
            stateMachine.EyeAnimator.SetFloat("Vertical", movement.y);

            stateMachine.HairAnimator.SetFloat("Horizontal", movement.x);
            stateMachine.HairAnimator.SetFloat("Vertical", movement.y);
        }
    }
}
