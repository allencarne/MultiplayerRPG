using UnityEngine;

public class EnemyIdleState : EnemyState
{
    public EnemyIdleState(EnemyStateMachine owner) : base(owner) { }

    float idleTime = 0;

    public override void EnterState()
    {
        owner.EnemyAnimator.Play("Idle");

        if (owner.enemy.Data.Enemy_Type == EnemyType.Dummy)
        {
            Debug.Log("Dummy in EnemyIdleState");
        }
    }

    public override void UpdateState()
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
                owner.SetState(new EnemyWanderState(owner));
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
            owner.SetState(new EnemyChaseState(owner));
        }
    }
}
