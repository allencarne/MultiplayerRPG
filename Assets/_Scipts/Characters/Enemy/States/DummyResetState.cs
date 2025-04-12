using System.Collections;
using UnityEngine;

public class DummyResetState : EnemyState
{
    public override void StartState(EnemyStateMachine owner)
    {
        owner.EnemyAnimator.Play("Reset");
        StartCoroutine(Delay(owner));
    }

    IEnumerator Delay(EnemyStateMachine owner)
    {
        yield return new WaitForSeconds(.6f);

        owner.IdleTime = 0;
        owner.enemy.PatienceBar.UpdatePatienceBar(owner.IdleTime);

        owner.EnemyRB.linearVelocity = Vector3.zero;
        owner.EnemyRB.position = owner.StartingPosition;

        owner.SetState(EnemyStateMachine.State.Spawn);
    }

    public override void UpdateState(EnemyStateMachine owner)
    {

    }

    public override void FixedUpdateState(EnemyStateMachine owner)
    {

    }
}
