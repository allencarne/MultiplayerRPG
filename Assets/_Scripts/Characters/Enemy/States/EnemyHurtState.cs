using System.Collections;
using UnityEngine;

public class EnemyHurtState : EnemyState
{
    public override void StartState(EnemyStateMachine owner)
    {
        owner.isHurt = true;
        owner.EnemyAnimator.Play("Hurt");
    }

    public override void UpdateState(EnemyStateMachine owner)
    {
        if (!owner.CrowdControl.knockBack.IsKnockedBack && 
            !owner.CrowdControl.stun.IsStunned && 
            !owner.CrowdControl.knockUp.IsKnockedUp && 
            !owner.CrowdControl.pull.IsPulled)
        {
            owner.isHurt = false;
            owner.SetState(EnemyStateMachine.State.Idle);
        }
    }

    public override void FixedUpdateState(EnemyStateMachine owner)
    {

    }
}
