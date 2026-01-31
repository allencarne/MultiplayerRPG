
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
        if (owner.npc.stats.isDead) return;

        if (!owner.CrowdControl.knockBack.IsKnockedBack &&
            !owner.CrowdControl.stun.IsStunned &&
            !owner.CrowdControl.knockUp.IsKnockedUp &&
            !owner.CrowdControl.pull.IsPulled)
        {
            if (owner.isResetting)
            {
                owner.SetState(NPCStateMachine.State.Reset);
            }
            else
            {
                owner.SetState(NPCStateMachine.State.Idle);
            }
        }
    }

    public override void FixedUpdateState(NPCStateMachine owner)
    {

    }
}
