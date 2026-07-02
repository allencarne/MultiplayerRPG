using System.Collections;
using UnityEngine;

public class EnemySpawnState : EnemyState
{
    public override void StartState(EnemyStateMachine owner)
    {
        // Disable the collider to prevent immediate collisions upon spawning
        owner.Collider.enabled = false;

        // Play the spawn animation
        owner.EnemyAnimator.Play("Spawn");

        // Start the duration coroutine
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
