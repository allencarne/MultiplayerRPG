using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    [Header("Obstacle")]
    public LayerMask obstacleLayerMask;

    // Recreates the obstacle-avoiding direction logic (migrated from Enemy/NPC).
    public Vector2 GetDirectionAroundObstacle(Vector2 currentPos, Vector2 targetPos, LayerMask mask, float distance = 2f, int rayCount = 21, float coneSpread = 225f, float castOffset = 0f)
    {
        Vector2 direction = (targetPos - currentPos).normalized;
        if (direction == Vector2.zero) return Vector2.zero;

        Vector2 bestDirection = Vector2.zero;

        Vector2 castOrigin = currentPos + direction * castOffset;
        RaycastHit2D centerRay = Physics2D.Raycast(castOrigin, direction, distance, mask);
        Debug.DrawRay(castOrigin, direction * distance, centerRay ? Color.red : Color.green);

        if (!centerRay) return direction;

        float angleIncrement = coneSpread / (rayCount - 1);
        float bestScore = -Mathf.Infinity;

        for (int i = 0; i < rayCount; i++)
        {
            float angleOffset = -coneSpread / 2f + angleIncrement * i;
            Vector2 dir = Quaternion.Euler(0, 0, angleOffset) * direction;

            castOrigin = currentPos + dir * castOffset;
            RaycastHit2D hit = Physics2D.Raycast(castOrigin, dir, distance, mask);
            Debug.DrawRay(castOrigin, dir * distance, hit ? Color.red : Color.green);

            if (!hit)
            {
                float score = Vector2.Dot(dir, direction);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestDirection = dir;
                }
            }
        }

        return bestDirection == Vector2.zero ? Vector2.zero : bestDirection.normalized;
    }

    // Find a nearby position that is not overlapping obstacles.
    // Returns desiredPos if no obstacle found or no better position found.
    public Vector2 GetValidGroundPosition(Vector2 desiredPos, LayerMask mask, float checkRadius = 0.5f, float maxSearchRadius = 2f, int sampleCount = 16, float step = 0.25f)
    {
        // If desired pos is already clear, return it.
        if (!Physics2D.OverlapCircle(desiredPos, checkRadius, mask))
            return desiredPos;

        // Sample outward in rings until we find a clear spot.
        for (float r = step; r <= maxSearchRadius; r += step)
        {
            for (int i = 0; i < sampleCount; i++)
            {
                float angle = (360f / sampleCount) * i;
                Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
                Vector2 candidate = desiredPos + dir * r;

                if (!Physics2D.OverlapCircle(candidate, checkRadius, mask))
                {
                    Debug.DrawLine(desiredPos, candidate, Color.cyan, 1f);
                    return candidate;
                }
            }
        }

        // Fallback to original (could not find a free spot)
        return desiredPos;
    }

    public bool TryGetLineOfSightTarget(Vector2 origin, Vector2 desiredTarget, LayerMask mask, out Vector2 result, float skinBuffer = 0.15f)
    {
        Vector2 toTarget = desiredTarget - origin;
        float distance = toTarget.magnitude;

        if (distance <= 0.0001f)
        {
            result = desiredTarget;
            return true;
        }

        Vector2 direction = toTarget / distance;
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, distance, mask);

        if (!hit)
        {
            result = desiredTarget;
            return true; // clear line of sight
        }

        // Obstacle in the way — clamp the target to just short of the hit point
        float clampedDistance = Mathf.Max(0f, hit.distance - skinBuffer);
        result = origin + direction * clampedDistance;
        return false; // blocked
    }
}
