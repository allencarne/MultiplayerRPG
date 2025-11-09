using UnityEngine;

public class FrailSlash : PlayerSkill
{
    public override void StartSkill(PlayerStateMachine owner)
    {
        InitializeAbility(skillType, owner);

        // Stop
        owner.PlayerRB.linearVelocity = Vector2.zero;
        SpawnPosition = owner.transform.position;

        // Cast Time - Basic Attack Only
        ModifiedCastTime = CastTime / owner.player.CurrentAttackSpeed.Value;

        // Aim
        AimDirection = owner.Aimer.right;
        AimRotation = owner.Aimer.rotation;
        AimOffset = AimDirection.normalized * SkillRange;

        // Animation Direction
        Vector2 snappedDirection = owner.SnapDirection(AimDirection);
        owner.SetAnimDir(snappedDirection);

        ChangeState(State.Cast, ModifiedCastTime);
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
        Animate(owner, weaponType, skillType, State.Cast);
        owner.player.CastBar.StartCast(CastTime, owner.player.CurrentAttackSpeed.Value);
    }

    public override void ImpactState(PlayerStateMachine owner)
    {
        owner.StartSlide(true);
        Animate(owner, weaponType, skillType, State.Impact);

        if (owner.IsServer)
        {
            Attack(true, true);
        }
        else
        {
            AttackServerRpc(SpawnPosition, AimDirection, AimRotation, true, true);
        }
    }

    public override void RecoveryState(PlayerStateMachine owner)
    {
        Animate(owner, weaponType, skillType, State.Recovery);
        owner.player.CastBar.StartRecovery(RecoveryTime, owner.player.CurrentAttackSpeed.Value);
    }
}
