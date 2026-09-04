using UnityEngine;

public struct AimContext
{
    public Vector2 SpawnPosition;
    public Vector2 Direction;
    public Quaternion Rotation;
    public Vector2 Offset;

    public AimContext FromDirection(StateMachine owner, Vector2 direction,float skillRange)
    {
        direction = direction.normalized;

        return new AimContext
        {
            SpawnPosition = owner.transform.position,
            Direction = direction,
            Rotation = DirectionToRotation(direction),
            Offset = direction * skillRange
        };
    }

    public AimContext FromGroundTarget(StateMachine owner, Vector2 target)
    {
        Vector2 direction = GetDirectionToTarget(owner.transform.position,target);

        return new AimContext
        {
            SpawnPosition = target,
            Direction = direction != Vector2.zero
                ? direction
                : owner.transform.right,

            Rotation = direction != Vector2.zero
                ? DirectionToRotation(direction)
                : owner.transform.rotation,

            Offset = Vector2.zero
        };
    }

    Vector2 GetDirectionToTarget(Vector2 origin, Vector2 target)
    {
        Vector2 direction = target - origin;

        if (direction.sqrMagnitude <= 0.0001f)
            return Vector2.zero;

        return direction.normalized;
    }

    Quaternion DirectionToRotation(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return Quaternion.Euler(0f, 0f, angle);
    }
}
