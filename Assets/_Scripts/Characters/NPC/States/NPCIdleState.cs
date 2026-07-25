using UnityEngine;

public class NPCIdleState : NPCState
{
    public override void StartState(NPCStateMachine owner)
    {
        // Face Down
        Vector2 dir = new Vector2 (0, -1);
        owner.SetAnimDir(dir);
        owner.npc.npcHead.SetEyes(dir);
        owner.npc.npcHead.SetHair(dir);
        owner.npc.npcHead.SetHelm(dir);

        // Play Animation
        owner.HeadAnimator.Play("Idle", -1, 0);
        owner.BodyAnimator.Play("Idle", -1, 0);
        owner.ChestAnimator.Play("Idle_" + owner.npc.Data.ChestIndex, -1, 0);
        owner.LegsAnimator.Play("Idle_" + owner.npc.Data.LegsIndex, -1, 0);
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
