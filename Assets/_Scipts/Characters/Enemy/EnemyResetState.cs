using UnityEngine;

public class EnemyResetState : EnemyState
{
    public override void StartState(EnemyStateMachine owner)
    {
        owner.EnemyAnimator.Play("Wander");
    }

    public override void UpdateState(EnemyStateMachine owner)
    {
        owner.enemy.UpdatePatienceBar();

        if (Vector2.Distance(transform.position, owner.StartingPosition) <= 0.1f)
        {
            owner.EnemyRB.linearVelocity = Vector2.zero;
            owner.SetState(EnemyStateMachine.State.Idle);
        }
    }

    public override void FixedUpdateState(EnemyStateMachine owner)
    {
        owner.MoveTowardsTarget(owner.StartingPosition);
    }
}
