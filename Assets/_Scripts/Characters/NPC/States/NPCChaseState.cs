using UnityEngine;

public class NPCChaseState : NPCState
{
    public override void StartState(NPCStateMachine owner)
    {
        owner.BodyAnimator.Play("Run");
        owner.EyesAnimator.Play("Run");
        owner.HairAnimator.Play("Run_" + owner.npc.hairIndex);
    }

    public override void UpdateState(NPCStateMachine owner)
    {
        if (!owner.IsServer) return;


        if (owner.Target == null)
        {
            TransitionToReset(owner);
            return;
        }

        //HandleAttack(owner);
        HandleDeAggro(owner);
    }

    public override void FixedUpdateState(NPCStateMachine owner)
    {
        if (!owner.IsServer) return;

        if (owner.Target)
        {
            owner.MoveTowardsTarget(owner.Target.position);

            Vector2 direction = (owner.Target.position - owner.transform.position).normalized;
            owner.BodyAnimator.SetFloat("Horizontal", direction.x);
            owner.BodyAnimator.SetFloat("Vertical", direction.y);
            owner.EyesAnimator.SetFloat("Horizontal", direction.x);
            owner.EyesAnimator.SetFloat("Vertical", direction.y);
            owner.HairAnimator.SetFloat("Horizontal", direction.x);
            owner.HairAnimator.SetFloat("Vertical", direction.y);
        }
    }

    public void TransitionToReset(NPCStateMachine owner)
    {
        owner.npc.PatienceBar.Patience.Value = 0;
        owner.IsEnemyInRange = false;
        owner.Target = null;
        owner.SetState(NPCStateMachine.State.Reset);
    }

    public void HandleDeAggro(NPCStateMachine owner)
    {
        float distanceToStartingPosition = Vector2.Distance(owner.StartingPosition, owner.Target.position);

        if (distanceToStartingPosition > owner.DeAggroRadius)
        {
            owner.npc.PatienceBar.Patience.Value += Time.deltaTime;
            if (owner.npc.PatienceBar.Patience.Value >= owner.npc.TotalPatience)
            {
                TransitionToReset(owner);
            }
        }
        else
        {
            owner.npc.PatienceBar.Patience.Value = Mathf.Max(0, owner.npc.PatienceBar.Patience.Value - Time.deltaTime);
        }
    }
}
