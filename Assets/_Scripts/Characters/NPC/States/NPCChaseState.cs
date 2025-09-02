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
}
