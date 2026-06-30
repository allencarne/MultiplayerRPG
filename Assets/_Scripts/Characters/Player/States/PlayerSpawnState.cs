using System.Collections;
using UnityEngine;


public class PlayerSpawnState : PlayerState
{
    public override void StartState(PlayerStateMachine owner)
    {
        owner.PlayerHeadAnimator.Play("Spawn");
        owner.BodyAnimator.Play("Spawn");

        owner.ChestAnimator.Play("Spawn");
        owner.LegsAnimator.Play( "Spawn");

        owner.WeaponAnimator.Play("Spawn");

        owner.StartCoroutine(Duration(owner));
    }

    public override void UpdateState(PlayerStateMachine owner)
    {

    }

    public override void FixedUpdateState(PlayerStateMachine owner)
    {

    }

    IEnumerator Duration(PlayerStateMachine owner)
    {
        yield return new WaitForSeconds(.6f);

        owner.IsFullySpawned = true;
        owner.SetState(PlayerStateMachine.State.Idle);
    }
}
