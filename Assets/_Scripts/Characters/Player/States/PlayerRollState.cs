using System.Collections;
using UnityEngine;

public class PlayerRollState : PlayerState
{
    public PlayerRollState(PlayerStateMachine owner) : base(owner) { }

    Vector2 facingDirection;
    float rollDuration = .6f;

    public override void EnterState()
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
            float _x = owner.Animator.BodyAnimator.GetFloat("Horizontal");
            float _y = owner.Animator.BodyAnimator.GetFloat("Vertical");
            Vector2 _newDir = new Vector2(_x, _y);

            // Add Force
            owner.RigidBody2D.AddForce(_newDir * 25, ForceMode2D.Impulse);

            // Snap direction and set head sprites
            facingDirection = owner.Animator.SnapDirection(_newDir);
            owner.playerHead.SetHead(facingDirection);
        }
        else // Roll in the direction of input
        {
            // Add Force
            owner.RigidBody2D.AddForce(moveInput * 25, ForceMode2D.Impulse);

            // Snap direction and set head sprites
            facingDirection = owner.Animator.SnapDirection(moveInput);

            owner.playerHead.SetHead(facingDirection);
        }

        owner.Animator.PlayAnimation("Roll", owner.customization.net_ChestIndex.Value, owner.customization.net_LegsIndex.Value, owner.customization.WeaponAnimType);
        owner.Animator.SetDirection(facingDirection);
    }

    public override void UpdateState()
    {
        owner.OffensiveAbility();
        owner.MobilityAbility();
        owner.DefensiveAbility();
        owner.UtilityAbility();
        owner.UltimateAbility();
    }

    IEnumerator Duration(PlayerStateMachine owner)
    {
        yield return new WaitForSeconds(rollDuration);
        owner.RigidBody2D.linearVelocity = Vector2.zero;
        owner.SetState(new PlayerIdleState(owner));
    }
}
