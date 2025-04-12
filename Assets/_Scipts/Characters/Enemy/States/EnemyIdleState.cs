using UnityEngine;

public class EnemyIdleState : EnemyState
{
    public override void StartState(EnemyStateMachine owner)
    {
        owner.EnemyAnimator.Play("Idle");
    }

    public override void UpdateState(EnemyStateMachine owner)
    {
        if (!owner.IsServer) return;

        if (owner.enemy.PatienceBar.Patience.Value >= 5f)
        {
            int maxAttempts = 3;
            int consecutiveFailures = Mathf.Min(owner.AttemptsCount, maxAttempts);
            float wanderProbability = Mathf.Min(0.5f + 0.25f * consecutiveFailures, 1.0f);

            // Transition To Wander
            if (Random.value < wanderProbability)
            {
                owner.enemy.PatienceBar.Patience.Value = 0;
                owner.SetState(EnemyStateMachine.State.Wander);
            }

            // Reset Idle
            owner.enemy.PatienceBar.Patience.Value = 0f;
            owner.AttemptsCount++;
        }

        // Transition To Chase
        if (owner.IsPlayerInRange)
        {
            owner.AttemptsCount = 0;
            owner.enemy.PatienceBar.Patience.Value = 0f;
            owner.SetState(EnemyStateMachine.State.Chase);
        }
    }

    public override void FixedUpdateState(EnemyStateMachine owner)
    {

    }
}
