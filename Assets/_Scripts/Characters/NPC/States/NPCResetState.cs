using UnityEngine;

public class NPCResetState : NPCState
{
    public override void StartState(NPCStateMachine owner)
    {
        owner.SwordAnimator.Play("Run");
        owner.BodyAnimator.Play("Run");
        owner.EyesAnimator.Play("Run");
        owner.HairAnimator.Play("Run_" + owner.npc.hairIndex);

        if (owner.IsServer)
        {
            owner.npc.PatienceBar.Patience.Value = 0;
        }
    }

    public override void UpdateState(NPCStateMachine owner)
    {
        if (!owner.IsServer) return;

        if (Vector2.Distance(transform.position, owner.StartingPosition) <= 0.1f)
        {
            owner.NpcRB.linearVelocity = Vector2.zero;
            owner.SetState(NPCStateMachine.State.Idle);
        }
    }

    public override void FixedUpdateState(NPCStateMachine owner)
    {
        if (owner.IsServer)
        {
            owner.MoveTowardsTarget(owner.StartingPosition);
        }

        Vector2 direction = (owner.StartingPosition - (Vector2)owner.transform.position).normalized;
        owner.BodyAnimator.SetFloat("Horizontal", direction.x);
        owner.BodyAnimator.SetFloat("Vertical", direction.y);

        owner.EyesAnimator.SetFloat("Horizontal", direction.x);
        owner.EyesAnimator.SetFloat("Vertical", direction.y);

        owner.HairAnimator.SetFloat("Horizontal", direction.x);
        owner.HairAnimator.SetFloat("Vertical", direction.y);
    }
}
