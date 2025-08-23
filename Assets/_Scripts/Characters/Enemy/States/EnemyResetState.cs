using UnityEngine;

public class EnemyResetState : EnemyState
{
    public override void StartState(EnemyStateMachine owner)
    {
        owner.EnemyAnimator.Play("Wander");

        if (owner.IsServer)
        {
            owner.enemy.PatienceBar.Patience.Value = 0;
        }

        if (owner.enemy.Health.Value < owner.enemy.MaxHealth.Value)
        {
            owner.Buffs.regeneration.Regeneration(HealType.Percentage, 10, .5f, 5);
        }
    }

    public override void UpdateState(EnemyStateMachine owner)
    {
        if (!owner.IsServer) return;

        if (Vector2.Distance(transform.position, owner.StartingPosition) <= 0.1f)
        {
            owner.EnemyRB.linearVelocity = Vector2.zero;
            owner.SetState(EnemyStateMachine.State.Idle);
        }
    }

    public override void FixedUpdateState(EnemyStateMachine owner)
    {
        if (owner.IsServer)
        {
            owner.MoveTowardsTarget(owner.StartingPosition);
        }

        Vector2 direction = (owner.StartingPosition - (Vector2)owner.transform.position).normalized;
        owner.EnemyAnimator.SetFloat("Horizontal", direction.x);
        owner.EnemyAnimator.SetFloat("Vertical", direction.y);
    }
}
