using System.Collections;
using UnityEngine;

public class EnemyDeathState : EnemyState
{
    public EnemyDeathState(EnemyStateMachine owner) : base(owner) { }

    public override void EnterState()
    {
        owner.EnemyAnimator.Play("Death");

        owner.Buffs.PurgeAllDebuffs();
        owner.DeBuffs.CleanseAllDebuffs();

        owner.Drops.DropItem();

        if (owner.enemy.EnemySpawnerReference != null)
        {
            owner.enemy.EnemySpawnerReference.DecreaseEnemyCount();
        }

        owner.StartCoroutine(Delay(owner));
    }
    IEnumerator Delay(EnemyStateMachine owner)
    {
        yield return new WaitForSeconds(.8f);
        owner.DespawnEnemy();
    }
}
