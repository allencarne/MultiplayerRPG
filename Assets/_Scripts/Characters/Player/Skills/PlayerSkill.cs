using System.Collections;
using UnityEngine;

public class PlayerSkill
{
    public ActiveSkillData skillData;
    int skillIndex;
    public SkillContext context;
    public Vector2? GroundTargetPosition = null;

    protected int repeatCycleIndex = 0;
    protected int totalRepeatCycles = 1;
    protected float repeatInterval = 0f;

    public PlayerSkill(ActiveSkillData data, int index)
    {
        skillData = data;
        skillIndex = index;
    }

    //public enum State { Cast, Action, Impact, Recovery, Done }
    //[HideInInspector] public State currentState;

    [HideInInspector] public ActiveSkillData.SkillPhase currentState;

    [Header("StateTimer")]
    [HideInInspector] public float StateTimer;
    [HideInInspector] public float ModifiedCastTime;
    [HideInInspector] public float ModifiedRecoveryTime;

    public virtual void StartSkill(PlayerStateMachine owner)
    {
        owner.CurrentSkill = this;

        if (IsBasicAttack())
        {
            ModifiedCastTime = skillData.CastTime / owner.Stats.TotalAS;
            ModifiedRecoveryTime = skillData.RecoveryTime / owner.Stats.TotalAS;
        }
        else
        {
            ModifiedCastTime = skillData.CastTime;
            ModifiedRecoveryTime = skillData.RecoveryTime;
        }

        // Handle Aim
        Vector2 aimDirection = owner.Aimer.right;
        Quaternion aimRotation = owner.Aimer.rotation;
        Vector2 spawnPos = owner.transform.position;
        Vector2 aimOffset = ((Vector2)owner.Aimer.right).normalized * skillData.SkillRange;

        if (GroundTargetPosition.HasValue)
        {
            Vector2 target = GroundTargetPosition.Value;
            spawnPos = target;
            Vector2 dir = (target - (Vector2)owner.transform.position);
            if (dir.sqrMagnitude > 0.0001f)
            {
                aimDirection = dir.normalized;
            }

            float ang = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
            aimRotation = Quaternion.Euler(0, 0, ang);
            aimOffset = Vector2.zero;
        }

        // Build Context
        context = new SkillContext
        {
            SpawnPosition = spawnPos,
            AimDirection = aimDirection,
            AimRotation = aimRotation,
            AimOffset = aimOffset,
            AttackerDamage = owner.Stats.TotalDamage,
            IsBasic = IsBasicAttack(),
            AttackerId = owner.OwnerClientId,
            SkillType = skillData.skillType,
            SkillIndex = skillIndex,
            FillDuration = ModifiedCastTime + skillData.ActionTime
        };

        repeatCycleIndex = 0;
        totalRepeatCycles = 1;
        repeatInterval = 0f;

        if (skillData.ImpactStyle == ActiveSkillData.ImpactAnimationStyle.Repeated)
        {
            totalRepeatCycles = GetImpactRepeatCount();
            repeatInterval = GetImpactRepeatInterval();
        }

        // Stop Moving
        owner.PlayerRB.linearVelocity = Vector2.zero;

        //Vector2 snappedDirection = owner.SnapDirection(context.AimDirection);
        //owner.SetAnimDir(snappedDirection);

        // Handle Animations
        Vector2 snappedDirection = owner.Animator.SnapDirection(context.AimDirection);
        owner.Animator.SetDirection(snappedDirection);

        // Head Animator
        owner.customization.net_FacingDirection.Value = snappedDirection;
        owner.playerHead.SetHead(snappedDirection);

        owner.StartCoroutine(CoolDownn(skillData.skillType, skillData.CoolDown, owner));
        ChangeState(ActiveSkillData.SkillPhase.Cast, ModifiedCastTime);
        CastState(owner);
    }

    public virtual void UpdateSkill(PlayerStateMachine owner)
    {
        if (currentState == ActiveSkillData.SkillPhase.Done) return;

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
        //Animate(owner, skillData.weaponType, ActiveSkillData.SkillType.Basic, ActiveSkillData.SkillPhase.Cast);
        owner.Animator.PlayAttackAnimation(skillData.weaponType, ActiveSkillData.SkillType.Basic, ActiveSkillData.SkillPhase.Cast, owner.customization.net_ChestIndex.Value, owner.customization.net_LegsIndex.Value);
        owner.player.CastBar.StartCast(ModifiedCastTime);
        RunEffects(skillData.OnCastEffects, owner, ActiveSkillData.SkillPhase.Cast);
    }
    public virtual void ActionState(PlayerStateMachine owner)
    {
        if (skillData.ActionTime <= 0) return;
        //Animate(owner, skillData.weaponType, ActiveSkillData.SkillType.Basic, ActiveSkillData.SkillPhase.Action);
        owner.Animator.PlayAttackAnimation(skillData.weaponType, ActiveSkillData.SkillType.Basic, ActiveSkillData.SkillPhase.Action, owner.customization.net_ChestIndex.Value, owner.customization.net_LegsIndex.Value);
        RunEffects(skillData.OnActionEffects, owner, ActiveSkillData.SkillPhase.Action);
    }
    public virtual void ImpactState(PlayerStateMachine owner, bool fireEffects = true)
    {
        //Animate(owner, skillData.weaponType, ActiveSkillData.SkillType.Basic, ActiveSkillData.SkillPhase.Impact);
        owner.Animator.PlayAttackAnimation(skillData.weaponType, ActiveSkillData.SkillType.Basic, ActiveSkillData.SkillPhase.Impact, owner.customization.net_ChestIndex.Value, owner.customization.net_LegsIndex.Value);
        if (fireEffects) RunEffects(skillData.OnImpactEffects, owner, ActiveSkillData.SkillPhase.Impact);
    }
    public virtual void RecoveryState(PlayerStateMachine owner, bool fireEffects = true)
    {
        //Animate(owner, skillData.weaponType, ActiveSkillData.SkillType.Basic, ActiveSkillData.SkillPhase.Recovery);
        owner.Animator.PlayAttackAnimation(skillData.weaponType, ActiveSkillData.SkillType.Basic, ActiveSkillData.SkillPhase.Recovery, owner.customization.net_ChestIndex.Value, owner.customization.net_LegsIndex.Value);
        if (fireEffects)
        {
            owner.player.CastBar.StartRecovery(ModifiedRecoveryTime);
            RunEffects(skillData.OnRecoveryEffects, owner, ActiveSkillData.SkillPhase.Recovery);
        }
    }
    public void DoneState(bool isStaggered, PlayerStateMachine owner)
    {
        currentState = ActiveSkillData.SkillPhase.Done;
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
            case ActiveSkillData.SkillPhase.Cast:
                if (hasAction)
                {
                    ActionState(owner);
                    ChangeState(ActiveSkillData.SkillPhase.Action, skillData.ActionTime);
                }
                else
                {
                    ImpactState(owner);
                    ChangeState(ActiveSkillData.SkillPhase.Impact, GetImpactStateDuration());
                }
                break;

            case ActiveSkillData.SkillPhase.Action:
                ImpactState(owner);
                ChangeState(ActiveSkillData.SkillPhase.Impact, GetImpactStateDuration());
                break;

            case ActiveSkillData.SkillPhase.Impact:
                if (skillData.ImpactStyle == ActiveSkillData.ImpactAnimationStyle.Repeated)
                {
                    bool moreCyclesRemain = repeatCycleIndex < totalRepeatCycles - 1;

                    if (moreCyclesRemain)
                    {
                        RecoveryState(owner, fireEffects: false);
                        float gapDuration = Mathf.Max(0f, repeatInterval - skillData.ImpactTime);
                        ChangeState(ActiveSkillData.SkillPhase.Recovery, gapDuration);
                    }
                    else
                    {
                        RecoveryState(owner, fireEffects: true);
                        float recoveryDuration = skillData.skillType == ActiveSkillData.SkillType.Basic ? ModifiedRecoveryTime : skillData.RecoveryTime;
                        ChangeState(ActiveSkillData.SkillPhase.Recovery, recoveryDuration);
                    }
                }
                else
                {
                    RecoveryState(owner);
                    float recoveryDuration = skillData.skillType == ActiveSkillData.SkillType.Basic ? ModifiedRecoveryTime : skillData.RecoveryTime;
                    ChangeState(ActiveSkillData.SkillPhase.Recovery, recoveryDuration);
                }
                break;

            case ActiveSkillData.SkillPhase.Recovery:
                if (skillData.ImpactStyle == ActiveSkillData.ImpactAnimationStyle.Repeated && repeatCycleIndex < totalRepeatCycles - 1)
                {
                    repeatCycleIndex++;
                    ImpactState(owner, fireEffects: false);
                    ChangeState(ActiveSkillData.SkillPhase.Impact, skillData.ImpactTime);
                }
                else
                {
                    DoneState(false, owner);
                }
                break;
        }
    }
    protected void ChangeState(ActiveSkillData.SkillPhase next, float duration)
    {
        currentState = next;
        StateTimer = duration;
    }

    IEnumerator CoolDownn(ActiveSkillData.SkillType type, float coolDown, PlayerStateMachine owner)
    {
        float modifiedCooldown = coolDown / owner.Stats.TotalCDR;

        foreach (SkillBarUI bar in owner.coolDownTracker)
        {
            if (bar == null) continue;
            if (!bar.gameObject.activeInHierarchy) continue;
            bar.SkillCoolDown(skillData.skillType, modifiedCooldown);
        }

        yield return new WaitForSeconds(modifiedCooldown);

        switch (type)
        {
            case ActiveSkillData.SkillType.Basic: owner.CanBasic = true; break;
            case ActiveSkillData.SkillType.Offensive: owner.CanOffensive = true; break;
            case ActiveSkillData.SkillType.Mobility: owner.CanMobility = true; break;
            case ActiveSkillData.SkillType.Defensive: owner.CanDefensive = true; break;
            case ActiveSkillData.SkillType.Utility: owner.CanUtility = true; break;
            case ActiveSkillData.SkillType.Ultimate: owner.CanUltimate = true; break;
        }
    }

    /*
    protected void Animate(PlayerStateMachine owner, WeaponType weapon, ActiveSkillData.SkillType type, ActiveSkillData.SkillPhase state)
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
            case ActiveSkillData.SkillType.Basic: _skill = "Basic"; break;
            case ActiveSkillData.SkillType.Offensive: _skill = "Offensive"; break;
            case ActiveSkillData.SkillType.Mobility: _skill = "Mobility"; break;
            case ActiveSkillData.SkillType.Defensive: _skill = "Defensive"; break;
            case ActiveSkillData.SkillType.Utility: _skill = "Utility"; break;
            case ActiveSkillData.SkillType.Ultimate: _skill = "Ultimate"; break;
        }

        switch (state)
        {
            case ActiveSkillData.SkillPhase.Cast: _state = "Cast"; break;
            case ActiveSkillData.SkillPhase.Action: _state = "Action"; break;
            case ActiveSkillData.SkillPhase.Impact: _state = "Impact"; break;
            case ActiveSkillData.SkillPhase.Recovery: _state = "Recovery"; break;
            case ActiveSkillData.SkillPhase.Done: _state = "Done"; break;
        }

        owner.PlayerHeadAnimator.Play(_weapon + " " + _skill + " " + "Front" + " " + _state);
        owner.BodyAnimator.Play(_weapon + " " + _skill + " " + "Front" + " " + _state);

        owner.ChestAnimator.Play(_weapon + " " + _skill + " " + "Front" + " " + _state + " " + owner.customization.net_ChestIndex.Value);
        owner.LegsAnimator.Play(_weapon + " " + _skill + " " + "Front" + " " + _state + " " + owner.customization.net_LegsIndex.Value);

        owner.WeaponAnimator.Play(_weapon + " " + _skill + " " + "Front" + " " + _state);
    }
    */

    void RunEffects(SkillEffect[] effects, PlayerStateMachine owner, ActiveSkillData.SkillPhase phase)
    {
        if (effects == null) return;

        for (int i = 0; i < effects.Length; i++)
        {
            SkillContext effectCtx = context;
            effectCtx.Phase = phase;
            effectCtx.EffectIndex = i;
            effects[i].Execute(owner, effectCtx);
        }
    }

    bool IsBasicAttack()
    {
        if (skillData.skillType == ActiveSkillData.SkillType.Basic)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    float GetImpactStateDuration()
    {
        if (skillData.ImpactStyle == ActiveSkillData.ImpactAnimationStyle.Long)
            return GetStretchedImpactDuration();

        // Normal and Repeated both use a single beat's worth of time —
        // Repeated just plays that beat multiple times instead of once.
        return skillData.ImpactTime;
    }

    float GetStretchedImpactDuration()
    {
        float duration = skillData.ImpactTime;
        if (skillData.OnImpactEffects != null)
        {
            foreach (SkillEffect effect in skillData.OnImpactEffects)
            {
                if (effect == null) continue;
                int count = effect.GetRepeatCount();
                if (count > 1)
                    duration = Mathf.Max(duration, (count - 1) * effect.GetRepeatInterval());
            }
        }
        return duration;
    }

    int GetImpactRepeatCount()
    {
        int count = 1;
        if (skillData.OnImpactEffects != null)
        {
            foreach (SkillEffect effect in skillData.OnImpactEffects)
            {
                if (effect == null) continue;
                count = Mathf.Max(count, effect.GetRepeatCount());
            }
        }
        return count;
    }

    float GetImpactRepeatInterval()
    {
        float interval = 0f;
        if (skillData.OnImpactEffects != null)
        {
            foreach (SkillEffect effect in skillData.OnImpactEffects)
            {
                if (effect == null) continue;
                if (effect.GetRepeatCount() > 1)
                    interval = Mathf.Max(interval, effect.GetRepeatInterval());
            }
        }
        return interval;
    }
}
