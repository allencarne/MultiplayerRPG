using UnityEngine;

public class DummyIdleState : EnemyState
{
    public DummyIdleState(EnemyStateMachine owner) : base(owner) { }

    public override void EnterState()
    {
        owner.Animator.PlayEnemyAnimation("Idle");
        //owner.EnemyAnimator.Play("Idle");
    }

    public override void UpdateState()
    {
        if (!owner.IsServer) return;

        // Increase patience if away from start OR if injured (HP < max)
        bool awayFromStart = owner.RigidBody2D.position != owner.StartingPosition;
        bool injured = owner.enemy.stats.net_CurrentHP.Value < owner.enemy.stats.net_TotalHP.Value;

        if (awayFromStart || injured)
        {
            // Increase patience over time, but cap it at TotalPatience
            float newPatience = owner.enemy.PatienceBar.Patience.Value + 1f * Time.deltaTime;
            owner.enemy.PatienceBar.Patience.Value = Mathf.Min(newPatience, owner.enemy.Data.TotalPatience);
        }

        if (owner.enemy.PatienceBar.Patience.Value >= owner.enemy.Data.TotalPatience)
        {
            owner.SetState(new DummyResetState(owner));
        }
    }
}