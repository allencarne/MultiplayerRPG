using System.Collections;
using UnityEngine;

public class DummyResetState : EnemyState
{
    public override void StartState(EnemyStateMachine owner)
    {
        owner.Collider.enabled = false;
        owner.EnemyAnimator.Play("Reset");

        float missingHealth = owner.enemy.stats.MaxHealth.Value - owner.enemy.stats.Health.Value;
        owner.enemy.GiveHeal(missingHealth, HealType.Flat);

        StartCoroutine(Delay(owner));
    }

    IEnumerator Delay(EnemyStateMachine owner)
    {
        yield return new WaitForSeconds(.6f);

        if (owner.IsServer)
        {
            owner.enemy.PatienceBar.Patience.Value = 0;
            owner.EnemyRB.linearVelocity = Vector3.zero;
            owner.EnemyRB.position = owner.StartingPosition;
            owner.Collider.enabled = true;

            owner.SetState(EnemyStateMachine.State.Spawn);
        }
    }

    public override void UpdateState(EnemyStateMachine owner)
    {

    }

    public override void FixedUpdateState(EnemyStateMachine owner)
    {

    }
}
