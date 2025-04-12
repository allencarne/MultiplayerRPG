using UnityEngine;

public class EnemyIdleState : EnemyState
{
    public override void StartState(EnemyStateMachine owner)
    {
        owner.EnemyAnimator.Play("Idle");
    }

    public override void UpdateState(EnemyStateMachine owner)
    {
        owner.enemy.PatienceBar.UpdatePatienceBar(owner.IdleTime);

        owner.IdleTime += Time.deltaTime;

        if (owner.IdleTime >= 5)
        {
            int maxAttempts = 3; // Maximum number of consecutive failed attempts
            int consecutiveFailures = Mathf.Min(owner.AttemptsCount, maxAttempts);

            // Calculate the probability of transitioning to the wander state based on the number of consecutive failures
            float wanderProbability = Mathf.Min(0.5f + 0.25f * consecutiveFailures, 1.0f);

            // Check if the enemy will transition to the wander state based on the calculated probability
            if (Random.value < wanderProbability)
            {
                owner.IdleTime = 0;

                owner.SetState(EnemyStateMachine.State.Wander);
            }

            // Reset the idle time and update the attempts count
            owner.IdleTime = 0;
            owner.AttemptsCount++;
        }

        if (owner.IsPlayerInRange)
        {
            owner.AttemptsCount = 0;
            owner.IdleTime = 0;
            owner.SetState(EnemyStateMachine.State.Chase);
        }
    }

    public override void FixedUpdateState(EnemyStateMachine owner)
    {

    }
}
