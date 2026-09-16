using UnityEngine;

public class EnemyResetState : EnemyState
{
    public EnemyResetState(EnemyStateMachine owner) : base(owner) { }

    public override void EnterState()
    {
        if (!owner.IsServer) return;

        owner.isResetting = true;
        owner.Animator.PlayEnemyAnimation("Wander");
        owner.enemy.PatienceBar.Patience.Value = owner.enemy.Data.TotalPatience;

        owner.IsPlayerInRange = false;
        owner.Target = null;
        owner.SecondTarget = null;
    }

    public override void UpdateState()
    {
        if (!owner.IsServer) return;

        if (Vector2.Distance(owner.transform.position, owner.StartingPosition) <= 0.5f)
        {
            owner.isResetting = false;
            owner.enemy.PatienceBar.Patience.Value = 0;
            owner.RigidBody2D.linearVelocity = Vector2.zero;
            owner.SetState(new EnemyIdleState(owner));
        }
    }

    public override void FixedUpdateState()
    {
        if (!owner.IsServer) return;

        owner.MoveTowardsTarget(owner.StartingPosition, true);

        Vector2 direction = (owner.StartingPosition - (Vector2)owner.transform.position).normalized;
        owner.Animator.SetEnemyDirection(direction);
    }
}
