using UnityEngine;

public class NPCIdleState : NPCState
{
    public override void StartState(NPCStateMachine owner)
    {

        owner.HeadAnimator.SetFloat("Vertical", -1);
        owner.BodyAnimator.SetFloat("Vertical", -1);

        owner.ChestAnimator.SetFloat("Vertical", -1);
        owner.LegsAnimator.SetFloat("Vertical", -1);

        owner.SwordAnimator.SetFloat("Vertical", -1);



        owner.HeadAnimator.Play("Idle");
        owner.BodyAnimator.Play("Idle");

        owner.ChestAnimator.Play("Idle_" + owner.npc.Data.ChestIndex);
        owner.LegsAnimator.Play("Idle_" + owner.npc.Data.LegsIndex);

        owner.SwordAnimator.Play("Idle");
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
