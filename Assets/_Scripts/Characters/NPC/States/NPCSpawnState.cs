using System.Collections;
using UnityEngine;

public class NPCSpawnState : NPCState
{
    public override void StartState(NPCStateMachine owner)
    {
        owner.Target = null;
        owner.IsEnemyInRange = false;

        owner.HeadAnimator.SetFloat("Vertical", -1);
        owner.BodyAnimator.SetFloat("Vertical", -1);

        owner.ChestAnimator.SetFloat("Vertical", -1);
        owner.LegsAnimator.SetFloat("Vertical", -1);

        owner.SwordAnimator.SetFloat("Vertical", -1);


        owner.HeadAnimator.Play("Spawn");
        owner.BodyAnimator.Play("Spawn");

        owner.ChestAnimator.Play("Spawn_" + owner.npc.Data.ChestIndex);
        owner.LegsAnimator.Play("Spawn_" + owner.npc.Data.LegsIndex);

        owner.SwordAnimator.Play("Spawn");

        owner.StartCoroutine(Duration(owner));
    }

    public override void UpdateState(NPCStateMachine owner)
    {

    }

    public override void FixedUpdateState(NPCStateMachine owner)
    {

    }

    IEnumerator Duration(NPCStateMachine owner)
    {
        yield return new WaitForSeconds(.6f);
        owner.SetState(NPCStateMachine.State.Idle);
    }
}
