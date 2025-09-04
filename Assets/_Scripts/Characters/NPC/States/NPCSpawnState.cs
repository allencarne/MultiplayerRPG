using System.Collections;
using UnityEngine;

public class NPCSpawnState : NPCState
{
    public override void StartState(NPCStateMachine owner)
    {
        Instantiate(owner.npc.spawn_Effect, transform.position, transform.rotation);
        owner.SwordAnimator.Play("Spawn");
        owner.BodyAnimator.Play("Spawn");
        owner.EyesAnimator.Play("Spawn");
        owner.HairAnimator.Play("Spawn");
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
