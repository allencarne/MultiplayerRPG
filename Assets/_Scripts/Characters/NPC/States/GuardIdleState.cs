using UnityEngine;

public class GuardIdleState : NPCState
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
        if (!owner.IsServer) return;

        // Transition To Chase
        if (owner.IsEnemyInRange)
        {
            owner.SetState(NPCStateMachine.State.Chase);
        }
    }

    public override void FixedUpdateState(NPCStateMachine owner)
    {

    }
}
