using UnityEngine;

public class EnemyChaseState : EnemyState
{
    public override void StartState(EnemyStateMachine owner)
    {
        owner.EnemyAnimator.Play("Chase");
    }

    public override void UpdateState(EnemyStateMachine owner)
    {
        owner.enemy.UpdatePatienceBar(owner.enemy.CurrentPatience);


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
        owner.enemy.CurrentPatience = 0;
        owner.IsPlayerInRange = false;
        owner.Target = null;
        owner.SetState(EnemyStateMachine.State.Reset);
    }

    public void HandleAttack(EnemyStateMachine owner)
    {
        float distanceToTarget = Vector2.Distance(transform.position, owner.Target.position);
        if (distanceToTarget <= owner.BasicRadius && owner.CanBasic)
        {
            owner.SetState(EnemyStateMachine.State.Basic);
        }
    }

    public void HandleDeAggro(EnemyStateMachine owner)
    {
        float distanceToStartingPosition = Vector2.Distance(owner.StartingPosition, owner.Target.position);

        if (distanceToStartingPosition > owner.DeAggroRadius)
        {
            // If outside deAggroRadius, increase patience
            owner.enemy.CurrentPatience += Time.deltaTime;

            if (owner.enemy.CurrentPatience >= owner.enemy.TotalPatience)
            {
                TransitionToReset(owner);
            }
        }
        else
        {
            // If back inside the radius, gradually decrease patience
            owner.enemy.CurrentPatience = Mathf.Max(0, owner.enemy.CurrentPatience - Time.deltaTime);
        }
    }
}
