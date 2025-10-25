using UnityEngine;

public class PatrolChaseState : NPCState
{
    Vector2 startingPosition;

    public override void StartState(NPCStateMachine owner)
    {
        if (!owner.IsServer) return;

        startingPosition = transform.position;

        owner.SwordAnimator.Play("Run");
        owner.BodyAnimator.Play("Run");
        owner.EyesAnimator.Play("Run");
        owner.HairAnimator.Play("Run_" + owner.npc.hairIndex);
        owner.HeadAnimator.Play("Run_" + owner.npc.HeadIndex);
        owner.ChestAnimator.Play("Run_" + owner.npc.ChestIndex);
        owner.LegsAnimator.Play("Run_" + owner.npc.LegsIndex);
    }

    public override void UpdateState(NPCStateMachine owner)
    {
        if (!owner.IsServer) return;

        if (owner.Target == null)
        {
            TransitionToIdle(owner);
            return;
        }

        HandleAttack(owner);
        HandleDeAggro(owner);
    }

    public override void FixedUpdateState(NPCStateMachine owner)
    {
        if (!owner.IsServer) return;

        if (owner.Target)
        {
            owner.MoveTowardsTarget(owner.Target.position);

            Vector2 rawDir = (owner.Target.position - owner.transform.position).normalized;
            Vector2 snappedDir = owner.SnapDirection(rawDir);
            owner.SetAnimDir(snappedDir);
        }
    }

    public void TransitionToIdle(NPCStateMachine owner)
    {
        owner.npc.PatienceBar.Patience.Value = 0;
        owner.IsEnemyInRange = false;
        owner.Target = null;
        owner.SetState(NPCStateMachine.State.Idle);
    }

    public void HandleDeAggro(NPCStateMachine owner)
    {
        float distanceToStartingPosition = Vector2.Distance(startingPosition, owner.Target.position);

        if (distanceToStartingPosition > owner.DeAggroRadius)
        {
            owner.npc.PatienceBar.Patience.Value += Time.deltaTime;
            if (owner.npc.PatienceBar.Patience.Value >= owner.npc.TotalPatience)
            {
                TransitionToIdle(owner);
            }
        }
        else
        {
            owner.npc.PatienceBar.Patience.Value = Mathf.Max(0, owner.npc.PatienceBar.Patience.Value - Time.deltaTime);
        }
    }

    public void HandleAttack(NPCStateMachine owner)
    {
        if (owner.IsAttacking) return;

        float distanceToTarget = Vector2.Distance(transform.position, owner.Target.position);

        if (distanceToTarget <= owner.BasicRadius)
        {
            if (owner.CanBasic && !owner.CrowdControl.disarm.IsDisarmed)
            {
                owner.SetState(NPCStateMachine.State.Basic);
                return;
            }
        }
    }
}
