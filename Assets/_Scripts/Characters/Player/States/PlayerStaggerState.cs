using UnityEngine;

public class PlayerStaggerState : PlayerState
{
    public override void StartState(PlayerStateMachine owner)
    {
        if (!owner.IsOwner) return;

        owner.CrowdControl.interrupt.Interrupt();
        owner.BodyAnimator.Play("Stagger");
    }

    public override void UpdateState(PlayerStateMachine owner)
    {
        if (!owner.IsOwner) return;
        if (owner.Stats.net_CurrentHealth.Value <= 0) return;

        if (!owner.CrowdControl.knockBack.IsKnockedBack &&
            !owner.CrowdControl.stun.IsStunned &&
            !owner.CrowdControl.knockUp.IsKnockedUp &&
            !owner.CrowdControl.pull.IsPulled)
        {
            owner.SetState(PlayerStateMachine.State.Idle);
        }
    }

    public override void FixedUpdateState(PlayerStateMachine owner)
    {

    }
}
