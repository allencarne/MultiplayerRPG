using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerRollState : PlayerState
{
    Vector2 facingDirection;
    float rollDuration = .6f;

    public override void StartState(PlayerStateMachine owner)
    {
        // Buffs
        owner.Buffs.immune.StartImmune(rollDuration);
        owner.Buffs.immoveable.StartImmovable(rollDuration);

        // Endurance
        owner.EnduranceBar.SpendEndurance(50);

        // Roll
        owner.StartCoroutine(Duration(owner));
        Vector2 moveInput = owner.Input.MoveInput.normalized;
        if (moveInput == Vector2.zero)
        {
            float _x = owner.BodyAnimator.GetFloat("Horizontal");
            float _y = owner.BodyAnimator.GetFloat("Vertical");
            Vector2 _newDir = new Vector2(_x, _y);
            owner.PlayerRB.AddForce(_newDir * 25, ForceMode2D.Impulse);
            facingDirection = owner.SnapDirection(_newDir);
        }
        else
        {
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

        owner.HeadAnimator.SetFloat("Horizontal", facingDirection.x);
        owner.HeadAnimator.SetFloat("Vertical", facingDirection.y);
        //owner.HeadAnimator.Play("Roll_" + owner.player.hairIndex);

        owner.ChestAnimator.SetFloat("Horizontal", facingDirection.x);
        owner.ChestAnimator.SetFloat("Vertical", facingDirection.y);
        //owner.ChestAnimator.Play("Roll_" + owner.player.hairIndex);

        owner.LegsAnimator.SetFloat("Horizontal", facingDirection.x);
        owner.LegsAnimator.SetFloat("Vertical", facingDirection.y);
        //owner.LegsAnimator.Play("Roll_" + owner.player.hairIndex);
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
        owner.PlayerRB.linearVelocity = Vector2.zero;
        owner.SetState(PlayerStateMachine.State.Idle);
    }
}
