using System.Collections;
using UnityEngine;

public class EnemySpawnState : EnemyState
{
    public EnemySpawnState(EnemyStateMachine owner) : base(owner) { }

    public override void EnterState()
    {
        // Disable the collider to prevent immediate collisions upon spawning
        owner.Collider.enabled = false;

        // Play the spawn animation
        owner.EnemyAnimator.Play("Spawn");

        // Start the duration coroutine
        owner.StartCoroutine(Delay(owner));
    }

    IEnumerator Delay(EnemyStateMachine owner)
    {
        yield return new WaitForSeconds(.6f);
        owner.Collider.enabled = true;
        owner.TransitionToIdle();
    }
}
