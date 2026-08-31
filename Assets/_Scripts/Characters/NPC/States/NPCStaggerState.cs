
public class NPCStaggerState : NPCState
{
    public NPCStaggerState(NPCStateMachine owner) : base(owner) { }

    public override void EnterState()
    {
        if (!owner.IsServer) return;

        // Stop all animations
        owner.Animator.PlayStaggerAnimation();
    }

    public override void UpdateState()
    {
        // Check if the owner is the server and if the NPC is dead
        if (!owner.IsServer) return;
        if (owner.npc.stats.isDead) return;

        // Check if the NPC is no longer crowd controlled
        if (!owner.CrowdControl.IsCrowdControlled)
        {
            // Resume all animations
            owner.Animator.EndStaggerAnimation();

            //Transition to the appropriate state based on whether the NPC is resetting or not
            if (owner.isResetting)
            {
                owner.SetState(new NPCResetState(owner));
            }
            else
            {
                owner.TransitionToIdle();
            }
        }
    }
}
