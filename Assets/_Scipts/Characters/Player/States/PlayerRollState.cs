using System.Collections;
using UnityEngine;

public class PlayerRollState : PlayerState
{
    public override void StartState(PlayerStateMachine owner)
    {
        owner.player.EnduranceBar.SpendEndurance(50);

        owner.StartCoroutine(Duration(owner));

        Vector2 moveInput = owner.InputHandler.MoveInput.normalized;
        Vector2 direction;
        if (moveInput != Vector2.zero)
        {
            direction = moveInput;
        }
        else
        {
            // Fall back to the direction the player is currently facing
            float horizontal = owner.BodyAnimator.GetFloat("Horizontal");
            float vertical = owner.BodyAnimator.GetFloat("Vertical");
            direction = new Vector2(horizontal, vertical).normalized;

            // default to right if no direction
            if (direction == Vector2.zero)
            {
                direction = Vector2.right;
            }
        }

        // Apply force
        owner.PlayerRB.AddForce(direction * 25, ForceMode2D.Impulse);

        // Animate
        owner.BodyAnimator.SetFloat("Horizontal", direction.x);
        owner.BodyAnimator.SetFloat("Vertical", direction.y);
        owner.BodyAnimator.Play("Roll");
    }

    public override void UpdateState(PlayerStateMachine owner)
    {

    }

    public override void FixedUpdateState(PlayerStateMachine owner)
    {

    }

    IEnumerator Duration(PlayerStateMachine owner)
    {
        yield return new WaitForSeconds(.6f);

        // Stop Moving
        owner.PlayerRB.linearVelocity = Vector2.zero;

        // Transition
        owner.SetState(PlayerStateMachine.State.Idle);
    }


}
