using System.Collections;
using UnityEngine;

public class EnemyHurtState : EnemyState
{
    public override void StartState(EnemyStateMachine owner)
    {
        owner.CrowdControl.IsInterrupted = true;
        owner.EnemyAnimator.Play("Hurt");
    }

    public override void UpdateState(EnemyStateMachine owner)
    {
        owner.HandlePotentialInterrupt();

        if (!owner.CrowdControl.knockBack.IsKnockedBack)
        {
            owner.CrowdControl.IsInterrupted = false;
            owner.SetState(EnemyStateMachine.State.Idle);
        }
    }

    public override void FixedUpdateState(EnemyStateMachine owner)
    {

    }
}
