using System.Collections;
using UnityEngine;


public class PlayerSpawnState : PlayerState
{
    public PlayerSpawnState(PlayerStateMachine playerStateMachine): base(playerStateMachine) { }

    public override void Start()
    {
        stateMachine.SwordAnimator.Play("Spawn");
        stateMachine.BodyAnimator.Play("Spawn");
        stateMachine.EyeAnimator.Play("Spawn");
        stateMachine.HairAnimator.Play("Spawn");

        stateMachine.StartCoroutine(Duration());
    }

    public override void Update()
    {

    }

    public override void FixedUpdate()
    {

    }

    IEnumerator Duration()
    {
        yield return new WaitForSeconds(.6f);

        stateMachine.SetState(new PlayerIdleState(stateMachine));
    }
}
