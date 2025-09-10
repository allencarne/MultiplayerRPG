using System.Collections.Generic;
using UnityEngine;

public class PatrolIdleState : NPCState
{
    [SerializeField] List<Transform> patrolPoints;

    public override void StartState(NPCStateMachine owner)
    {
        if (patrolPoints == null || patrolPoints.Count == 0)
        {
            Debug.LogWarning($"{owner.name} has no patrol points set!");
            owner.SetState(NPCStateMachine.State.Idle);
            return;
        }

        owner.SwordAnimator.Play("Run");
        owner.BodyAnimator.Play("Run");
        owner.EyesAnimator.Play("Run");
        owner.HairAnimator.Play("Run_" + owner.npc.hairIndex);
    }

    public override void UpdateState(NPCStateMachine owner)
    {
        // Transition To Chase
        if (owner.IsEnemyInRange)
        {
            owner.SetState(NPCStateMachine.State.Chase);
            return;
        }

        // If reached patrol point
        if (Vector2.Distance(owner.transform.position, patrolPoints[owner.PatrolIndex].position) <= 0.1f)
        {
            AdvancePatrolIndex(owner);
        }
    }

    public override void FixedUpdateState(NPCStateMachine owner)
    {
        if (owner.CrowdControl.immobilize.IsImmobilized) return;

        Vector2 target = patrolPoints[owner.PatrolIndex].position;

        owner.MoveTowardsTarget(target);

        Vector2 rawDir = (target - (Vector2)owner.transform.position).normalized;
        Vector2 snappedDir = owner.SnapDirection(rawDir);
        owner.SetAnimDir(snappedDir);
    }

    private void AdvancePatrolIndex(NPCStateMachine owner)
    {
        if (owner.PatrolForward)
        {
            owner.PatrolIndex++;
            if (owner.PatrolIndex >= patrolPoints.Count)
            {
                owner.PatrolIndex = patrolPoints.Count - 2; // step back
                owner.PatrolForward = false;
            }
        }
        else
        {
            owner.PatrolIndex--;
            if (owner.PatrolIndex < 0)
            {
                owner.PatrolIndex = 1; // step forward
                owner.PatrolForward = true;
            }
        }
    }
}
