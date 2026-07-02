using System.Collections;
using UnityEngine;


public class PlayerSpawnState : PlayerState
{
    public override void StartState(PlayerStateMachine owner)
    {
        // Play spawn animation
        owner.PlayerHeadAnimator.Play("Spawn");
        owner.BodyAnimator.Play("Spawn");
        owner.ChestAnimator.Play("Spawn");
        owner.LegsAnimator.Play( "Spawn");
        owner.WeaponAnimator.Play(owner.customization.WeaponAnimType + " Spawn");

        // Face Down
        owner.PlayerHeadAnimator.SetFloat("Vertical", -1);
        owner.customization.net_FacingDirection.Value = new Vector2(0, -1);
        owner.BodyAnimator.SetFloat("Vertical", -1);
        owner.ChestAnimator.SetFloat("Vertical", -1);
        owner.LegsAnimator.SetFloat("Vertical", -1);
        owner.WeaponAnimator.SetFloat("Vertical", -1);

        // Start the duration coroutine
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
        yield return new WaitForSeconds(.3f);

        owner.IsFullySpawned = true;
        owner.SetState(PlayerStateMachine.State.Idle);
    }
}
