using UnityEngine;

public class NPCHurtState : NPCState
{
    public override void StartState(NPCStateMachine owner)
    {
        owner.CrowdControl.IsInterrupted = true;
        owner.BodyAnimator.Play("Hurt");
    }

    public override void UpdateState(NPCStateMachine owner)
    {
        if (!owner.IsOwner) return;

        owner.HandlePotentialInterrupt();

        if (!owner.CrowdControl.knockBack.IsKnockedBack &&
            !owner.CrowdControl.stun.IsStunned &&
            !owner.CrowdControl.knockUp.IsKnockedUp &&
            !owner.CrowdControl.pull.IsPulled)
        {
            owner.CrowdControl.IsInterrupted = false;
            owner.SetState(NPCStateMachine.State.Idle);
        }
    }

    public override void FixedUpdateState(NPCStateMachine owner)
    {

    }
}
