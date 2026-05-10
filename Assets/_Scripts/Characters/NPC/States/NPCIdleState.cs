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



        owner.HeadAnimator.Play("Idle", -1, 0);
        owner.BodyAnimator.Play("Idle", -1, 0);

        owner.ChestAnimator.Play("Idle_" + owner.npc.Data.ChestIndex, -1, 0);
        owner.LegsAnimator.Play("Idle_" + owner.npc.Data.LegsIndex, -1, 0);

        // Add Idle Index
        owner.SwordAnimator.Play(owner.npc.Data.WeaponType.ToString() + " Idle", -1, 0);
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
