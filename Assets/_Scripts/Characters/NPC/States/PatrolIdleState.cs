using UnityEngine;

public class PatrolIdleState : NPCState
{
    public PatrolIdleState(NPCStateMachine owner) : base(owner) { }

    bool isPatrollingForward;

    public override void EnterState()
    {
        // Animate
        owner.Animator.PlayAnimation("Run", owner.npc.Data.ChestIndex, owner.npc.Data.LegsIndex, owner.npc.Data.WeaponType);

        //owner.HeadAnimator.Play("Run");
        //owner.BodyAnimator.Play("Run");
        //owner.ChestAnimator.Play("Run_" + owner.npc.Data.ChestIndex);
        //owner.LegsAnimator.Play("Run_" + owner.npc.Data.LegsIndex);
        //owner.SwordAnimator.Play(owner.npc.Data.WeaponType.ToString() + " Run");
    }

    public override void UpdateState()
    {
        // Check if an enemy is in range
        if (owner.IsEnemyInRange)
        {
            // If it is, then enter the chase state
            owner.TransitionToChase();
            return;
        }

        // Get the next waypoint
        Vector2 target = owner.npc.Data.waypoints[owner.PatrolIndex];

        // Check the distance from the NPC to the target waypoint
        if (Vector2.Distance(owner.transform.position, target) <= 0.1f)
        {
            // If we are close enough, advance to the next waypoint
            AdvancePatrolIndex(owner, owner.npc.Data.waypoints.Length);
        }
    }

    public override void FixedUpdateState()
    {
        if (owner.CrowdControl.immobilize.IsImmobilized) return;

        // Get the next waypoint
        Vector2 target = owner.npc.Data.waypoints[owner.PatrolIndex];

        // Move towards target waypoint
        owner.MoveTowardsTarget(target);

        // Get Raw Direction
        Vector2 rawDir = (target - (Vector2)owner.transform.position).normalized;

        // Get snapped direction for pixel snapping
        //Vector2 snappedDir = owner.SnapDirection(rawDir);
        Vector2 snappedDir = owner.Animator.SnapDirection(rawDir);

        // Set Animator Direction
        //owner.SetAnimDir(snappedDir);
        //owner.npc.npcHead.SetEyes(snappedDir);
        //owner.npc.npcHead.SetHair(snappedDir);
        //owner.npc.npcHead.SetHelm(snappedDir);

        owner.Animator.SetDirection(snappedDir);
        owner.npc.npcHead.SetHead(snappedDir);
    }

    private void AdvancePatrolIndex(NPCStateMachine owner, int length)
    {
        if (isPatrollingForward)
        {
            owner.PatrolIndex++;

            // Reached the end, turn around
            if (owner.PatrolIndex >= length)
            {
                owner.PatrolIndex = length - 2;
                isPatrollingForward = false;
            }
        }
        else
        {
            owner.PatrolIndex--;

            // Reached the beginning, turn around
            if (owner.PatrolIndex < 0)
            {
                owner.PatrolIndex = 1;
                isPatrollingForward = true;
            }
        }
    }
}
