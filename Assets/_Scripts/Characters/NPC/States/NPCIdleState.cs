using UnityEngine;

public class NPCIdleState : NPCState
{
    public override void StartState(NPCStateMachine owner)
    {
        owner.SwordAnimator.Play("Idle");
        owner.BodyAnimator.Play("Idle");
        owner.EyesAnimator.Play("Idle");
        owner.HairAnimator.Play("Idle_" + owner.npc.hairIndex);
        owner.HeadAnimator.Play("Idle_" + owner.npc.HeadIndex);
        owner.ChestAnimator.Play("Idle_" + owner.npc.ChestIndex);
        owner.LegsAnimator.Play("Idle_" + owner.npc.LegsIndex);
    }

    public override void UpdateState(NPCStateMachine owner)
    {
        if (owner.IsEnemyInRange)
        {
            owner.SetState(NPCStateMachine.State.Chase);
        }
    }

    public override void FixedUpdateState(NPCStateMachine owner)
    {

    }
}
