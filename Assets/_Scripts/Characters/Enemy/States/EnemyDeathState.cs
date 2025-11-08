using System.Collections;
using UnityEngine;

public class EnemyDeathState : EnemyState
{
    public override void StartState(EnemyStateMachine owner)
    {
        owner.enemy.IsDead = true;
        owner.Drops.DropItem();

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

    }

    public override void FixedUpdateState(EnemyStateMachine owner)
    {

    }
}
