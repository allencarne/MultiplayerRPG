using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public override void StartState(PlayerStateMachine owner)
    {
        owner.PlayerHeadAnimator.Play("Idle", -1, 0);
        owner.BodyAnimator.Play("Idle", -1, 0);

        owner.ChestAnimator.Play("Idle_" + owner.customization.net_ChestIndex.Value, -1, 0);
        owner.LegsAnimator.Play("Idle_" + owner.customization.net_LegsIndex.Value, -1, 0);

        owner.SwordAnimator.Play("Idle", -1, 0);
    }

    public override void UpdateState(PlayerStateMachine owner)
    {
        owner.Roll();
        owner.BasicAbility();
        owner.OffensiveAbility();
        owner.MobilityAbility();
        owner.DefensiveAbility();
        owner.UtilityAbility();
        owner.UltimateAbility();
    }

    public override void FixedUpdateState(PlayerStateMachine owner)
    {
        // Transition to Move State
        if (owner.Input.MoveInput != Vector2.zero)
        {
            if (!owner.CrowdControl.immobilize.IsImmobilized)
            {
                owner.SetState(PlayerStateMachine.State.Run);
            }
        }
    }
}
