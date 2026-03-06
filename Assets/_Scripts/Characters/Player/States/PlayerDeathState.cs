using System.Collections;
using UnityEngine;

public class PlayerDeathState : PlayerState
{
    public override void StartState(PlayerStateMachine owner)
    {
        if (!owner.IsOwner) return;

        owner.Buffs.PurgeAllDebuffs();
        owner.DeBuffs.CleanseAllDebuffs();

        owner.BodyAnimator.Play("Death");
        owner.IsAttacking = false;

        owner.player.CastBar.ResetCastBar();
        owner.RequestSetColliderServerRpc(false);

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
        owner.RequestSetSpritesServerRpc(false);
        owner.transform.position = Vector2.zero;

        yield return new WaitForSeconds(1);

        owner.RequestRespawnServerRpc();
        owner.RequestSetColliderServerRpc(true);
        owner.RequestSetSpritesServerRpc(true);
        owner.SetState(PlayerStateMachine.State.Spawn);
    }
}
