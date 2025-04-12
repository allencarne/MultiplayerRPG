using UnityEngine;

public class DummyIdleState : EnemyState
{
    public override void StartState(EnemyStateMachine owner)
    {
        owner.EnemyAnimator.Play("Idle");
    }

    public override void UpdateState(EnemyStateMachine owner)
    {
        if (!owner.IsServer) return;

        if (owner.EnemyRB.position != owner.StartingPosition)
        {
            owner.enemy.PatienceBar.Patience.Value += 1 * Time.deltaTime;
        }

        if (owner.enemy.PatienceBar.Patience.Value >= owner.enemy.TotalPatience)
        {
            owner.SetState(EnemyStateMachine.State.Reset);
        }
    }

    public override void FixedUpdateState(EnemyStateMachine owner)
    {

    }
}