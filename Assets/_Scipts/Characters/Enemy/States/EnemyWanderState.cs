using UnityEngine;

public class EnemyWanderState : EnemyState
{
    public override void StartState(EnemyStateMachine owner)
    {
        owner.EnemyAnimator.Play("Wander");

        if (owner.IsServer)
        {
            owner.WanderPosition = GetRandomClearPoint(owner.StartingPosition, owner.WanderRadius, owner.obstacleLayerMask);
        }
    }

    public override void UpdateState(EnemyStateMachine owner)
    {
        if (!owner.IsServer) return;

        // Transition To Idle
        if (Vector2.Distance(owner.transform.position, owner.WanderPosition) <= 0.1f)
        {
            owner.EnemyRB.linearVelocity = Vector2.zero;
            owner.SetState(EnemyStateMachine.State.Idle);
        }

        // Transition To Chase
        if (owner.IsPlayerInRange)
        {
            owner.SetState(EnemyStateMachine.State.Chase);
        }
    }

    public override void FixedUpdateState(EnemyStateMachine owner)
    {
        if (owner.IsServer)
        {
            Vector2 dir = (owner.WanderPosition - (Vector2)owner.transform.position).normalized;
            owner.EnemyRB.linearVelocity = dir * owner.enemy.BaseSpeed;
            //owner.MoveTowardsTarget(owner.WanderPosition);
        }

        Vector2 direction = (owner.WanderPosition - (Vector2)owner.transform.position).normalized;
        owner.EnemyAnimator.SetFloat("Horizontal", direction.x);
        owner.EnemyAnimator.SetFloat("Vertical", direction.y);
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

            // Raycast from Enemy Position to the random point
            RaycastHit2D hit = Physics2D.Raycast(transform.position, randomDir.normalized, distance, obstacleLayer);

            // Debug: Draw the ray (green if clear, red if blocked)
            Color rayColor = hit.collider == null ? Color.green : Color.red;
            Debug.DrawLine(transform.position, randomPos, rayColor, 1f);

            if (hit.collider == null)
            {
                return randomPos; // found clear path
            }
        }

        // Fallback: just return current position if all rays failed
        return startingPosition;
    }
}