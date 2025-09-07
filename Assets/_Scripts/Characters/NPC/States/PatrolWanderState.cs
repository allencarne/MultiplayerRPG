using System.Collections.Generic;
using UnityEngine;

public class PatrolWanderState : NPCState
{
    [SerializeField] float walkSpeed;
    [SerializeField] List<Vector2> patrolPoints;
    int currentIndex = 0;
    bool goingForward = true;

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
        if (Vector2.Distance(owner.transform.position, patrolPoints[currentIndex]) <= 0.1f)
        {
            AdvancePatrolIndex();
        }
    }

    public override void FixedUpdateState(NPCStateMachine owner)
    {
        if (owner.CrowdControl.immobilize.IsImmobilized) return;

        Vector2 target = patrolPoints[currentIndex];
        Vector2 direction = (target - (Vector2)owner.transform.position).normalized;
        owner.NpcRB.linearVelocity = direction * walkSpeed;

        owner.SwordAnimator.SetFloat("Horizontal", direction.x);
        owner.SwordAnimator.SetFloat("Vertical", direction.y);

        owner.BodyAnimator.SetFloat("Horizontal", direction.x);
        owner.BodyAnimator.SetFloat("Vertical", direction.y);

        owner.EyesAnimator.SetFloat("Horizontal", direction.x);
        owner.EyesAnimator.SetFloat("Vertical", direction.y);

        owner.HairAnimator.SetFloat("Horizontal", direction.x);
        owner.HairAnimator.SetFloat("Vertical", direction.y);
    }

    private void AdvancePatrolIndex()
    {
        if (goingForward)
        {
            currentIndex++;
            if (currentIndex >= patrolPoints.Count)
            {
                currentIndex = patrolPoints.Count - 2; // step back
                goingForward = false;
            }
        }
        else
        {
            currentIndex--;
            if (currentIndex < 0)
            {
                currentIndex = 1; // step forward
                goingForward = true;
            }
        }
    }
}
