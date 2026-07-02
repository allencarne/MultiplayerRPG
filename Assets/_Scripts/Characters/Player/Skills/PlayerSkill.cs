using System.Collections;
using Unity.Netcode;
using UnityEngine;

public abstract class PlayerSkill : NetworkBehaviour
{
    public SkillData skillData;

    public enum State { Cast, Action, Impact, Recovery, Done }
    [HideInInspector] public State currentState;

    [Header("StateTimer")]
    [HideInInspector] protected float StateTimer;
    [HideInInspector] protected float ModifiedCastTime;
    [HideInInspector] protected float ModifiedRecoveryTime;

    [Header("Aim")]
    [HideInInspector] protected Vector2 SpawnPosition;
    [HideInInspector] protected Vector2 AimDirection;
    [HideInInspector] protected Vector2 AimOffset;
    [HideInInspector] protected Quaternion AimRotation;
    [HideInInspector] protected float AttackerDamage;
    [HideInInspector] protected bool IsBasic = false;

    public virtual void StartSkill(PlayerStateMachine owner)
    {

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

    }
    public virtual void ActionState(PlayerStateMachine owner)
    {

    }
    public virtual void ImpactState(PlayerStateMachine owner)
    {

    }
    public virtual void RecoveryState(PlayerStateMachine owner)
    {

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
    public void DoneState(bool isStaggered, PlayerStateMachine owner)
    {
        currentState = State.Done;
        owner.IsAttacking = false;
        owner.CurrentSkill = null;

        if (isStaggered)
        {
            owner.SetState(PlayerStateMachine.State.Stagger);

        }
        else
        {
            if (owner.IsFullySpawned) owner.SetState(PlayerStateMachine.State.Idle);
        }
    }

    protected void InitializeAbility(SkillData.SkillType skilltype, PlayerStateMachine owner)
    {
        owner.CurrentSkill = this;

        if (skilltype == SkillData.SkillType.Basic)
        {
            IsBasic = true;
            ModifiedCastTime = skillData.CastTime / owner.Stats.TotalAS;
            ModifiedRecoveryTime = skillData.RecoveryTime / owner.Stats.TotalAS;
        }
            
        AttackerDamage = owner.Stats.TotalDamage;

        owner.PlayerRB.linearVelocity = Vector2.zero;
        SpawnPosition = owner.transform.position;

        StartCoroutine(CoolDownn(skillData.skillType, skillData.CoolDown, owner));
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

    protected void Telegraph(float time, bool useOffset, bool useRotation)
    {
        if (skillData.TelegraphPrefab == null) return;

        Vector2 position = useOffset ? SpawnPosition + AimOffset : SpawnPosition;
        Quaternion rotation = useRotation ? AimRotation : Quaternion.identity;

        GameObject attackInstance = Instantiate(skillData.TelegraphPrefab, position, rotation);
        NetworkObject attackNetObj = attackInstance.GetComponent<NetworkObject>();
        attackNetObj.Spawn();

        CircleTelegraph circle = attackInstance.GetComponent<CircleTelegraph>();
        if (circle != null)
        {
            circle.stats = gameObject.GetComponentInParent<CharacterStats>();
            circle.Init();

            circle.FillSpeed = time;
        }

        SquareTelegraph square = attackInstance.GetComponent<SquareTelegraph>();
        if (square != null)
        {
            square.stats = gameObject.GetComponentInParent<CharacterStats>();
            square.Init();

            square.FillSpeed = time;
        }
    }
    protected void Attack(ulong attackerID)
    {
        NetworkObject attacker = NetworkManager.Singleton.ConnectedClients[attackerID].PlayerObject;

        GameObject attackInstance = Instantiate(skillData.SkillPrefab, SpawnPosition + AimOffset, AimRotation);
        NetworkObject attackNetObj = attackInstance.GetComponent<NetworkObject>();
        attackNetObj.Spawn();

        Rigidbody2D attackRB = attackInstance.GetComponent<Rigidbody2D>();
        if (attackRB != null)
        {
            attackRB.AddForce(AimDirection * skillData.SkillForce, ForceMode2D.Impulse);
        }

        DamageOnTrigger damageOnTrigger = attackInstance.GetComponent<DamageOnTrigger>();
        if (damageOnTrigger != null)
        {
            damageOnTrigger.CanGenerateFury = IsBasic;
            damageOnTrigger.attacker = attacker;
            damageOnTrigger.AbilityDamage = AttackerDamage + skillData.SkillDamage;
            damageOnTrigger.IgnorePlayer = true;
            damageOnTrigger.IgnoreNPC = true;

            if (skillData.HealAmount > 0)
            {
                damageOnTrigger.HealAmount = skillData.HealAmount;
                damageOnTrigger.CanHeal = true; 
            }
        }

        InterruptOnTrigger interruptOnTrigger = attackInstance.GetComponent<InterruptOnTrigger>();
        if (interruptOnTrigger != null)
        {
            interruptOnTrigger.attacker = attacker;
            interruptOnTrigger.IgnorePlayer = true;
            interruptOnTrigger.IgnoreNPC = true;
        }

        KnockbackOnTrigger knockbackOnTrigger = attackInstance.GetComponent<KnockbackOnTrigger>();
        if (knockbackOnTrigger != null)
        {
            knockbackOnTrigger.attacker = attacker;
            knockbackOnTrigger.Amount = skillData.KnockBackForce;
            knockbackOnTrigger.Duration = skillData.KnockBackDuration;
            knockbackOnTrigger.Direction = AimDirection.normalized;
            knockbackOnTrigger.IgnorePlayer = true;
            knockbackOnTrigger.IgnoreNPC = true;
        }

        StunOnTrigger stunOnTrigger = attackInstance.GetComponent<StunOnTrigger>();
        if (stunOnTrigger != null)
        {
            stunOnTrigger.attacker = attacker;
            stunOnTrigger.Duration = skillData.StunDuration;
            stunOnTrigger.IgnorePlayer = true;
            stunOnTrigger.IgnoreNPC = true;
        }

        SlowOnTrigger slow = attackInstance.GetComponent<SlowOnTrigger>();
        if (slow != null)
        {
            slow.attacker = attacker;
            slow.Duration = skillData.SlowDuration;
            slow.Stacks = skillData.SlowStacks;
            slow.IgnorePlayer = true;
            slow.IgnoreNPC = true;
        }

        FollowTarget target = attackInstance.GetComponent<FollowTarget>();
        if (target != null) target.Target = transform;

        DestroyOnDeath death = attackInstance.GetComponent<DestroyOnDeath>();
        if (death != null) death.stats = GetComponentInParent<CharacterStats>();

        DespawnDelay despawnDelay = attackInstance.GetComponent<DespawnDelay>();
        if (despawnDelay != null) despawnDelay.StartCoroutine(despawnDelay.DespawnAfterDuration(skillData.SkillDuration));
    }

    [ServerRpc]
    public void AttackServerRpc(bool isBasic, Vector2 spawnPosition, Vector2 aimOffset, Vector2 aimDirection, Quaternion aimRotation, float damage, ulong attackerID)
    {
        IsBasic = isBasic;
        SpawnPosition = spawnPosition;
        AimOffset = aimOffset;
        AimDirection = aimDirection;
        AimRotation = aimRotation;
        AttackerDamage = damage;
        Attack(attackerID);
    }
}
