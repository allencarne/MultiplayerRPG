using UnityEngine;

public class PlayerHurtState : PlayerState
{
    public override void StartState(PlayerStateMachine owner)
    {
        if (!owner.IsOwner) return;

        owner.CrowdControl.interrupt.CanInterrupt = true;
        owner.BodyAnimator.Play("Hurt");
    }

    public override void UpdateState(PlayerStateMachine owner)
    {
        if (!owner.IsOwner) return;

        owner.HandlePotentialInterrupt();

        if (!owner.CrowdControl.knockBack.IsKnockedBack &&
            !owner.CrowdControl.stun.IsStunned &&
            !owner.CrowdControl.knockUp.IsKnockedUp &&
            !owner.CrowdControl.pull.IsPulled)
        {
            owner.CrowdControl.interrupt.CanInterrupt = false;
            owner.SetState(PlayerStateMachine.State.Idle);
        }
    }

    public override void FixedUpdateState(PlayerStateMachine owner)
    {

    }
}
