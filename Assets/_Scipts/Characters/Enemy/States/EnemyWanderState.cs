using UnityEngine;

public class EnemyWanderState : EnemyState
{
    public override void StartState(EnemyStateMachine owner)
    {
        owner.EnemyAnimator.Play("Wander");

        if (owner.IsServer)
        {
            float minWanderDistance = 1f;
            owner.WanderPosition = GetRandomPointInCircle(owner.StartingPosition, minWanderDistance, owner.WanderRadius);
        }
    }

    public override void UpdateState(EnemyStateMachine owner)
    {
        if (!owner.IsServer) return;

        if (Vector2.Distance(owner.transform.position, owner.WanderPosition) <= 0.1f)
        {
            owner.EnemyRB.linearVelocity = Vector2.zero;
            owner.SetState(EnemyStateMachine.State.Idle);
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

    Vector2 GetRandomPointInCircle(Vector2 startingPosition, float minDistance, float maxRadius)
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float randomRadius = Random.Range(minDistance, maxRadius);
        Vector2 randomPoint = startingPosition + new Vector2(Mathf.Cos(angle) * randomRadius, Mathf.Sin(angle) * randomRadius);
        return randomPoint;
    }
}
