using UnityEngine;

public class EnemyResetState : EnemyState
{
    public override void StartState(EnemyStateMachine owner)
    {
        owner.EnemyAnimator.Play("Wander");
    }

    public override void UpdateState(EnemyStateMachine owner)
    {
        owner.enemy.UpdatePatienceBar(owner.enemy.CurrentPatience);

        if (Vector2.Distance(transform.position, owner.StartingPosition) <= 0.1f)
        {
            owner.EnemyRB.linearVelocity = Vector2.zero;
            owner.SetState(EnemyStateMachine.State.Idle);
        }
    }

    public override void FixedUpdateState(EnemyStateMachine owner)
    {
        owner.MoveTowardsTarget(owner.StartingPosition);

        Vector2 direction = (owner.StartingPosition - (Vector2)owner.transform.position).normalized;
        owner.EnemyAnimator.SetFloat("Horizontal", direction.x);
        owner.EnemyAnimator.SetFloat("Vertical", direction.y);
    }
}
