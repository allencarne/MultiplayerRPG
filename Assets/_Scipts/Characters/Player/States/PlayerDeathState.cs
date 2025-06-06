using System.Collections;
using UnityEngine;

public class PlayerDeathState : PlayerState
{
    public override void StartState(PlayerStateMachine owner)
    {
        owner.player.IsDead = true;
        owner.HandleDeathClientRPC(false);
        StartCoroutine(Delay(owner));
    }

    IEnumerator Delay(PlayerStateMachine owner)
    {
        yield return new WaitForSeconds(1);
        owner.HandleDeathClientRPC(true);
        owner.SetState(PlayerStateMachine.State.Spawn);

        yield return new WaitForSeconds(1);
        owner.player.GiveHeal(100, HealType.Percentage);
    }

    public override void FixedUpdateState(PlayerStateMachine owner)
    {

    }
    public override void UpdateState(PlayerStateMachine owner)
    {

    }
}
