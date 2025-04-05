using UnityEngine;

public class DummyIdleState : EnemyState
{
    public override void StartState(EnemyStateMachine owner)
    {
        owner.EnemyAnimator.Play("Idle");
    }

    public override void UpdateState(EnemyStateMachine owner)
    {
        owner.enemy.UpdatePatienceBar(owner.IdleTime);

        if (owner.EnemyRB.position != owner.StartingPosition)
        {
            owner.IdleTime += 1 * Time.deltaTime;
        }

        if (owner.IdleTime >= owner.enemy.TotalPatience)
        {
            owner.SetState(EnemyStateMachine.State.Reset);
        }
    }

    public override void FixedUpdateState(EnemyStateMachine owner)
    {

    }
}
