using UnityEngine;

public class EnemyWanderState : EnemyState
{
    public EnemyWanderState(EnemyStateMachine owner) : base(owner) { }

    float wanderTime;

    public override void EnterState()
    {
        wanderTime = 0;

        if (owner.IsServer)
        {
            owner.WanderPosition = GetRandomClearPoint(owner);
        }

        Vector2 direction = (owner.WanderPosition - (Vector2)owner.transform.position).normalized;

        //owner.EnemyAnimator.Play("Wander");
        //owner.EnemyAnimator.SetFloat("Horizontal", direction.x);
        //owner.EnemyAnimator.SetFloat("Vertical", direction.y);

        owner.Animator.PlayEnemyAnimation("Wander");
        owner.Animator.SetEnemyDirection(direction);
    }
    public override void UpdateState()
    {
        if (!owner.IsServer) return;

        wanderTime += Time.deltaTime;

        if (wanderTime >= 15f)
        {
            wanderTime = 0;
            owner.SetState(new EnemyIdleState(owner));
        }

        // Transition To Idle
        if (Vector2.Distance(owner.transform.position, owner.WanderPosition) <= 0.1f)
        {
            owner.RigidBody2D.linearVelocity = Vector2.zero;
            owner.SetState(new EnemyIdleState(owner));
        }

        // Transition To Chase
        if (owner.IsPlayerInRange)
        {
            owner.SetState(new EnemyChaseState(owner));
        }
    }

    public override void FixedUpdateState()
    {
        if (owner.CrowdControl.immobilize.IsImmobilized) return;

        if (owner.IsServer)
        {
            Vector2 dir = (owner.WanderPosition - (Vector2)owner.transform.position).normalized;
            owner.RigidBody2D.linearVelocity = dir * owner.enemy.stats.TotalSpeed;
        }
    }

    Vector2 GetRandomClearPoint(EnemyStateMachine owner)
    {
        int maxAttempts = 10;

        for (int i = 0; i < maxAttempts; i++)
        {
            Vector2 randomPos = owner.StartingPosition + Random.insideUnitCircle * owner.enemy.Data.WanderRadius;
            Vector2 randomDir = (randomPos - (Vector2)owner.transform.position);
            float distance = randomDir.magnitude;

            RaycastHit2D hit = Physics2D.Raycast(owner.transform.position, randomDir.normalized, distance, owner.obstacleLayerMask);

            if (hit.collider != null)
            {
                randomPos = hit.point;
            }
            else
            {
                return randomPos;
            }

            Debug.DrawLine(owner.transform.position, randomPos, Color.red, 1f);
        }

        return owner.StartingPosition;
    }
}