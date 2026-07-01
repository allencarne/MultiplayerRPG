
public class NPCStaggerState : NPCState
{
    public override void StartState(NPCStateMachine owner)
    {
        if (!owner.IsServer) return;

        // Stop all animations
        owner.HeadAnimator.speed = 0;
        owner.BodyAnimator.speed = 0;
        owner.ChestAnimator.speed = 0;
        owner.LegsAnimator.speed = 0;
        owner.SwordAnimator.speed = 0;

        // Player Spawn Animation to Hide Clothes
        owner.ChestAnimator.Play("Spawn");
        owner.LegsAnimator.Play("Spawn");
    }

    public override void UpdateState(NPCStateMachine owner)
    {
        // Check if the owner is the server and if the NPC is dead
        if (!owner.IsServer) return;
        if (owner.npc.stats.isDead) return;

        // Check if the NPC is no longer crowd controlled
        if (!owner.CrowdControl.IsCrowdControlled)
        {
            // Resume all animations
            owner.HeadAnimator.speed = 1;
            owner.BodyAnimator.speed = 1;
            owner.ChestAnimator.speed = 1;
            owner.LegsAnimator.speed = 1;
            owner.SwordAnimator.speed = 1;

            //Transition to the appropriate state based on whether the NPC is resetting or not
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
