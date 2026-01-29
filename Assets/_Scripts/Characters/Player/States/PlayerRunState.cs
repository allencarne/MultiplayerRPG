using UnityEngine;

public class PlayerRunState : PlayerState
{
    private Vector2 lastDirection = Vector2.zero;
    private Vector2 lastRawInput = Vector2.zero;

    public override void StartState(PlayerStateMachine owner)
    {
        owner.PlayerHeadAnimator.Play("Run", -1, 0);
        owner.BodyAnimator.Play("Run", -1, 0);

        owner.ChestAnimator.Play("Run_" + owner.customization.net_ChestIndex.Value, -1, 0);
        owner.LegsAnimator.Play("Run_" + owner.customization.net_LegsIndex.Value, -1, 0);

        owner.SwordAnimator.Play("Run", -1, 0);

        lastDirection = Vector2.zero;
        lastRawInput = Vector2.zero;
    }

    public override void UpdateState(PlayerStateMachine owner)
    {
        // Transitions
        owner.Roll();
        owner.BasicAbility();
        owner.OffensiveAbility();
        owner.MobilityAbility();
        owner.DefensiveAbility();
        owner.UtilityAbility();
        owner.UltimateAbility();

        // Leave if we are Immobilized
        if (owner.CrowdControl.immobilize.IsImmobilized)
        {
            owner.SetState(PlayerStateMachine.State.Idle);
        }
    }

    public override void FixedUpdateState(PlayerStateMachine owner)
    {
        HandleMovement(owner, owner.Input.MoveInput);

        // Leave if no move input
        if (owner.Input.MoveInput == Vector2.zero)
        {
            owner.SetState(PlayerStateMachine.State.Idle);
        }
    }

    void HandleMovement(PlayerStateMachine owner, Vector2 moveInput)
    {
        Vector2 movement = moveInput.normalized * owner.Stats.TotalSpeed;
        owner.PlayerRB.linearVelocity = movement;

        if (movement != Vector2.zero)
        {
            Vector2 snappedDirection = SnapDirectionWithPriority(movement, moveInput);

            if (snappedDirection != lastDirection)
            {
                UpdateAllAnimators(owner, snappedDirection);
                lastDirection = snappedDirection;

                owner.customization.net_FacingDirection.Value = snappedDirection;

                owner.playerHead.SetEyes(snappedDirection);
                owner.playerHead.SetHair(snappedDirection);
                owner.playerHead.SetHelm(snappedDirection);
            }

            lastRawInput = moveInput;
        }
    }

    void UpdateAllAnimators(PlayerStateMachine owner, Vector2 direction)
    {
        owner.PlayerHeadAnimator.SetFloat("Horizontal", direction.x);
        owner.PlayerHeadAnimator.SetFloat("Vertical", direction.y);
        owner.BodyAnimator.SetFloat("Horizontal", direction.x);
        owner.BodyAnimator.SetFloat("Vertical", direction.y);

        owner.ChestAnimator.SetFloat("Horizontal", direction.x);
        owner.ChestAnimator.SetFloat("Vertical", direction.y);
        owner.LegsAnimator.SetFloat("Horizontal", direction.x);
        owner.LegsAnimator.SetFloat("Vertical", direction.y);

        owner.SwordAnimator.SetFloat("Horizontal", direction.x);
        owner.SwordAnimator.SetFloat("Vertical", direction.y);
    }

    Vector2 SnapDirectionWithPriority(Vector2 direction, Vector2 currentInput)
    {
        float absX = Mathf.Abs(direction.x);
        float absY = Mathf.Abs(direction.y);

        // If magnitudes are clearly different, use the larger one
        if (absX > absY + 0.01f)
        {
            return new Vector2(Mathf.Sign(direction.x), 0);
        }
        else if (absY > absX + 0.01f)
        {
            return new Vector2(0, Mathf.Sign(direction.y));
        }
        else
        {
            // Magnitudes are equal or very close - prioritize newest input
            // Check which axis was PRESSED (went from 0 to non-zero)
            bool xPressed = Mathf.Abs(lastRawInput.x) < 0.01f && Mathf.Abs(currentInput.x) > 0.01f;
            bool yPressed = Mathf.Abs(lastRawInput.y) < 0.01f && Mathf.Abs(currentInput.y) > 0.01f;

            if (xPressed && !yPressed)
            {
                // X was just pressed, prioritize horizontal
                return new Vector2(Mathf.Sign(direction.x), 0);
            }
            else if (yPressed && !xPressed)
            {
                // Y was just pressed, prioritize vertical
                return new Vector2(0, Mathf.Sign(direction.y));
            }
            else
            {
                // Both pressed simultaneously OR neither just pressed
                // Keep the current direction if we have one
                if (lastDirection != Vector2.zero)
                {
                    return lastDirection;
                }
                // Otherwise default to horizontal
                return new Vector2(Mathf.Sign(direction.x), 0);
            }
        }
    }
}
