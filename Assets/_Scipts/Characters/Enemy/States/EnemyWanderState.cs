using UnityEngine;

public class EnemyWanderState : EnemyState
{
    public override void StartState(EnemyStateMachine owner)
    {
        owner.EnemyAnimator.Play("Wander");

        if (owner.IsServer)
        {
            owner.WanderPosition = GetRandomClearPoint(owner.StartingPosition, 1f, owner.WanderRadius, owner.obstacleLayerMask);
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
            owner.MoveTowardsTarget(owner.WanderPosition);
        }

        Vector2 direction = (owner.WanderPosition - (Vector2)owner.transform.position).normalized;
        owner.EnemyAnimator.SetFloat("Horizontal", direction.x);
        owner.EnemyAnimator.SetFloat("Vertical", direction.y);
    }

    Vector2 GetRandomClearPoint(Vector2 startingPosition, float minDistance, float maxRadius, LayerMask obstacleLayer, int maxAttempts = 10)
    {
        for (int i = 0; i < maxAttempts; i++)
        {
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float randomRadius = Random.Range(minDistance, maxRadius);

            Vector2 randomPoint = startingPosition + new Vector2(
                Mathf.Cos(angle) * randomRadius,
                Mathf.Sin(angle) * randomRadius
            );

            // Raycast toward the point
            Vector2 direction = randomPoint - startingPosition;
            float distance = direction.magnitude;
            RaycastHit2D hit = Physics2D.Raycast(startingPosition, direction.normalized, distance, obstacleLayer);

            // Debug: Draw the ray (green if clear, red if blocked)
            Color rayColor = hit.collider == null ? Color.green : Color.red;
            Debug.DrawLine(startingPosition, randomPoint, rayColor, 1f);

            if (hit.collider == null)
            {
                return randomPoint; // found clear path
            }
        }

        // Fallback: just return current position if all rays failed
        return startingPosition;
    }
}
