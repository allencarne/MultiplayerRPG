using System.Collections.Generic;
using UnityEngine;

public class EnemyChaseState : EnemyState
{
    public override void StartState(EnemyStateMachine owner)
    {
        owner.EnemyAnimator.Play("Chase");

        if (owner.IsServer)
        {
            //owner.enemy.PatienceBar.Patience.Value = 0;
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
        if (owner.IsAttacking) return;

        float distanceToTarget = Vector2.Distance(transform.position, owner.Target.position);

        // Collect all valid attack states
        List<EnemyStateMachine.State> possibleAttacks = new();

        if (distanceToTarget <= owner.BasicRadius && owner.CanBasic)
            possibleAttacks.Add(EnemyStateMachine.State.Basic);

        if (distanceToTarget <= owner.SpecialRadius && owner.CanSpecial)
            possibleAttacks.Add(EnemyStateMachine.State.Special);

        if (distanceToTarget <= owner.UltimateRadius && owner.CanUltimate)
            possibleAttacks.Add(EnemyStateMachine.State.Ultimate);

        // Choose one at random if any are valid
        if (possibleAttacks.Count > 0)
        {
            int randomIndex = Random.Range(0, possibleAttacks.Count);
            owner.SetState(possibleAttacks[randomIndex]);
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