using System.Collections;
using UnityEngine;


public class PlayerSpawnState : PlayerState
{
    public override void StartState(PlayerStateMachine owner)
    {
        Instantiate(owner.player.spawn_Effect, transform.position, transform.rotation);

        owner.SwordAnimator.Play("Spawn");
        owner.BodyAnimator.Play("Spawn");
        owner.EyesAnimator.Play("Spawn");
        owner.HairAnimator.Play("Spawn");
        owner.HeadAnimator.Play("Spawn_" + owner.Equipment.HeadAnimIndex);
        owner.ChestAnimator.Play("Spawn_" + owner.Equipment.ChestAnimIndex);
        owner.LegsAnimator.Play( "Spawn_" + owner.Equipment.LegsAnimIndex);

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

        owner.SetState(PlayerStateMachine.State.Idle);
    }
}
