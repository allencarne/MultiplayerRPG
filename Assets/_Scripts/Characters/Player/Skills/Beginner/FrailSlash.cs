using UnityEngine;

public class FrailSlash : PlayerSkill
{
    public override void StartSkill(PlayerStateMachine owner)
    {
        Debug.Log($"FrailSlash.StartSkill called. Frame: {Time.frameCount}, IsOwner: {IsOwner}");
        Debug.LogError(System.Environment.StackTrace);

        InitializeAbility(skillType, owner);

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
        owner.StartSlide(true);
        Animate(owner, weaponType, skillType, State.Cast);
        owner.player.CastBar.StartCast(ModifiedCastTime);
    }

    public override void ImpactState(PlayerStateMachine owner)
    {
        Animate(owner, weaponType, skillType, State.Impact);

        if (owner.IsServer)
        {
            Attack(OwnerClientId);
        }
        else
        {
            AttackServerRpc(SpawnPosition, AimOffset, AimDirection, AimRotation, AttackerDamage, OwnerClientId);
        }
    }

    public override void RecoveryState(PlayerStateMachine owner)
    {
        Animate(owner, weaponType, skillType, State.Recovery);
        owner.player.CastBar.StartRecovery(RecoveryTime);
    }
}