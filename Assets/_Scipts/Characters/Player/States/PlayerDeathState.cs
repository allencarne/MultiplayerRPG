using System.Collections;
using UnityEngine;

public class PlayerDeathState : PlayerState
{
    public override void StartState(PlayerStateMachine owner)
    {
        if (!owner.IsOwner) return;

        owner.player.IsDead = true;
        owner.BodyAnimator.Play("Death");

        owner.RequestDisableColliderServerRpc();

        StartCoroutine(Delay(owner));
    }

    IEnumerator Delay(PlayerStateMachine owner)
    {
        yield return new WaitForSeconds(4);

        owner.player.IsDead = false;
        owner.transform.position = Vector2.zero;

        yield return new WaitForSeconds(1);

        owner.RequestRespawnServerRpc();
        owner.RequestEnableColliderServerRpc();

        owner.SetState(PlayerStateMachine.State.Spawn);
    }

    public override void FixedUpdateState(PlayerStateMachine owner)
    {

    }
    public override void UpdateState(PlayerStateMachine owner)
    {

    }
}
