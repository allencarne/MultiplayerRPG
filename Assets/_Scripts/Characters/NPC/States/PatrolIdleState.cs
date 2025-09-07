using UnityEngine;

public class PatrolIdleState : NPCState
{
    float idleTime = 0;
    int attempts = 0;

    public override void StartState(NPCStateMachine owner)
    {
        owner.SwordAnimator.Play("Idle");
        owner.BodyAnimator.Play("Idle");
        owner.EyesAnimator.Play("Idle");
        owner.HairAnimator.Play("Idle_" + owner.npc.hairIndex);
    }

    public override void UpdateState(NPCStateMachine owner)
    {
        if (!owner.IsServer) return;

        idleTime += Time.deltaTime;

        if (idleTime >= 5f)
        {
            int maxAttempts = 3;
            int consecutiveFailures = Mathf.Min(attempts, maxAttempts);
            float wanderProbability = Mathf.Min(0.5f + 0.25f * consecutiveFailures, 1.0f);

            // Transition To Wander
            if (Random.value < wanderProbability)
            {
                idleTime = 0;
                owner.SetState(NPCStateMachine.State.Wander);
            }

            // Reset Idle
            idleTime = 0f;
            attempts++;
        }

        // Transition To Chase
        if (owner.IsEnemyInRange)
        {
            attempts = 0;
            idleTime = 0f;
            owner.SetState(NPCStateMachine.State.Chase);
        }
    }

    public override void FixedUpdateState(NPCStateMachine owner)
    {

    }
}
