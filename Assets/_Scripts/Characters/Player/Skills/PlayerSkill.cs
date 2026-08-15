using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSkill
{
    public PlayerSkill(SkillData data)
    {
        skillData = data;
    }

    public SkillData skillData;
    public SkillContext context;

    public enum State { Cast, Action, Impact, Recovery, Done }
    [HideInInspector] public State currentState;

    [Header("StateTimer")]
    [HideInInspector] public float StateTimer;
    [HideInInspector] public float ModifiedCastTime;
    [HideInInspector] public float ModifiedRecoveryTime;

    public virtual void StartSkill(PlayerStateMachine owner)
    {
        owner.CurrentSkill = this;

        // Build Context
        context = new SkillContext
        {
            SpawnPosition = owner.transform.position,
            AimDirection = owner.Aimer.right,
            AimRotation = owner.Aimer.rotation,
            AimOffset = ((Vector2)owner.Aimer.right).normalized * skillData.SkillRange,
            AttackerDamage = owner.Stats.TotalDamage,
            IsBasic = IsBasicAttack(),
            AttackerId = owner.OwnerClientId
        };

        if (IsBasicAttack())
        {
            ModifiedCastTime = skillData.CastTime / owner.Stats.TotalAS;
            ModifiedRecoveryTime = skillData.RecoveryTime / owner.Stats.TotalAS;
        }

        // Stop Moving
        owner.PlayerRB.linearVelocity = Vector2.zero;

        // Handle Aim

        // Handle Animations
        Vector2 snappedDirection = owner.SnapDirection(context.AimDirection);
        owner.SetAnimDir(snappedDirection);

        owner.StartCoroutine(CoolDownn(skillData.skillType, skillData.CoolDown, owner));
        ChangeState(State.Cast, ModifiedCastTime);
        CastState(owner);
    }

    public virtual void UpdateSkill(PlayerStateMachine owner)
    {
        if (currentState == State.Done) return;

        StateTimer -= Time.deltaTime;
        if (skillData.ActionTime > 0)
        {
            if (StateTimer <= 0f) StateTransition(owner, true);
        }
        else
        {
            if (StateTimer <= 0f) StateTransition(owner);
        }
    }
    public virtual void FixedUpdateSkill(PlayerStateMachine owner)
    {

    }

    public virtual void CastState(PlayerStateMachine owner)
    {
        Animate(owner, skillData.weaponType, skillData.skillType, State.Cast);
        owner.player.CastBar.StartCast(ModifiedCastTime);
        RunEffects(skillData.OnCastEffects, owner);
    }
    public virtual void ActionState(PlayerStateMachine owner)
    {
        if (skillData.ActionTime <= 0) return;
        Animate(owner, skillData.weaponType, skillData.skillType, State.Action);
        RunEffects(skillData.OnActionEffects, owner);
    }
    public virtual void ImpactState(PlayerStateMachine owner)
    {
        Animate(owner, skillData.weaponType, skillData.skillType, State.Impact);
        RunEffects(skillData.OnImpactEffects, owner);
    }
    public virtual void RecoveryState(PlayerStateMachine owner)
    {
        Animate(owner, skillData.weaponType, skillData.skillType, State.Recovery);
        owner.player.CastBar.StartRecovery(ModifiedRecoveryTime);
        RunEffects(skillData.OnRecoveryEffects, owner);
    }
    public void DoneState(bool isStaggered, PlayerStateMachine owner)
    {
        currentState = State.Done;
        owner.IsAttacking = false;
        owner.CurrentSkill = null;

        if (isStaggered)
        {
            owner.SetState(new PlayerStaggerState(owner));

        }
        else
        {
            if (owner.IsFullySpawned) owner.SetState(new PlayerIdleState(owner));
        }
    }

    protected void StateTransition(PlayerStateMachine owner, bool hasAction = false)
    {
        switch (currentState)
        {
            case State.Cast:
                if (hasAction)
                {
                    ActionState(owner);
                    ChangeState(State.Action, skillData.ActionTime);
                }
                else
                {
                    ImpactState(owner);
                    ChangeState(State.Impact, skillData.ImpactTime);
                }
                break;

            case State.Action:
                ImpactState(owner);
                ChangeState(State.Impact, skillData.ImpactTime);
                break;

            case State.Impact:
                RecoveryState(owner);
                if (skillData.skillType == SkillData.SkillType.Basic)
                {
                    ChangeState(State.Recovery, ModifiedRecoveryTime);
                }
                else
                {
                    ChangeState(State.Recovery, skillData.RecoveryTime);
                }
                break;

            case State.Recovery:
                DoneState(false, owner);
                break;
        }
    }
    protected void ChangeState(State next, float duration)
    {
        currentState = next;
        StateTimer = duration;
    }

    IEnumerator CoolDownn(SkillData.SkillType type, float coolDown, PlayerStateMachine owner)
    {
        float modifiedCooldown = coolDown / owner.Stats.TotalCDR;
        owner.coolDownTracker.SkillCoolDown(skillData.skillType, modifiedCooldown);

        yield return new WaitForSeconds(modifiedCooldown);

        switch (type)
        {
            case SkillData.SkillType.Basic: owner.CanBasic = true; break;
            case SkillData.SkillType.Offensive: owner.CanOffensive = true; break;
            case SkillData.SkillType.Mobility: owner.CanMobility = true; break;
            case SkillData.SkillType.Defensive: owner.CanDefensive = true; break;
            case SkillData.SkillType.Utility: owner.CanUtility = true; break;
            case SkillData.SkillType.Ultimate: owner.CanUltimate = true; break;
        }
    }
    protected void Animate(PlayerStateMachine owner, WeaponType weapon, SkillData.SkillType type, State state)
    {
        string _weapon = "";
        string _skill = "";
        string _state = "";

        switch (weapon)
        {
            case WeaponType.Sword: _weapon = weapon.ToString(); break;
            case WeaponType.Staff: _weapon = weapon.ToString(); break;
            case WeaponType.Bow: _weapon = weapon.ToString(); break;
            case WeaponType.Dagger: _weapon = weapon.ToString(); break;
        }

        switch (type)
        {
            case SkillData.SkillType.Basic: _skill = "Basic"; break;
            case SkillData.SkillType.Offensive: _skill = "Offensive"; break;
            case SkillData.SkillType.Mobility: _skill = "Mobility"; break;
            case SkillData.SkillType.Defensive: _skill = "Defensive"; break;
            case SkillData.SkillType.Utility: _skill = "Utility"; break;
            case SkillData.SkillType.Ultimate: _skill = "Ultimate"; break;
        }

        switch (state)
        {
            case State.Cast: _state = "Cast"; break;
            case State.Action: _state = "Action"; break;
            case State.Impact: _state = "Impact"; break;
            case State.Recovery: _state = "Recovery"; break;
            case State.Done: _state = "Done"; break;
        }

        owner.PlayerHeadAnimator.Play(_weapon + " " + _skill + " " + "Front" + " " + _state);
        owner.BodyAnimator.Play(_weapon + " " + _skill + " " + "Front" + " " + _state);

        owner.ChestAnimator.Play(_weapon + " " + _skill + " " + "Front" + " " + _state + " " + owner.customization.net_ChestIndex.Value);
        owner.LegsAnimator.Play(_weapon + " " + _skill + " " + "Front" + " " + _state + " " + owner.customization.net_LegsIndex.Value);

        owner.WeaponAnimator.Play(_weapon + " " + _skill + " " + "Front" + " " + _state);
    }

    void RunEffects(SkillEffect[] effects, PlayerStateMachine owner)
    {
        if (effects == null) return;
        foreach (SkillEffect effect in effects)
        {
            effect.Execute(owner, context);
        }
    }

    bool IsBasicAttack()
    {
        if (skillData.skillType == SkillData.SkillType.Basic)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
