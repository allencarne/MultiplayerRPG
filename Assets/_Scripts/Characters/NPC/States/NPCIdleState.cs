using UnityEngine;

public class NPCIdleState : NPCState
{
    public override void StartState(NPCStateMachine owner)
    {
        owner.SwordAnimator.Play("Idle");
        owner.BodyAnimator.Play("Idle");
        owner.EyesAnimator.Play("Idle");
        owner.HairAnimator.Play("Idle_" + owner.npc.Data.hairIndex);
        owner.HeadAnimator.Play("Idle_" + owner.npc.Data.HeadIndex);
        owner.ChestAnimator.Play("Idle_" + owner.npc.Data.ChestIndex);
        owner.LegsAnimator.Play("Idle_" + owner.npc.Data.LegsIndex);

        owner.SwordAnimator.SetFloat("Vertical", -1);
        owner.BodyAnimator.SetFloat("Vertical", -1);
        owner.EyesAnimator.SetFloat("Vertical", -1);
        owner.HairAnimator.SetFloat("Vertical", -1);
        owner.HeadAnimator.SetFloat("Vertical", -1);
        owner.ChestAnimator.SetFloat("Vertical", -1);
        owner.LegsAnimator.SetFloat("Vertical", -1);
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
