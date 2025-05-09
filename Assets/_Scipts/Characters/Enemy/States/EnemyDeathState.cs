using System.Collections;
using UnityEngine;

public class EnemyDeathState : EnemyState
{
    public override void StartState(EnemyStateMachine owner)
    {
        owner.EnemyAnimator.Play("Death");

        owner.HandleDeathClientRPC();

        if (owner.enemy.EnemySpawnerReference != null)
        {
            owner.enemy.EnemySpawnerReference.DecreaseEnemyCount();
        }

        StartCoroutine(Delay(owner));
    }

    IEnumerator Delay(EnemyStateMachine owner)
    {
        yield return new WaitForSeconds(.8f);

        owner.DespawnEnemy();
    }

    public override void UpdateState(EnemyStateMachine owner)
    {
        if (owner.ImpactCoroutine != null)
        {
            owner.StopCoroutine(owner.ImpactCoroutine);
            owner.ImpactCoroutine = null;
        }

        if (owner.RecoveryCoroutine != null)
        {
            owner.StopCoroutine(owner.RecoveryCoroutine);
            owner.RecoveryCoroutine = null;
        }
    }

    public override void FixedUpdateState(EnemyStateMachine owner)
    {

    }
}
