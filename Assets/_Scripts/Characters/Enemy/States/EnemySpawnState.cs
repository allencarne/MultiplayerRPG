using System.Collections;
using UnityEngine;

public class EnemySpawnState : EnemyState
{
    public override void StartState(EnemyStateMachine owner)
    {
        owner.Collider.enabled = false;
        owner.EnemyAnimator.Play("Spawn");

        StartCoroutine(Delay(owner));
    }

    IEnumerator Delay(EnemyStateMachine owner)
    {
        yield return new WaitForSeconds(.6f);

        owner.Collider.enabled = true;
        owner.SetState(EnemyStateMachine.State.Idle);
    }

    public override void UpdateState(EnemyStateMachine owner)
    {

    }

    public override void FixedUpdateState(EnemyStateMachine owner)
    {

    }
}
