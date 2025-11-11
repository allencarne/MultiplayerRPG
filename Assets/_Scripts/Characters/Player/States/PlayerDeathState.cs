using System.Collections;
using UnityEngine;

public class PlayerDeathState : PlayerState
{
    public override void StartState(PlayerStateMachine owner)
    {
        if (!owner.IsOwner) return;

        owner.BodyAnimator.Play("Death");
        owner.player.IsDead = true;
        owner.IsAttacking = false;

        owner.player.CastBar.ForceReset();
        owner.RequestDisableColliderServerRpc(false);

        StartCoroutine(Delay(owner));
    }

    public override void FixedUpdateState(PlayerStateMachine owner)
    {

    }
    public override void UpdateState(PlayerStateMachine owner)
    {

    }

    IEnumerator Delay(PlayerStateMachine owner)
    {
        yield return new WaitForSeconds(4);

        owner.player.IsDead = false;
        owner.transform.position = Vector2.zero;

        yield return new WaitForSeconds(1);

        owner.RequestRespawnServerRpc();
        owner.RequestDisableColliderServerRpc(true);

        owner.SetState(PlayerStateMachine.State.Spawn);
    }
}
