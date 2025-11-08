using UnityEngine;

public class BasicAttack : NPCSkill
{
    public override void StartSkill(NPCStateMachine owner)
    {
        InitializeAbility(skillType, owner);
        owner.NpcRB.linearVelocity = Vector2.zero;
        ModifiedCastTime = CastTime / owner.npc.CurrentAttackSpeed;
        SpawnPosition = owner.transform.position;

        // Aim
        AimDirection = (owner.Target.position - transform.position).normalized;
        float angle = Mathf.Atan2(AimDirection.y, AimDirection.x) * Mathf.Rad2Deg;
        AimRotation = Quaternion.Euler(0, 0, angle);
        AimOffset = AimDirection.normalized * SkillRange;

        ChangeState(State.Cast, ModifiedCastTime);
        CastState(owner);
    }

    public override void UpdateSkill(NPCStateMachine owner)
    {
        if (currentState == State.Done) return;

        StateTimer -= Time.deltaTime;
        if (StateTimer <= 0f) StateTransition(owner);
    }

    public override void CastState(NPCStateMachine owner)
    {
        Animate(owner, weaponType, skillType, State.Cast);
        owner.BodyAnimator.SetFloat("Horizontal", AimDirection.x);
        owner.BodyAnimator.SetFloat("Vertical", AimDirection.y);

        owner.HairAnimator.SetFloat("Horizontal", AimDirection.x);
        owner.HairAnimator.SetFloat("Vertical", AimDirection.y);

        owner.EyesAnimator.SetFloat("Horizontal", AimDirection.x);
        owner.EyesAnimator.SetFloat("Vertical", AimDirection.y);

        owner.SwordAnimator.SetFloat("Horizontal", AimDirection.x);
        owner.SwordAnimator.SetFloat("Vertical", AimDirection.y);

        owner.npc.CastBar.StartCast(CastTime, owner.npc.CurrentAttackSpeed);
        Telegraph(true, false);
    }

    public override void ImpactState(NPCStateMachine owner)
    {
        Animate(owner, weaponType, skillType, State.Impact);

        if (owner.IsServer)
        {
            Debug.Log("Server");
            Attack(true, false);
        }
        else
        {
            Debug.Log("Not Server");
            AttackServerRpc(AimDirection,AimRotation, true,false);
        }
    }

    public override void RecoveryState(NPCStateMachine owner)
    {
        Animate(owner, weaponType, skillType, State.Recovery);
        owner.npc.CastBar.StartRecovery(RecoveryTime, owner.npc.CurrentAttackSpeed);
    }
}
