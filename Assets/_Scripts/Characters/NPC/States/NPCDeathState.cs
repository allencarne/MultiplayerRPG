using System.Collections;
using UnityEngine;

public class NPCDeathState : NPCState
{
    public override void StartState(NPCStateMachine owner)
    {
        if (!owner.IsOwner) return;

        owner.BodyAnimator.Play("Death");
        owner.npc.IsDead = true;
        owner.IsAttacking = false;

        owner.npc.CastBar.ForceReset();
        owner.RequestDisableColliderServerRpc();

        StartCoroutine(Delay(owner));
    }

    public override void UpdateState(NPCStateMachine owner)
    {

    }

    public override void FixedUpdateState(NPCStateMachine owner)
    {

    }

    IEnumerator Delay(NPCStateMachine owner)
    {
        yield return new WaitForSeconds(4);

        owner.npc.IsDead = false;
        owner.transform.position = owner.StartingPosition;

        yield return new WaitForSeconds(1);

        owner.RequestRespawnServerRpc();
        owner.RequestEnableColliderServerRpc();

        owner.SetState(NPCStateMachine.State.Spawn);
    }
}
