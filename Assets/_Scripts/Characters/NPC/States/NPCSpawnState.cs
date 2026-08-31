using System.Collections;
using UnityEngine;

public class NPCSpawnState : NPCState
{
    public NPCSpawnState(NPCStateMachine owner) : base(owner) { }

    public override void EnterState()
    {
        // Spawn Event
        owner.OnSpawn?.Invoke();

        // Clear Targets
        owner.Target = null;
        owner.SecondTarget = null;
        owner.IsEnemyInRange = false;

        // Play Spawn Animation
        owner.Animator.PlayAnimation("Idle", owner.npc.Data.ChestIndex, owner.npc.Data.LegsIndex, owner.npc.Data.WeaponType);
        Vector2 dir = new Vector2(0, -1);
        owner.Animator.SetDirection(dir);

        //owner.HeadAnimator.Play("Spawn");
        //owner.BodyAnimator.Play("Spawn");
        //owner.ChestAnimator.Play("Spawn");
        //owner.LegsAnimator.Play("Spawn");
        //owner.SwordAnimator.Play(owner.npc.Data.WeaponType.ToString() + " Spawn");

        // Face Down
        //owner.HeadAnimator.SetFloat("Vertical", -1);
        //owner.BodyAnimator.SetFloat("Vertical", -1);
        //owner.ChestAnimator.SetFloat("Vertical", -1);
        //owner.LegsAnimator.SetFloat("Vertical", -1);
        //owner.SwordAnimator.SetFloat("Vertical", -1);

        // Start Duration Coroutine
        owner.StartCoroutine(Duration(owner));
    }

    IEnumerator Duration(NPCStateMachine owner)
    {
        yield return new WaitForSeconds(.2f);
        owner.TransitionToIdle();
    }
}
