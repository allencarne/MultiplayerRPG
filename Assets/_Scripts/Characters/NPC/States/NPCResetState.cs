using UnityEngine;

public class NPCResetState : NPCState
{
    public override void StartState(NPCStateMachine owner)
    {
        owner.isResetting = true;

        owner.HeadAnimator.Play("Run");
        owner.BodyAnimator.Play("Run");
        owner.ChestAnimator.Play("Run_" + owner.npc.Data.ChestIndex);
        owner.LegsAnimator.Play("Run_" + owner.npc.Data.LegsIndex);
        owner.SwordAnimator.Play("Run");

        owner.npc.PatienceBar.Patience.Value = 0;

        if (owner.npc.stats.net_CurrentHP.Value < owner.npc.stats.net_TotalHP.Value)
        {
            owner.npc.IsRegen = true;
            owner.Buffs.regeneration.StartRegen(1, -1);
        }
    }

    public override void UpdateState(NPCStateMachine owner)
    {
        if (Vector2.Distance(transform.position, owner.StartingPosition) <= 0.1f)
        {
            owner.isResetting = false;

            owner.HeadAnimator.SetFloat("Vertical", -1);
            owner.BodyAnimator.SetFloat("Vertical", -1);

            owner.ChestAnimator.SetFloat("Vertical", -1);
            owner.LegsAnimator.SetFloat("Vertical", -1);

            owner.SwordAnimator.SetFloat("Vertical", -1);

            owner.NpcRB.linearVelocity = Vector2.zero;
            owner.SetState(NPCStateMachine.State.Idle);
        }
    }

    public override void FixedUpdateState(NPCStateMachine owner)
    {
        owner.MoveTowardsTarget(owner.StartingPosition);

        Vector2 direction = (owner.StartingPosition - (Vector2)owner.transform.position).normalized;
        Vector2 snappedDir = owner.SnapDirection(direction);
        owner.SetAnimDir(snappedDir);

        owner.npc.npcHead.SetEyes(snappedDir);
        owner.npc.npcHead.SetHair(snappedDir);
        owner.npc.npcHead.SetHelm(snappedDir);
    }
}
