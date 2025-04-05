using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyWanderState : EnemyState
{
    public override void StartState(EnemyStateMachine owner)
    {
        owner.EnemyAnimator.Play("Wander");

        float minWanderDistance = 1f; // Minimum distance away
        owner.WanderPosition = GetRandomPointInCircle(owner.StartingPosition, minWanderDistance, owner.WanderRadius);
    }

    public override void UpdateState(EnemyStateMachine owner)
    {
        if (Vector2.Distance(transform.position, owner.WanderPosition) <= 0.1f)
        {
            owner.EnemyRB.linearVelocity = Vector2.zero;
            owner.SetState(EnemyStateMachine.State.Idle);
        }
    }

    public override void FixedUpdateState(EnemyStateMachine owner)
    {
        owner.MoveTowardsTarget(owner.WanderPosition);

        owner.EnemyAnimator.SetFloat("Horizontal", owner.WanderPosition.x);
        owner.EnemyAnimator.SetFloat("Vertical", owner.WanderPosition.y);
    }

    Vector2 GetRandomPointInCircle(Vector2 startingPosition, float minDistance, float maxRadius)
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float randomRadius = Random.Range(minDistance, maxRadius);
        Vector2 randomPoint = startingPosition + new Vector2(Mathf.Cos(angle) * randomRadius, Mathf.Sin(angle) * randomRadius);
        return randomPoint;
    }
}
