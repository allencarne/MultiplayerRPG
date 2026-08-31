using UnityEngine;

public class BasicAttack : NPCSkill
{
    public override void StartSkill(NPCStateMachine owner)
    {
        InitializeAbility(skillData.skillType, owner);

        // Aim
        AimDirection = (owner.Target.position - transform.position).normalized;
        float angle = Mathf.Atan2(AimDirection.y, AimDirection.x) * Mathf.Rad2Deg;
        AimRotation = Quaternion.Euler(0, 0, angle);
        AimOffset = AimDirection.normalized * skillData.SkillRange;

        //Vector2 snappedDirection = owner.SnapDirection(AimDirection);
        //owner.SetAnimDir(snappedDirection);

        // Animation Direction
        Vector2 snappedDirection = owner.Animator.SnapDirection(AimDirection);
        owner.Animator.SetDirection(snappedDirection);

        ChangeState(ActiveSkillData.SkillPhase.Cast, ModifiedCastTime);
        CastState(owner);
    }

    public override void CastState(NPCStateMachine owner)
    {
        owner.Animator.PlayAttackAnimation(skillData.weaponType, skillData.skillType, ActiveSkillData.SkillPhase.Cast, owner.npc.Data.ChestIndex, owner.npc.Data.LegsIndex);
        //Animate(owner, skillData.weaponType, skillData.skillType, State.Cast);
        owner.npc.CastBar.StartCast(ModifiedCastTime);
    }

    public override void ImpactState(NPCStateMachine owner)
    {
        owner.Animator.PlayAttackAnimation(skillData.weaponType, skillData.skillType, ActiveSkillData.SkillPhase.Impact, owner.npc.Data.ChestIndex, owner.npc.Data.LegsIndex);
        //Animate(owner, skillData.weaponType, skillData.skillType, State.Impact);

        if (owner.IsServer)
        {
            Attack();
        }
        else
        {
            AttackServerRpc(SpawnPosition, AimOffset, AimDirection, AimRotation, AttackerDamage);
        }
    }

    public override void RecoveryState(NPCStateMachine owner)
    {
        owner.Animator.PlayAttackAnimation(skillData.weaponType, skillData.skillType, ActiveSkillData.SkillPhase.Recovery, owner.npc.Data.ChestIndex, owner.npc.Data.LegsIndex);
        //Animate(owner, skillData.weaponType, skillData.skillType, State.Recovery);
        owner.npc.CastBar.StartRecovery(skillData.RecoveryTime);
    }
}
