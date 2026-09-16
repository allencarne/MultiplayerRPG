using System.Collections;
using UnityEngine;

public class EnemySpawnState : EnemyState
{
    public EnemySpawnState(EnemyStateMachine owner) : base(owner) { }

    public override void EnterState()
    {
        owner.Collider2D.enabled = false;
        owner.Animator.PlayEnemyAnimation("Spawn");
        owner.StartCoroutine(Delay(owner));
    }

    IEnumerator Delay(EnemyStateMachine owner)
    {
        yield return new WaitForSeconds(.6f);
        owner.Collider2D.enabled = true;
        owner.TransitionToIdle();
    }
}
