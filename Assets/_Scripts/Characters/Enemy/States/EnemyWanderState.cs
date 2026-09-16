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
            owner.WanderPosition = owner.Pathfinding.GetRandomClearPoint(owner.StartingPosition,owner.transform.position,owner.enemy.Data.WanderRadius,owner.obstacleLayerMask);
        }

        Vector2 direction = (owner.WanderPosition - (Vector2)owner.transform.position).normalized;

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
}