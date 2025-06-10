using System.Collections;
using UnityEngine;

public class PlayerDeathState : PlayerState
{
    public override void StartState(PlayerStateMachine owner)
    {
        // This code is executed by the server only (not run locally by client)
        owner.HandleDeathClientRPC(false);
        StartCoroutine(Delay(owner));
    }

    IEnumerator Delay(PlayerStateMachine owner)
    {
        yield return new WaitForSeconds(1);
        owner.SetState(PlayerStateMachine.State.Spawn);
        owner.player.GiveHeal(100, HealType.Percentage);
        owner.HandleDeathClientRPC(true);
    }

    public override void FixedUpdateState(PlayerStateMachine owner)
    {

    }
    public override void UpdateState(PlayerStateMachine owner)
    {

    }
}
