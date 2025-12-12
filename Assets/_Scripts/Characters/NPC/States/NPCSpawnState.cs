using System.Collections;
using UnityEngine;

public class NPCSpawnState : NPCState
{
    public override void StartState(NPCStateMachine owner)
    {
        owner.BodyAnimator.SetFloat("Vertical", -1);
        owner.HairAnimator.SetFloat("Vertical", -1);
        owner.EyesAnimator.SetFloat("Vertical", -1);
        owner.SwordAnimator.SetFloat("Vertical", -1);

        owner.SwordAnimator.Play("Spawn");
        owner.BodyAnimator.Play("Spawn");
        owner.EyesAnimator.Play("Spawn");
        owner.HairAnimator.Play("Spawn");
        owner.HeadAnimator.Play("Spawn_" + owner.npc.Data.HeadIndex);
        owner.ChestAnimator.Play("Spawn_" + owner.npc.Data.ChestIndex);
        owner.LegsAnimator.Play("Spawn_" + owner.npc.Data.LegsIndex);

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
