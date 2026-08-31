using UnityEngine;

public class PatrolChaseState : NPCState
{
    public PatrolChaseState(NPCStateMachine owner) : base(owner) { }

    Vector2 startingPosition;

    public override void EnterState()
    {
        if (!owner.IsServer) return;

        startingPosition = owner.transform.position;

        // Animate
        owner.Animator.PlayAnimation("Run", owner.npc.Data.ChestIndex, owner.npc.Data.LegsIndex, owner.npc.Data.WeaponType);

        //owner.HeadAnimator.Play("Run");
        //owner.BodyAnimator.Play("Run");
        //owner.ChestAnimator.Play("Run_" + owner.npc.Data.ChestIndex);
        //owner.LegsAnimator.Play("Run_" + owner.npc.Data.LegsIndex);
        //owner.SwordAnimator.Play(owner.npc.Data.WeaponType.ToString() + " Run");
    }

    public override void UpdateState()
    {
        if (!owner.IsServer) return;

        if (owner.Target == null)
        {
            TransitionToIdle(owner);
            return;
        }

        HandleAttack(owner);
        HandleDeAggro(owner);
    }

    public override void FixedUpdateState()
    {
        if (!owner.IsServer) return;

        if (owner.Target)
        {
            owner.MoveTowardsTarget(owner.Target.position);

            Vector2 rawDir = (owner.Target.position - owner.transform.position).normalized;

            //Vector2 snappedDir = owner.SnapDirection(rawDir);
            //owner.SetAnimDir(snappedDir);
            //owner.npc.npcHead.SetEyes(snappedDir);
            //owner.npc.npcHead.SetHair(snappedDir);
            //owner.npc.npcHead.SetHelm(snappedDir);

            Vector2 snappedDir = owner.Animator.SnapDirection(rawDir);
            owner.Animator.SetDirection(snappedDir);
            owner.npc.npcHead.SetHead(snappedDir);
        }
    }

    public void TransitionToIdle(NPCStateMachine owner)
    {
        owner.npc.PatienceBar.Patience.Value = 0;
        owner.IsEnemyInRange = false;
        owner.Target = null;
        owner.TransitionToIdle();
    }

    public void HandleDeAggro(NPCStateMachine owner)
    {
        float distanceToStartingPosition = Vector2.Distance(startingPosition, owner.Target.position);

        if (distanceToStartingPosition > owner.DeAggroRadius)
        {
            owner.npc.PatienceBar.Patience.Value += Time.deltaTime;
            if (owner.npc.PatienceBar.Patience.Value >= owner.npc.Data.TotalPatience)
            {
                TransitionToIdle(owner);
            }
        }
        else
        {
            owner.npc.PatienceBar.Patience.Value = Mathf.Max(0, owner.npc.PatienceBar.Patience.Value - Time.deltaTime);
        }
    }

    public void HandleAttack(NPCStateMachine owner)
    {
        float distanceToTarget = Vector2.Distance(owner.transform.position, owner.Target.position);

        if (distanceToTarget <= owner.BasicRadius)
        {
            if (owner.CanBasic && !owner.CrowdControl.disarm.IsDisarmed)
            {
                owner.IsAttacking = true;
                owner.CanBasic = false;

                owner.SetSkill(NPCStateMachine.SkillType.Basic);
                return;
            }
        }
    }
}
