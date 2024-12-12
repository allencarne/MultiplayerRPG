using System.Collections;
using UnityEngine;

public class PlayerRollState : PlayerState
{
    public PlayerRollState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Start()
    {
        stateMachine.StartCoroutine(Duration());

        // Check if there is any input from the player
        Vector2 moveInput = stateMachine.InputHandler.MoveInput.normalized;

        Vector2 direction;

        if (moveInput != Vector2.zero)
        {
            // Use MoveInput direction if available
            direction = moveInput;
        }
        else
        {
            // Fall back to the direction the player is currently facing
            float horizontal = stateMachine.BodyAnimator.GetFloat("Horizontal");
            float vertical = stateMachine.BodyAnimator.GetFloat("Vertical");
            direction = new Vector2(horizontal, vertical).normalized;

            // Ensure there's a valid direction; default to forward if none
            if (direction == Vector2.zero)
            {
                direction = Vector2.up; // Default direction, adjust based on your game design
            }
        }

        // Apply force in the determined direction
        stateMachine.Rigidbody.AddForce(direction * 25, ForceMode2D.Impulse);

        // Update the animator floats to reflect the roll direction
        stateMachine.BodyAnimator.SetFloat("Horizontal", direction.x);
        stateMachine.BodyAnimator.SetFloat("Vertical", direction.y);

        // Play the roll animation
        stateMachine.BodyAnimator.Play("Roll");
    }

    public override void Update()
    {
        
    }

    public override void FixedUpdate()
    {
        
    }

    IEnumerator Duration()
    {
        yield return new WaitForSeconds(.6f);

        // Stop the Rigidbody's movement after the roll ends
        stateMachine.Rigidbody.linearVelocity = Vector2.zero;

        // Transition to the idle state
        stateMachine.SetState(new PlayerIdleState(stateMachine));
    }
}
