using System.Collections;
using UnityEngine;

public class DummyResetState : EnemyState
{
    public DummyResetState(EnemyStateMachine owner) : base(owner) { }

    public override void EnterState()
    {
        owner.isResetting = true;
        owner.Collider2D.enabled = false;
        //owner.EnemyAnimator.Play("Reset");
        owner.Animator.PlayEnemyAnimation("Reset");

        float missingHealth = owner.enemy.stats.net_BaseHP.Value - owner.enemy.stats.net_CurrentHP.Value;
        owner.enemy.stats.GiveHeal(missingHealth, HealType.Flat);

        owner.StartCoroutine(Delay(owner));
    }

    IEnumerator Delay(EnemyStateMachine owner)
    {
        yield return new WaitForSeconds(.6f);

        if (owner.IsServer)
        {
            owner.isResetting = false;
            owner.enemy.PatienceBar.Patience.Value = 0;
            owner.RigidBody2D.linearVelocity = Vector3.zero;
            owner.RigidBody2D.position = owner.StartingPosition;
            owner.Collider2D.enabled = true;

            owner.SetState(new EnemySpawnState(owner));
        }
    }
}
