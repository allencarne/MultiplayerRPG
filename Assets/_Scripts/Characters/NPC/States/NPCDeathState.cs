using System.Collections;
using UnityEngine;

public class NPCDeathState : NPCState
{
    public override void StartState(NPCStateMachine owner)
    {
        // Prevents attacking while dead
        owner.IsAttacking = false;

        // Clear all buffs and debuffs
        owner.Buffs.PurgeAllDebuffs();
        owner.DeBuffs.CleanseAllDebuffs();

        // Play death animation
        owner.HeadAnimator.Play("Death");
        owner.BodyAnimator.Play("Death");
        owner.ChestAnimator.Play("Death_" + owner.npc.Data.ChestIndex);
        owner.LegsAnimator.Play("Death_" + owner.npc.Data.LegsIndex);
        owner.SwordAnimator.Play(owner.npc.Data.WeaponType + " Death");

        // Face Down
        owner.HeadAnimator.SetFloat("Horizontal", 1);
        Vector2 dir = new Vector2(1, 0);
        owner.npc.npcHead.SetEyes(dir);
        owner.npc.npcHead.SetHair(dir);
        owner.npc.npcHead.SetHelm(dir);

        // Reset Patrol Index
        owner.PatrolIndex = 0;

        // Reset Cast Bar
        owner.npc.CastBar.ResetCastBar();

        // Disable colliders and sprites
        owner.SetColliderAndSprites(false);

        // Start the respawn delay coroutine
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
        yield return new WaitForSeconds(5);

        // Reset the NPC's health and status
        owner.npc.stats.isDead = false;
        owner.npc.stats.GiveHeal(100, HealType.Percentage);

        // Enable colliders and sprites
        owner.SetColliderAndSprites(true);

        // Reset position to starting position
        owner.transform.position = owner.StartingPosition;

        // Reset the NPC's state to Spawn
        owner.SetState(NPCStateMachine.State.Spawn);
    }
}
