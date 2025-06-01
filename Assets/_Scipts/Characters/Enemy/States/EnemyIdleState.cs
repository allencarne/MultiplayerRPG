using UnityEngine;

public class EnemyIdleState : EnemyState
{
    float idleTime = 0;

    public override void StartState(EnemyStateMachine owner)
    {
        owner.EnemyAnimator.Play("Idle");
    }

    public override void UpdateState(EnemyStateMachine owner)
    {
        if (!owner.IsServer) return;

        idleTime += Time.deltaTime;

        if (idleTime >= 5f)
        {
            int maxAttempts = 3;
            int consecutiveFailures = Mathf.Min(owner.AttemptsCount, maxAttempts);
            float wanderProbability = Mathf.Min(0.5f + 0.25f * consecutiveFailures, 1.0f);

            // Transition To Wander
            if (Random.value < wanderProbability)
            {
                idleTime = 0;
                owner.SetState(EnemyStateMachine.State.Wander);
            }

            // Reset Idle
            idleTime = 0f;
            owner.AttemptsCount++;
        }

        // Transition To Chase
        if (owner.IsPlayerInRange)
        {
            owner.AttemptsCount = 0;
            idleTime = 0f;
            owner.SetState(EnemyStateMachine.State.Chase);
        }
    }

    public override void FixedUpdateState(EnemyStateMachine owner)
    {

    }
}
