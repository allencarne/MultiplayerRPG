using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyChaseState : EnemyState
{
    public override void StartState(EnemyStateMachine owner)
    {
        owner.EnemyAnimator.Play("Chase");

        if (owner.IsServer)
        {
            // Optionally reset patience at the start of chase
            owner.enemy.PatienceBar.Patience.Value = 0;
        }
    }

    public override void UpdateState(EnemyStateMachine owner)
    {
        if (!owner.IsServer) return;


        if (owner.Target == null)
        {
            TransitionToReset(owner);
            return;
        }

        HandleAttack(owner);
        HandleDeAggro(owner);
    }

    public override void FixedUpdateState(EnemyStateMachine owner)
    {
        if (!owner.IsServer) return;

        if (owner.Target)
        {
            owner.MoveTowardsTarget(owner.Target.position);

            Vector2 direction = (owner.Target.position - owner.transform.position).normalized;
            owner.EnemyAnimator.SetFloat("Horizontal", direction.x);
            owner.EnemyAnimator.SetFloat("Vertical", direction.y);
        }
    }

    public void TransitionToReset(EnemyStateMachine owner)
    {
        owner.enemy.PatienceBar.Patience.Value = 0;
        owner.IsPlayerInRange = false;
        owner.Target = null;
        owner.SetState(EnemyStateMachine.State.Reset);
    }

    public void HandleAttack(EnemyStateMachine owner)
    {
        float distanceToTarget = Vector2.Distance(transform.position, owner.Target.position);

        // Transition To Basic
        if (distanceToTarget <= owner.BasicRadius)
        {
            if (owner.IsAttacking) return;
            if (!owner.CanBasic) return;

            owner.SetState(EnemyStateMachine.State.Basic);
        }

        // Transition To Special
        if (distanceToTarget <= owner.SpecialRadius)
        {
            if (owner.IsAttacking) return;
            if (!owner.CanSpecial) return;

            owner.SetState(EnemyStateMachine.State.Special);
        }

        // Transition To Ultimate
        if (distanceToTarget <= owner.UltimateRadius)
        {
            if (owner.IsAttacking) return;
            if (!owner.CanUltimate) return;

            owner.SetState(EnemyStateMachine.State.Ultimate);
        }
    }

    public void HandleDeAggro(EnemyStateMachine owner)
    {
        float distanceToStartingPosition = Vector2.Distance(owner.StartingPosition, owner.Target.position);

        if (distanceToStartingPosition > owner.DeAggroRadius)
        {
            // If outside deAggroRadius, increase patience
            owner.enemy.PatienceBar.Patience.Value += Time.deltaTime;

            if (owner.enemy.PatienceBar.Patience.Value >= owner.enemy.TotalPatience)
            {
                TransitionToReset(owner);
            }
        }
        else
        {
            // If back inside the radius, gradually decrease patience
            owner.enemy.PatienceBar.Patience.Value = Mathf.Max(0, owner.enemy.PatienceBar.Patience.Value - Time.deltaTime);
        }
    }
}