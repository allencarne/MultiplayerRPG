using UnityEngine;

public class FrailLunge : PlayerSkill
{
    public override void StartSkill(PlayerStateMachine owner)
    {
        InitializeAbility(skillType, owner);

        // Aim
        AimDirection = owner.Aimer.right;
        AimRotation = owner.Aimer.rotation;
        AimOffset = AimDirection.normalized * SkillRange;

        // Animation Direction
        Vector2 snappedDirection = owner.SnapDirection(AimDirection);
        owner.SetAnimDir(snappedDirection);

        ChangeState(State.Cast, CastTime);
        CastState(owner);
    }

    public override void FixedUpdateSkill(PlayerStateMachine owner)
    {
        if (!owner.IsSliding) return;

        owner.PlayerRB.linearVelocity = AimDirection * SlideForce;
        StartCoroutine(owner.SlideDuration(AimDirection, SlideForce, SlideDuration));
    }

    public override void CastState(PlayerStateMachine owner)
    {
        owner.StartSlide(false);
        Animate(owner, weaponType, SkillType.Basic, State.Cast);
        owner.player.CastBar.StartCast(CastTime);
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
            AttackServerRpc(IsBasic, SpawnPosition, AimOffset, AimDirection, AimRotation, AttackerDamage, OwnerClientId);
        }
    }

    public override void RecoveryState(PlayerStateMachine owner)
    {
        Animate(owner, weaponType, SkillType.Basic, State.Recovery);
        owner.player.CastBar.StartRecovery(RecoveryTime);
    }
}
