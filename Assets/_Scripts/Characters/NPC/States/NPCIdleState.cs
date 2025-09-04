using UnityEngine;

public class NPCIdleState : NPCState
{
    public override void StartState(NPCStateMachine owner)
    {
        owner.SwordAnimator.Play("Idle");
        owner.BodyAnimator.Play("Idle");
        owner.EyesAnimator.Play("Idle");
        owner.HairAnimator.Play("Idle_" + owner.npc.hairIndex);
    }

    public override void UpdateState(NPCStateMachine owner)
    {

    }

    public override void FixedUpdateState(NPCStateMachine owner)
    {

    }
}
