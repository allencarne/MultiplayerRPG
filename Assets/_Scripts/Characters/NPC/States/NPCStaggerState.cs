using UnityEngine;

public class NPCStaggerState : NPCState
{
    public override void StartState(NPCStateMachine owner)
    {
        if (!owner.IsServer) return;

        owner.CrowdControl.interrupt.Interrupt();
        owner.BodyAnimator.Play("Stagger");
    }

    public override void UpdateState(NPCStateMachine owner)
    {
        if (!owner.IsServer) return;

        if (owner.npc.stats.net_CurrentHP.Value <= 0) return;

        if (!owner.CrowdControl.knockBack.IsKnockedBack &&
            !owner.CrowdControl.stun.IsStunned &&
            !owner.CrowdControl.knockUp.IsKnockedUp &&
            !owner.CrowdControl.pull.IsPulled)
        {
            owner.SetState(NPCStateMachine.State.Idle);
        }
    }

    public override void FixedUpdateState(NPCStateMachine owner)
    {

    }
}
