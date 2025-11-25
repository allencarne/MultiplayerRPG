using System.Collections.Generic;
using UnityEngine;

public class EnemyChaseState : EnemyState
{
    private const float ANIMATION_UPDATE_INTERVAL = 0.1f; // 10 times per second
    private float nextAnimationUpdateTime;

    public override void StartState(EnemyStateMachine owner)
    {
        if (!owner.IsServer) return;

        owner.EnemyAnimator.Play("Chase");
        nextAnimationUpdateTime = Time.time;
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
        if (!owner.IsServer || !owner.Target) return;

        owner.MoveTowardsTarget(owner.Target.position);

        if (Time.time >= nextAnimationUpdateTime)
        {
            Vector2 direction = (owner.Target.position - owner.transform.position).normalized;
            owner.EnemyAnimator.SetFloat("Horizontal", direction.x);
            owner.EnemyAnimator.SetFloat("Vertical", direction.y);

            nextAnimationUpdateTime = Time.time + ANIMATION_UPDATE_INTERVAL;
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
        if (owner.IsAttacking)
        {
            Debug.Log("Is Attacking");
            return;
        }

        float distanceToTarget = Vector2.Distance(owner.transform.position, owner.Target.position);

        if (distanceToTarget <= owner.UltimateRadius)
        {
            if (owner.CanUltimate && !owner.CrowdControl.silence.IsSilenced)
            {
                owner.SetState(EnemyStateMachine.State.Ultimate);
                return;
            }
        }

        if (distanceToTarget <= owner.SpecialRadius)
        {
            if (owner.CanSpecial && !owner.CrowdControl.silence.IsSilenced)
            {
                owner.SetState(EnemyStateMachine.State.Special);
                return;
            }
        }

        if (distanceToTarget <= owner.BasicRadius)
        {
            if (owner.CanBasic && !owner.CrowdControl.disarm.IsDisarmed)
            {
                owner.SetState(EnemyStateMachine.State.Basic);
                return;
            }
        }
    }

    public void HandleDeAggro(EnemyStateMachine owner)
    {
        float distanceToStartingPosition = Vector2.Distance(owner.StartingPosition, owner.Target.position);

        if (distanceToStartingPosition > owner.DeAggroRadius)
        {
            owner.enemy.PatienceBar.Patience.Value += Time.deltaTime;
            if (owner.enemy.PatienceBar.Patience.Value >= owner.enemy.TotalPatience)
            {
                TransitionToReset(owner);
            }
        }
        else
        {
            owner.enemy.PatienceBar.Patience.Value = Mathf.Max(0, owner.enemy.PatienceBar.Patience.Value - Time.deltaTime);
        }
    }
}