using UnityEngine;

public class NPCResetState : NPCState
{
    public override void StartState(NPCStateMachine owner)
    {
        owner.SwordAnimator.Play("Run");
        owner.BodyAnimator.Play("Run");
        owner.EyesAnimator.Play("Run");
        owner.HairAnimator.Play("Run_" + owner.npc.Data.hairIndex);
        owner.HeadAnimator.Play("Run_" + owner.npc.Data.HeadIndex);
        owner.ChestAnimator.Play("Run_" + owner.npc.Data.ChestIndex);
        owner.LegsAnimator.Play("Run_" + owner.npc.Data.LegsIndex);

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
            owner.BodyAnimator.SetFloat("Vertical", -1);
            owner.HairAnimator.SetFloat("Vertical", -1);
            owner.EyesAnimator.SetFloat("Vertical", -1);
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
    }
}
