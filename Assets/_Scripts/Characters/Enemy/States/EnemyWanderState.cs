using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyWanderState : EnemyState
{
    float wanderTime;

    public override void StartState(EnemyStateMachine owner)
    {
        wanderTime = 0;

        if (owner.IsServer)
        {
            owner.WanderPosition = GetRandomClearPoint(owner);
        }

        Vector2 direction = (owner.WanderPosition - (Vector2)owner.transform.position).normalized;

        owner.EnemyAnimator.Play("Wander");
        owner.EnemyAnimator.SetFloat("Horizontal", direction.x);
        owner.EnemyAnimator.SetFloat("Vertical", direction.y);
    }

    public override void UpdateState(EnemyStateMachine owner)
    {
        if (!owner.IsServer) return;

        wanderTime += Time.deltaTime;

        if (wanderTime >= 15f)
        {
            wanderTime = 0;
            owner.SetState(EnemyStateMachine.State.Idle);
        }

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
        if (owner.CrowdControl.immobilize.IsImmobilized) return;

        if (owner.IsServer)
        {
            Vector2 dir = (owner.WanderPosition - (Vector2)owner.transform.position).normalized;
            owner.EnemyRB.linearVelocity = dir * owner.enemy.stats.BaseSpeed;
        }
    }

    Vector2 GetRandomClearPoint(EnemyStateMachine owner)
    {
        int maxAttempts = 10;

        for (int i = 0; i < maxAttempts; i++)
        {
            Vector2 randomPos = owner.StartingPosition + Random.insideUnitCircle * owner.WanderRadius;
            Vector2 randomDir = (randomPos - (Vector2)transform.position);
            float distance = randomDir.magnitude;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, randomDir.normalized, distance, owner.obstacleLayerMask);

            if (hit.collider != null)
            {
                randomPos = hit.point;
            }
            else
            {
                return randomPos;
            }

            Debug.DrawLine(transform.position, randomPos, Color.red, 1f);
        }

        return owner.StartingPosition;
    }
}