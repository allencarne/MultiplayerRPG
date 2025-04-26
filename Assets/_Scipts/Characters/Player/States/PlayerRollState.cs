using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerRollState : PlayerState
{
    Vector2 facingDirection;
    float rollDuration = .6f;

    public override void StartState(PlayerStateMachine owner)
    {
        owner.player.EnduranceBar.SpendEndurance(50);

        owner.StartCoroutine(Duration(owner));

        owner.Buffs.Immunity(rollDuration);
        owner.Buffs.Immoveable(rollDuration);

        Vector2 moveInput = owner.InputHandler.MoveInput.normalized;

        if (moveInput == Vector2.zero)
        {
            // get Body animator horizontal and vertical float
            float _x = owner.BodyAnimator.GetFloat("Horizontal");
            float _y = owner.BodyAnimator.GetFloat("Vertical");

            // create new vector 2
            Vector2 _newDir = new Vector2(_x, _y);

            // Apply force
            owner.PlayerRB.AddForce(_newDir * 25, ForceMode2D.Impulse);

            facingDirection = owner.SnapDirection(_newDir);

        }
        else
        {
            // Apply force
            owner.PlayerRB.AddForce(moveInput * 25, ForceMode2D.Impulse);

            facingDirection = owner.SnapDirection(moveInput);
        }

        // Animate
        owner.BodyAnimator.SetFloat("Horizontal", facingDirection.x);
        owner.BodyAnimator.SetFloat("Vertical", facingDirection.y);
        owner.BodyAnimator.Play("Roll");

        owner.EyesAnimator.SetFloat("Horizontal", facingDirection.x);
        owner.EyesAnimator.SetFloat("Vertical", facingDirection.y);
        owner.EyesAnimator.Play("Roll");

        owner.SwordAnimator.SetFloat("Horizontal", facingDirection.x);
        owner.SwordAnimator.SetFloat("Vertical", facingDirection.y);
        owner.SwordAnimator.Play("Roll");

        owner.HairAnimator.SetFloat("Horizontal", facingDirection.x);
        owner.HairAnimator.SetFloat("Vertical", facingDirection.y);
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
        yield return new WaitForSeconds(rollDuration);

        // Stop Moving
        owner.PlayerRB.linearVelocity = Vector2.zero;

        // Transition
        owner.SetState(PlayerStateMachine.State.Idle);
    }


}
