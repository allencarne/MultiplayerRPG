using System.Collections;
using UnityEngine;

public class NPCDeathState : NPCState
{
    public override void StartState(NPCStateMachine owner)
    {
        owner.Buffs.PurgeAllDebuffs();
        owner.DeBuffs.CleanseAllDebuffs();

        owner.BodyAnimator.Play("Death");
        owner.npc.IsDead = true;
        owner.IsAttacking = false;
        owner.Target = null;
        owner.IsEnemyInRange = false;

        // Patrol
        owner.PatrolIndex = 0;
        owner.PatrolForward = true;

        owner.npc.CastBar.ResetCastBar();
        owner.RequestDisableColliderServerRpc(false);

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
        owner.RequestDisableColliderServerRpc(true);

        owner.SetState(NPCStateMachine.State.Spawn);
    }
}
