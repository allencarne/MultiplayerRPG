using System.Collections;
using UnityEngine;

public class EnemyHurtState : EnemyState
{
    public override void StartState(EnemyStateMachine owner)
    {
        owner.CrowdControl.interrupt.Interrupt();
        owner.EnemyAnimator.Play("Stagger");
    }

    public override void UpdateState(EnemyStateMachine owner)
    {
        if (owner.enemy.Health.Value <= 0) return;

        if (!owner.CrowdControl.knockBack.IsKnockedBack && 
            !owner.CrowdControl.stun.IsStunned && 
            !owner.CrowdControl.knockUp.IsKnockedUp && 
            !owner.CrowdControl.pull.IsPulled)
        {
            owner.SetState(EnemyStateMachine.State.Idle);
        }
    }

    public override void FixedUpdateState(EnemyStateMachine owner)
    {

    }
}
