using UnityEngine;

public class NPCResetState : NPCState
{
    public NPCResetState(NPCStateMachine owner) : base(owner) { }

    public override void EnterState()
    {
        owner.isResetting = true;

        // Animate
        owner.Animator.PlayAnimation("Run", owner.npc.Data.ChestIndex, owner.npc.Data.LegsIndex, owner.npc.Data.WeaponType);

        //owner.HeadAnimator.Play("Run");
        //owner.BodyAnimator.Play("Run");
        //owner.ChestAnimator.Play("Run_" + owner.npc.Data.ChestIndex);
        //owner.LegsAnimator.Play("Run_" + owner.npc.Data.LegsIndex);
        //owner.SwordAnimator.Play(owner.npc.Data.WeaponType.ToString() + " Run");

        owner.npc.PatienceBar.Patience.Value = owner.npc.Data.TotalPatience;

        owner.IsEnemyInRange = false;
        owner.Target = null;
        owner.SecondTarget = null;

        if (owner.npc.stats.net_CurrentHP.Value < owner.npc.stats.net_TotalHP.Value)
        {
            owner.npc.IsRegen = true;
            owner.Buffs.regeneration.StartRegen(1, -1);
        }
    }

    public override void UpdateState()
    {
        if (Vector2.Distance(owner.transform.position, owner.StartingPosition) <= 0.1f)
        {
            owner.isResetting = false;
            owner.npc.PatienceBar.Patience.Value = 0;

            // Face down once reset
            //Vector2 faceDown = new Vector2(0, -1);
            //owner.Animator.SetDirection(faceDown);

            //owner.HeadAnimator.SetFloat("Vertical", -1);
            //owner.BodyAnimator.SetFloat("Vertical", -1);
            //owner.ChestAnimator.SetFloat("Vertical", -1);
            //owner.LegsAnimator.SetFloat("Vertical", -1);
            //owner.SwordAnimator.SetFloat("Vertical", -1);

            owner.NpcRB.linearVelocity = Vector2.zero;
            owner.TransitionToIdle();
        }
    }

    public override void FixedUpdateState()
    {
        owner.MoveTowardsTarget(owner.StartingPosition);

        Vector2 direction = (owner.StartingPosition - (Vector2)owner.transform.position).normalized;

        //Vector2 snappedDir = owner.SnapDirection(direction);
        //owner.SetAnimDir(snappedDir);
        //owner.npc.npcHead.SetEyes(snappedDir);
        //owner.npc.npcHead.SetHair(snappedDir);
        //owner.npc.npcHead.SetHelm(snappedDir);

        Vector2 snappedDir = owner.Animator.SnapDirection(direction);
        owner.Animator.SetDirection(snappedDir);
        owner.npc.npcHead.SetHead(snappedDir);
    }
}
