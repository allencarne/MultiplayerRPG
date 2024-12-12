using System.Collections;
using UnityEngine;

public class PlayerRollState : PlayerState
{
    public PlayerRollState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Start()
    {
        stateMachine.StartCoroutine(Duration());

        Vector2 moveInput = stateMachine.InputHandler.MoveInput.normalized;
        Vector2 direction;
        if (moveInput != Vector2.zero)
        {
            direction = moveInput;
        }
        else
        {
            // Fall back to the direction the player is currently facing
            float horizontal = stateMachine.BodyAnimator.GetFloat("Horizontal");
            float vertical = stateMachine.BodyAnimator.GetFloat("Vertical");
            direction = new Vector2(horizontal, vertical).normalized;

            // default to right if no direction
            if (direction == Vector2.zero)
            {
                direction = Vector2.right;
            }
        }

        // Apply force
        stateMachine.Rigidbody.AddForce(direction * 25, ForceMode2D.Impulse);

        // Animate
        stateMachine.BodyAnimator.SetFloat("Horizontal", direction.x);
        stateMachine.BodyAnimator.SetFloat("Vertical", direction.y);
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

        // Stop Moving
        stateMachine.Rigidbody.linearVelocity = Vector2.zero;

        // Transition
        stateMachine.SetState(new PlayerIdleState(stateMachine));
    }
}
