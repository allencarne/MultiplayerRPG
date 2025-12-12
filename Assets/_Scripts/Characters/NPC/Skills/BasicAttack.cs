using UnityEngine;

public class BasicAttack : NPCSkill
{
    public override void StartSkill(NPCStateMachine owner)
    {
        InitializeAbility(skillType, owner);

        // Basic
        ModifiedCastTime = CastTime / owner.npc.stats.AttackSpeed;

        // Aim
        AimDirection = (owner.Target.position - transform.position).normalized;
        float angle = Mathf.Atan2(AimDirection.y, AimDirection.x) * Mathf.Rad2Deg;
        AimRotation = Quaternion.Euler(0, 0, angle);
        AimOffset = AimDirection.normalized * SkillRange;

        // Animation Direction
        Vector2 snappedDirection = owner.SnapDirection(AimDirection);
        owner.SetAnimDir(snappedDirection);

        ChangeState(State.Cast, ModifiedCastTime);
        CastState(owner);
    }

    public override void CastState(NPCStateMachine owner)
    {
        Animate(owner, weaponType, skillType, State.Cast);
        owner.npc.CastBar.StartCast(ModifiedCastTime);
    }

    public override void ImpactState(NPCStateMachine owner)
    {
        Animate(owner, weaponType, skillType, State.Impact);

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
        Animate(owner, weaponType, skillType, State.Recovery);
        owner.npc.CastBar.StartRecovery(RecoveryTime);
    }
}
