
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
        owner.SwordAnimator.Play(owner.npc.Data.WeaponType + " Idle", -1, 0);
    }

    public override void UpdateState(NPCStateMachine owner)
    {
        if (!owner.IsServer) return;
        if (owner.npc.stats.isDead) return;

        if (!owner.CrowdControl.IsCrowdControlled)
        {
            owner.HeadAnimator.speed = 1;
            owner.BodyAnimator.speed = 1;
            owner.ChestAnimator.speed = 1;
            owner.LegsAnimator.speed = 1;
            owner.SwordAnimator.speed = 1;

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
