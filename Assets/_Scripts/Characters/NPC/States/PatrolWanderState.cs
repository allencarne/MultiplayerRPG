using UnityEngine;

public class PatrolWanderState : NPCState
{
    [SerializeField] float wanderRadius;
    Vector2 wanderPosition;
    float wanderTime;

    public override void StartState(NPCStateMachine owner)
    {
        wanderTime = 0;

        owner.SwordAnimator.Play("Run");
        owner.BodyAnimator.Play("Run");
        owner.EyesAnimator.Play("Run");
        owner.HairAnimator.Play("Run_" + owner.npc.hairIndex);

        wanderPosition = GetRandomClearPoint(owner.StartingPosition, wanderRadius, owner.obstacleLayerMask);
    }

    public override void UpdateState(NPCStateMachine owner)
    {
        wanderTime += Time.deltaTime;

        if (wanderTime >= 15f)
        {
            wanderTime = 0;
            owner.SetState(NPCStateMachine.State.Idle);
        }

        // Transition To Idle
        if (Vector2.Distance(owner.transform.position, wanderPosition) <= 0.1f)
        {
            owner.NpcRB.linearVelocity = Vector2.zero;
            owner.SetState(NPCStateMachine.State.Idle);
        }

        // Transition To Chase
        if (owner.IsEnemyInRange)
        {
            owner.SetState(NPCStateMachine.State.Chase);
        }
    }

    public override void FixedUpdateState(NPCStateMachine owner)
    {
        if (owner.CrowdControl.immobilize.IsImmobilized) return;

        Vector2 dir = (wanderPosition - (Vector2)owner.transform.position).normalized;
        owner.NpcRB.linearVelocity = dir * owner.npc.BaseSpeed;

        Vector2 direction = (wanderPosition - (Vector2)owner.transform.position).normalized;
        owner.SwordAnimator.SetFloat("Horizontal", direction.x);
        owner.SwordAnimator.SetFloat("Vertical", direction.y);

        owner.BodyAnimator.SetFloat("Horizontal", direction.x);
        owner.BodyAnimator.SetFloat("Vertical", direction.y);

        owner.EyesAnimator.SetFloat("Horizontal", direction.x);
        owner.EyesAnimator.SetFloat("Vertical", direction.y);

        owner.HairAnimator.SetFloat("Horizontal", direction.x);
        owner.HairAnimator.SetFloat("Vertical", direction.y);
    }

    Vector2 GetRandomClearPoint(Vector2 startingPosition, float maxRadius, LayerMask obstacleLayer, int maxAttempts = 10)
    {
        for (int i = 0; i < maxAttempts; i++)
        {
            // Get Random Point within Wander Radius
            Vector2 randomPos = startingPosition + Random.insideUnitCircle * maxRadius;

            // Get Direction to Point
            Vector2 randomDir = (randomPos - (Vector2)transform.position);

            // Get Distance to Point
            float distance = randomDir.magnitude;

            // Raycast from Enemy Position
            RaycastHit2D hit = Physics2D.Raycast(transform.position, randomDir.normalized, distance, obstacleLayer);

            // Draw Line
            Color rayColor = hit.collider == null ? Color.green : Color.red;
            Debug.DrawLine(transform.position, randomPos, rayColor, 1f);

            // If Valid Path is Found
            if (hit.collider == null && !Physics2D.OverlapCircle(randomPos, .5f, obstacleLayer))
            {
                return randomPos;
            }
        }

        // If no valid position is found
        return startingPosition;
    }
}
