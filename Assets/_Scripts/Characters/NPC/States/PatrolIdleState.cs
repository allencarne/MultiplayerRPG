using UnityEngine;

public class PatrolIdleState : NPCState
{
    public override void StartState(NPCStateMachine owner)
    {
        owner.HeadAnimator.Play("Run");
        owner.BodyAnimator.Play("Run");

        owner.ChestAnimator.Play("Run_" + owner.npc.Data.ChestIndex);
        owner.LegsAnimator.Play("Run_" + owner.npc.Data.LegsIndex);

        owner.SwordAnimator.Play(owner.npc.Data.WeaponType.ToString() + " Run");
    }

    public override void UpdateState(NPCStateMachine owner)
    {
        if (owner.IsEnemyInRange)
        {
            owner.SetState(NPCStateMachine.State.Chase);
            return;
        }

        Vector2 target = owner.npc.Data.waypoints[owner.PatrolIndex];
        if (Vector2.Distance(owner.transform.position, target) <= 0.1f)
        {
            AdvancePatrolIndex(owner, owner.npc.Data.waypoints.Length);
        }
    }

    public override void FixedUpdateState(NPCStateMachine owner)
    {
        if (owner.CrowdControl.immobilize.IsImmobilized) return;

        Vector2 target = owner.npc.Data.waypoints[owner.PatrolIndex];
        owner.MoveTowardsTarget(target);

        Vector2 rawDir = (target - (Vector2)owner.transform.position).normalized;
        Vector2 snappedDir = owner.SnapDirection(rawDir);
        owner.SetAnimDir(snappedDir);

        owner.npc.npcHead.SetEyes(snappedDir);
        owner.npc.npcHead.SetHair(snappedDir);
        owner.npc.npcHead.SetHelm(snappedDir);
    }

    private void AdvancePatrolIndex(NPCStateMachine owner, int length)
    {
        if (owner.npc.Data.PatrolForward)
        {
            owner.PatrolIndex++;
            if (owner.PatrolIndex >= length) owner.PatrolIndex = 0;
        }
        else
        {
            owner.PatrolIndex--;
            if (owner.PatrolIndex < 0) owner.PatrolIndex = length - 1;
        }
    }
}
