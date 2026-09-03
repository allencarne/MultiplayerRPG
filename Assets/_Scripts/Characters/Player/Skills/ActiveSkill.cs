using System.Collections;
using UnityEngine;

public class ActiveSkill
{
    public ActiveSkill(ActiveSkillData data, int index)
    {
        skillData = data;
        skillIndex = index;
    }

    public ActiveSkillData skillData;
    int skillIndex;
    public SkillContext context;
    public Vector2? GroundTargetPosition = null;

    protected int repeatCycleIndex = 0;
    protected int totalRepeatCycles = 1;
    protected float repeatInterval = 0f;

    [HideInInspector] public ActiveSkillData.SkillPhase currentState;

    [Header("StateTimer")]
    [HideInInspector] public float StateTimer;
    [HideInInspector] public float ModifiedCastTime;
    [HideInInspector] public float ModifiedRecoveryTime;

    public virtual void StartSkill(StateMachine owner)
    {
        if (skillData == null) return;

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

        // Determine aim & spawn based on skill targeting mode and owner type.
        Vector2 spawnPos = owner.transform.position;
        Vector2 aimDirection = owner.transform.right;
        Quaternion aimRotation = owner.transform.rotation;
        Vector2 aimOffset = Vector2.zero;

        // Helper: get target transform for NPC/Enemy if present
        Transform aiTarget = null;
        if (owner is EnemyStateMachine eOwner) aiTarget = eOwner.Target;
        else if (owner is NPCStateMachine nOwner) aiTarget = nOwner.Target;

        if (skillData.TargetingMode == ActiveSkillData.Targeting.Directional)
        {
            // Directional: players use their Aimer; AI aim at target
            if (owner is PlayerStateMachine pOwner && pOwner.Aimer != null)
            {
                aimDirection = pOwner.Aimer.right;
                aimRotation = pOwner.Aimer.rotation;
                spawnPos = owner.transform.position;
                aimOffset = aimDirection.normalized * skillData.SkillRange;
            }
            else if (aiTarget != null)
            {
                Vector2 dir = (Vector2)aiTarget.position - (Vector2)owner.transform.position;
                if (dir.sqrMagnitude > 0.0001f)
                {
                    aimDirection = dir.normalized;
                    float ang = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
                    aimRotation = Quaternion.Euler(0, 0, ang);
                }
                spawnPos = owner.transform.position;
                aimOffset = aimDirection.normalized * skillData.SkillRange;
            }
            else
            {
                // fallback to owner's facing
                aimDirection = owner.transform.right;
                aimRotation = owner.transform.rotation;
                spawnPos = owner.transform.position;
                aimOffset = aimDirection.normalized * skillData.SkillRange;
            }
        }
        else // Ground targeting
        {
            // Player sets GroundTargetPosition before StartSkill (via Indicator). If present, use that.
            if (GroundTargetPosition.HasValue)
            {
                Vector2 target = GroundTargetPosition.Value;
                spawnPos = target;
                Vector2 dir = (target - (Vector2)owner.transform.position);
                if (dir.sqrMagnitude > 0.0001f)
                {
                    aimDirection = dir.normalized;
                    float ang = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
                    aimRotation = Quaternion.Euler(0, 0, ang);
                }
                aimOffset = Vector2.zero;
            }
            else if (aiTarget != null)
            {
                // For enemies/NPCs, spawn on floor where their target stands if within range.
                Vector2 targetPos = aiTarget.position;
                float dist = Vector2.Distance(owner.transform.position, targetPos);
                if (dist <= skillData.SkillRange)
                {
                    spawnPos = targetPos;
                    Vector2 dir = (targetPos - (Vector2)owner.transform.position);
                    if (dir.sqrMagnitude > 0.0001f)
                    {
                        aimDirection = dir.normalized;
                        float ang = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
                        aimRotation = Quaternion.Euler(0, 0, ang);
                    }
                    aimOffset = Vector2.zero;
                }
                else
                {
                    // target out of range -> fallback to directional from owner
                    Vector2 dir = (targetPos - (Vector2)owner.transform.position);
                    if (dir.sqrMagnitude > 0.0001f)
                    {
                        aimDirection = dir.normalized;
                        float ang = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
                        aimRotation = Quaternion.Euler(0, 0, ang);
                    }
                    spawnPos = owner.transform.position;
                    aimOffset = aimDirection.normalized * skillData.SkillRange;
                }
            }
            else
            {
                // No ground position provided and no AI target -> fallback to directional
                if (owner is PlayerStateMachine pOwner && pOwner.Aimer != null)
                {
                    aimDirection = pOwner.Aimer.right;
                    aimRotation = pOwner.Aimer.rotation;
                }
                else
                {
                    aimDirection = owner.transform.right;
                    aimRotation = owner.transform.rotation;
                }
                spawnPos = owner.transform.position;
                aimOffset = aimDirection.normalized * skillData.SkillRange;
            }
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
            Attacker = owner.NetworkObject,
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
        owner.RigidBody2D.linearVelocity = Vector2.zero;

        // Handle Animations
        Vector2 snappedDirection = owner.Animator.SnapDirection(context.AimDirection);
        owner.Animator.SetDirection(snappedDirection);

        // Animate Player Head
        if (owner is PlayerStateMachine player)
        {
            player.customization.net_FacingDirection.Value = snappedDirection;
            player.playerHead.SetHead(snappedDirection);
        }

        // Animate NPC Head
        if (owner is NPCStateMachine npc)
        {
            npc.npc.net_FacingDirection.Value = snappedDirection;
            npc.npc.npcHead.SetHead(snappedDirection);
        }

        // Start Cool Down
        owner.StartCoroutine(CoolDownn(skillData.skillType, skillData.CoolDown, owner));

        // Change State
        ChangeState(ActiveSkillData.SkillPhase.Cast, ModifiedCastTime);
        CastState(owner);
    }

    public virtual void UpdateSkill(StateMachine owner)
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
    public virtual void FixedUpdateSkill(StateMachine owner)
    {

    }

    public virtual void CastState(StateMachine owner)
    {
        if (owner is PlayerStateMachine player)
        {
            owner.Animator.PlayAttackAnimation(skillData.weaponType, ActiveSkillData.SkillType.Basic, ActiveSkillData.SkillPhase.Cast, player.customization.net_ChestIndex.Value, player.customization.net_LegsIndex.Value);
        }

        if (owner is NPCStateMachine npc)
        {
            owner.Animator.PlayAttackAnimation(skillData.weaponType, ActiveSkillData.SkillType.Basic, ActiveSkillData.SkillPhase.Cast, npc.npc.Data.ChestIndex, npc.npc.Data.LegsIndex);
        }

        if (owner is EnemyStateMachine enemy)
        {
            owner.Animator.PlayEnemyAttackAnimation(skillData.skillType, ActiveSkillData.SkillPhase.Cast);
        }

        owner.CastBar.StartCast(ModifiedCastTime);
        RunEffects(skillData.OnCastEffects, owner, ActiveSkillData.SkillPhase.Cast);
    }
    public virtual void ActionState(StateMachine owner)
    {
        if (skillData.ActionTime <= 0) return;

        if (owner is PlayerStateMachine player)
        {
            owner.Animator.PlayAttackAnimation(skillData.weaponType, ActiveSkillData.SkillType.Basic, ActiveSkillData.SkillPhase.Action, player.customization.net_ChestIndex.Value, player.customization.net_LegsIndex.Value);
        }

        if (owner is NPCStateMachine npc)
        {
            owner.Animator.PlayAttackAnimation(skillData.weaponType, ActiveSkillData.SkillType.Basic, ActiveSkillData.SkillPhase.Action, npc.npc.Data.ChestIndex, npc.npc.Data.LegsIndex);
        }

        if (owner is EnemyStateMachine enemy)
        {
            owner.Animator.PlayEnemyAttackAnimation(skillData.skillType, ActiveSkillData.SkillPhase.Action);
        }

        RunEffects(skillData.OnActionEffects, owner, ActiveSkillData.SkillPhase.Action);
    }
    public virtual void ImpactState(StateMachine owner, bool fireEffects = true)
    {
        if (owner is PlayerStateMachine player)
        {
            owner.Animator.PlayAttackAnimation(skillData.weaponType, ActiveSkillData.SkillType.Basic, ActiveSkillData.SkillPhase.Impact, player.customization.net_ChestIndex.Value, player.customization.net_LegsIndex.Value);
        }

        if (owner is NPCStateMachine npc)
        {
            owner.Animator.PlayAttackAnimation(skillData.weaponType, ActiveSkillData.SkillType.Basic, ActiveSkillData.SkillPhase.Impact, npc.npc.Data.ChestIndex, npc.npc.Data.LegsIndex);
        }

        if (owner is EnemyStateMachine enemy)
        {
            owner.Animator.PlayEnemyAttackAnimation(skillData.skillType, ActiveSkillData.SkillPhase.Impact);
        }

        if (fireEffects) RunEffects(skillData.OnImpactEffects, owner, ActiveSkillData.SkillPhase.Impact);
    }
    public virtual void RecoveryState(StateMachine owner, bool fireEffects = true)
    {
        if (owner is PlayerStateMachine player)
        {
            owner.Animator.PlayAttackAnimation(skillData.weaponType, ActiveSkillData.SkillType.Basic, ActiveSkillData.SkillPhase.Recovery, player.customization.net_ChestIndex.Value, player.customization.net_LegsIndex.Value);
        }

        if (owner is NPCStateMachine npc)
        {
            owner.Animator.PlayAttackAnimation(skillData.weaponType, ActiveSkillData.SkillType.Basic, ActiveSkillData.SkillPhase.Recovery, npc.npc.Data.ChestIndex, npc.npc.Data.LegsIndex);
        }

        if (owner is EnemyStateMachine enemy)
        {
            owner.Animator.PlayEnemyAttackAnimation(skillData.skillType, ActiveSkillData.SkillPhase.Recovery);
        }

        if (fireEffects)
        {
            owner.CastBar.StartRecovery(ModifiedRecoveryTime);
            RunEffects(skillData.OnRecoveryEffects, owner, ActiveSkillData.SkillPhase.Recovery);
        }
    }
    public void DoneState(bool isStaggered, StateMachine owner)
    {
        currentState = ActiveSkillData.SkillPhase.Done;

        // Clear owner.CurrentSkill and IsAttacking in an owner-specific manner
        if (owner is PlayerStateMachine player)
        {
            player.IsAttacking = false;
            player.CurrentSkill = null;

            if (isStaggered)
            {
                player.SetState(new PlayerStaggerState(player));
            }
            else
            {
                if (player.IsFullySpawned) player.SetState(new PlayerIdleState(player));
            }
        }
        else if (owner is EnemyStateMachine enemy)
        {
            enemy.IsAttacking = false;
            enemy.CurrentSkill = null;

            if (isStaggered)
            {
                enemy.SetState(new EnemyStaggerState(enemy));
                return;
            }

            if (enemy.Target == null)
            {
                enemy.enemy.PatienceBar.Patience.Value = 0;
                enemy.IsPlayerInRange = false;
                enemy.Target = null;
                enemy.SetState(new EnemyResetState(enemy));
            }
            else
            {
                enemy.SetState(new EnemyIdleState(enemy));
            }
        }
        else if (owner is NPCStateMachine npc)
        {
            npc.IsAttacking = false;
            npc.CurrentSkill = null;

            if (isStaggered)
            {
                npc.SetState(new NPCStaggerState(npc));
            }
            else
            {
                npc.TransitionToIdle();
            }
        }
    }

    protected void StateTransition(StateMachine owner, bool hasAction = false)
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

    IEnumerator CoolDownn(ActiveSkillData.SkillType type, float coolDown, StateMachine owner)
    {
        float modifiedCooldown = coolDown / owner.Stats.TotalCDR;

        // Player-specific UI update
        if (owner is PlayerStateMachine p)
        {
            foreach (SkillBarUI bar in p.coolDownTracker)
            {
                if (bar == null) continue;
                if (!bar.gameObject.activeInHierarchy) continue;
                bar.SkillCoolDown(skillData.skillType, modifiedCooldown);
            }
        }

        yield return new WaitForSeconds(modifiedCooldown);

        // Set cooldown flags on the concrete owner
        if (owner is PlayerStateMachine playerOwner)
        {
            switch (type)
            {
                case ActiveSkillData.SkillType.Basic: playerOwner.CanBasic = true; break;
                case ActiveSkillData.SkillType.Offensive: playerOwner.CanOffensive = true; break;
                case ActiveSkillData.SkillType.Mobility: playerOwner.CanMobility = true; break;
                case ActiveSkillData.SkillType.Defensive: playerOwner.CanDefensive = true; break;
                case ActiveSkillData.SkillType.Utility: playerOwner.CanUtility = true; break;
                case ActiveSkillData.SkillType.Ultimate: playerOwner.CanUltimate = true; break;
            }
        }
        else if (owner is EnemyStateMachine enemyOwner)
        {
            switch (type)
            {
                case ActiveSkillData.SkillType.Basic: enemyOwner.CanBasic = true; break;
                case ActiveSkillData.SkillType.Mobility: enemyOwner.CanSpecial = true; break;
                case ActiveSkillData.SkillType.Ultimate: enemyOwner.CanUltimate = true; break;
            }
        }
        else if (owner is NPCStateMachine npcOwner)
        {
            switch (type)
            {
                case ActiveSkillData.SkillType.Basic: npcOwner.CanBasic = true; break;
                case ActiveSkillData.SkillType.Mobility: npcOwner.CanMobility = true; break;
                case ActiveSkillData.SkillType.Ultimate: npcOwner.CanUltimate = true; break;
            }
        }
    }

    void RunEffects(SkillEffect[] effects, StateMachine owner, ActiveSkillData.SkillPhase phase)
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
