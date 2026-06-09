using System.Collections;
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

        // Get input
        Vector2 moveInput = owner.Input.MoveInput.normalized;

        // If no input, roll in the direction we are facing
        if (moveInput == Vector2.zero)
        {
            // Get direction from animator (last direction we were moving)
            float _x = owner.BodyAnimator.GetFloat("Horizontal");
            float _y = owner.BodyAnimator.GetFloat("Vertical");
            Vector2 _newDir = new Vector2(_x, _y);

            // Add Force
            owner.PlayerRB.AddForce(_newDir * 25, ForceMode2D.Impulse);

            // Snap direction and set head sprites
            facingDirection = owner.SnapDirection(_newDir);
            owner.playerHead.SetEyes(facingDirection);
            owner.playerHead.SetHair(facingDirection);
            owner.playerHead.SetHelm(facingDirection);
        }
        // Roll in the direction of input
        else
        {
            // Add Force
            owner.PlayerRB.AddForce(moveInput * 25, ForceMode2D.Impulse);

            // Snap direction and set head sprites
            facingDirection = owner.SnapDirection(moveInput);
            owner.playerHead.SetEyes(facingDirection);
            owner.playerHead.SetHair(facingDirection);
            owner.playerHead.SetHelm(facingDirection);
        }

        // Head
        owner.PlayerHeadAnimator.SetFloat("Horizontal", facingDirection.x);
        owner.PlayerHeadAnimator.SetFloat("Vertical", facingDirection.y);
        owner.PlayerHeadAnimator.Play("Roll");

        // Body
        owner.BodyAnimator.SetFloat("Horizontal", facingDirection.x);
        owner.BodyAnimator.SetFloat("Vertical", facingDirection.y);
        owner.BodyAnimator.Play("Roll");

        // Chest
        owner.ChestAnimator.SetFloat("Horizontal", facingDirection.x);
        owner.ChestAnimator.SetFloat("Vertical", facingDirection.y);
        owner.ChestAnimator.Play("Roll_" + owner.customization.net_ChestIndex.Value);

        // Legs
        owner.LegsAnimator.SetFloat("Horizontal", facingDirection.x);
        owner.LegsAnimator.SetFloat("Vertical", facingDirection.y);
        owner.LegsAnimator.Play("Roll_" + owner.customization.net_LegsIndex.Value);

        // Weapon
        if (owner.Equipment.IsWeaponEquipped)
        {
            owner.WeaponAnimator.SetFloat("Horizontal", facingDirection.x);
            owner.WeaponAnimator.SetFloat("Vertical", facingDirection.y);
            owner.WeaponAnimator.Play(owner.customization.WeaponAnimType + " Roll");
        }
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
