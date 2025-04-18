using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerRollState : PlayerState
{
    public override void StartState(PlayerStateMachine owner)
    {
        owner.player.EnduranceBar.SpendEndurance(50);

        owner.StartCoroutine(Duration(owner));

        Vector2 moveInput = owner.InputHandler.MoveInput.normalized;
        Vector2 direction = owner.SnapDirection(moveInput);

        // Apply force
        owner.PlayerRB.AddForce(moveInput * 25, ForceMode2D.Impulse);

        // Animate
        owner.BodyAnimator.SetFloat("Horizontal", direction.x);
        owner.BodyAnimator.SetFloat("Vertical", direction.y);
        owner.BodyAnimator.Play("Roll");

        owner.EyesAnimator.SetFloat("Horizontal", direction.x);
        owner.EyesAnimator.SetFloat("Vertical", direction.y);
        owner.EyesAnimator.Play("Roll");

        owner.SwordAnimator.SetFloat("Horizontal", direction.x);
        owner.SwordAnimator.SetFloat("Vertical", direction.y);
        owner.SwordAnimator.Play("Roll");

        owner.HairAnimator.SetFloat("Horizontal", direction.x);
        owner.HairAnimator.SetFloat("Vertical", direction.y);
        owner.HairAnimator.Play("Roll_" + owner.player.hairIndex);
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
