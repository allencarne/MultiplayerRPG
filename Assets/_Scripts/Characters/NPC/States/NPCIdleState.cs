using UnityEngine;

public class NPCIdleState : NPCState
{
    public NPCIdleState(NPCStateMachine owner) : base(owner) { }

    public override void EnterState()
    {
        // Face Down
        Vector2 dir = new Vector2(0, -1);
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

    public override void UpdateState()
    {
        if (owner.IsEnemyInRange)
        {
            owner.SetState(new NPCChaseState(owner));
        }
    }
}
