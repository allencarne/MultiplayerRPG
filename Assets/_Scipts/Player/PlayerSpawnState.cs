using System.Collections;
using UnityEngine;


public class PlayerSpawnState : PlayerState
{
    bool canSpawn = true;

    public PlayerSpawnState(PlayerStateMachine playerStateMachine): base(playerStateMachine) { }

    public override void Update()
    {
        if (canSpawn)
        {
            canSpawn = false;

            stateMachine.BodyAnimator.Play("Spawn");
            stateMachine.EyeAnimator.Play("Spawn");
            stateMachine.HairAnimator.Play("Spawn");

            stateMachine.StartCoroutine(SpawnDuration());
        }
    }

    IEnumerator SpawnDuration()
    {
        yield return new WaitForSeconds(.6f);

        canSpawn = true;

        stateMachine.SetState(new PlayerIdleState(stateMachine));
    }
}
