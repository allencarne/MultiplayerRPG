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

        // Increase patience if away from start OR if injured (HP < max)
        bool awayFromStart = owner.EnemyRB.position != owner.StartingPosition;
        bool injured = owner.enemy.stats.net_CurrentHP.Value < owner.enemy.stats.net_TotalHP.Value;

        if (awayFromStart || injured)
        {
            // Increase patience over time, but cap it at TotalPatience
            float newPatience = owner.enemy.PatienceBar.Patience.Value + 1f * Time.deltaTime;
            owner.enemy.PatienceBar.Patience.Value = Mathf.Min(newPatience, owner.enemy.Data.TotalPatience);
        }

        if (owner.enemy.PatienceBar.Patience.Value >= owner.enemy.Data.TotalPatience)
        {
            owner.SetState(EnemyStateMachine.State.Reset);
        }
    }

    public override void FixedUpdateState(EnemyStateMachine owner)
    {

    }
}