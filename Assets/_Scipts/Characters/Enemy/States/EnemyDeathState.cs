using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class EnemyDeathState : EnemyState
{
    public override void StartState(EnemyStateMachine owner)
    {
        if (owner.enemy.EnemySpawnerReference != null)
        {
            owner.enemy.EnemySpawnerReference.DecreaseEnemyCount();
        }

        owner.EnemyAnimator.Play("Death");
        owner.EnemyCollider.enabled = false;

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
