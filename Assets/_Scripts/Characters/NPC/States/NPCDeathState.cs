using System.Collections;
using UnityEngine;

public class NPCDeathState : NPCState
{
    public override void StartState(NPCStateMachine owner)
    {
        owner.Buffs.PurgeAllDebuffs();
        owner.DeBuffs.CleanseAllDebuffs();

        owner.BodyAnimator.Play("Death");
        owner.IsAttacking = false;

        // Patrol
        owner.PatrolIndex = 0;

        owner.npc.CastBar.ResetCastBar();

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
        owner.transform.position = owner.StartingPosition;
        yield return new WaitForSeconds(1);
        owner.npc.stats.isDead = false;
        owner.npc.stats.GiveHeal(100, HealType.Percentage);
        owner.SetColliderAndSprites(false);
        yield return null;
        owner.SetColliderAndSprites(true);
        owner.SetState(NPCStateMachine.State.Spawn);
    }
}
