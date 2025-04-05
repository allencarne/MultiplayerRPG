using System.Collections;
using UnityEngine;

public class EnemySpawnState : EnemyState
{
    public override void StartState(EnemyStateMachine owner)
    {
        owner.EnemyAnimator.Play("Spawn");

        StartCoroutine(Delay(owner));
    }

    IEnumerator Delay(EnemyStateMachine owner)
    {
        yield return new WaitForSeconds(.6f);

        owner.SetState(EnemyStateMachine.State.Idle);
    }

    public override void UpdateState(EnemyStateMachine owner)
    {

    }

    public override void FixedUpdateState(EnemyStateMachine owner)
    {

    }
}
