using UnityEngine;

public class FlailingEdge : PlayerSkill
{
    public override void StartSkill(PlayerStateMachine owner)
    {
        InitializeAbility(skillType, owner);

        // Aim
        AimDirection = owner.Aimer.right;
        AimRotation = owner.Aimer.rotation;
        AimOffset = AimDirection.normalized * skillData.SkillRange;

        // Animation Direction
        Vector2 snappedDirection = owner.SnapDirection(AimDirection);
        owner.SetAnimDir(snappedDirection);

        ChangeState(State.Cast, skillData.CastTime);
        CastState(owner);
    }

    public override void FixedUpdateSkill(PlayerStateMachine owner)
    {
        if (!owner.IsSliding) return;

        owner.PlayerRB.linearVelocity = AimDirection * skillData.SlideForce;
        StartCoroutine(owner.SlideDuration(AimDirection, skillData.SlideForce, skillData.SlideDuration));
    }

    public override void CastState(PlayerStateMachine owner)
    {
        owner.StartSlide(true);
        Animate(owner, weaponType, SkillType.Basic, State.Cast);
        owner.player.CastBar.StartCast(skillData.CastTime);
    }

    public override void ImpactState(PlayerStateMachine owner)
    {
        Animate(owner, weaponType, SkillType.Basic, State.Impact);

        if (owner.IsServer)
        {
            Attack(OwnerClientId);
        }
        else
        {
            AttackServerRpc(IsBasic,SpawnPosition, AimOffset, AimDirection, AimRotation, AttackerDamage, OwnerClientId);
        }
    }

    public override void RecoveryState(PlayerStateMachine owner)
    {
        Animate(owner, weaponType, SkillType.Basic, State.Recovery);
        owner.player.CastBar.StartRecovery(skillData.RecoveryTime);
    }
}