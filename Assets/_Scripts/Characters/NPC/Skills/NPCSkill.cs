using System.Collections;
using Unity.Netcode;
using UnityEngine;

public abstract class NPCSkill : NetworkBehaviour
{
    public ActiveSkillData skillData;
    [HideInInspector] public ActiveSkillData.SkillPhase currentState;

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

    public virtual void StartSkill(NPCStateMachine owner)
    {

    }
    public virtual void UpdateSkill(NPCStateMachine owner)
    {
        if (currentState == ActiveSkillData.SkillPhase.Done) return;

        StateTimer -= Time.deltaTime;
        if (StateTimer <= 0f) StateTransition(owner);
    }
    public virtual void FixedUpdateSkill(NPCStateMachine owner)
    {

    }

    public virtual void CastState(NPCStateMachine owner)
    {

    }
    public virtual void ActionState(NPCStateMachine owner)
    {

    }
    public virtual void ImpactState(NPCStateMachine owner)
    {

    }
    public virtual void RecoveryState(NPCStateMachine owner)
    {

    }

    protected void StateTransition(NPCStateMachine owner, bool hasAction = false)
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
                    ChangeState(ActiveSkillData.SkillPhase.Impact, skillData.ImpactTime);
                }
                break;

            case ActiveSkillData.SkillPhase.Action:
                ImpactState(owner);
                ChangeState(ActiveSkillData.SkillPhase.Impact, skillData.ImpactTime);
                break;

            case ActiveSkillData.SkillPhase.Impact:
                RecoveryState(owner);
                if (skillData.skillType == ActiveSkillData.SkillType.Basic)
                {
                    ChangeState(ActiveSkillData.SkillPhase.Recovery, ModifiedRecoveryTime);
                }
                else
                {
                    ChangeState(ActiveSkillData.SkillPhase.Recovery, skillData.RecoveryTime);
                }
                break;

            case ActiveSkillData.SkillPhase.Recovery:
                DoneState(false, owner);
                break;
        }
    }
    protected void ChangeState(ActiveSkillData.SkillPhase next, float duration)
    {
        currentState = next;
        StateTimer = duration;
    }
    public void DoneState(bool isStaggered, NPCStateMachine owner)
    {
        currentState = ActiveSkillData.SkillPhase.Done;
        owner.IsAttacking = false;
        owner.CurrentSkill = null;

        if (isStaggered)
        {
            owner.SetState(new NPCStaggerState(owner));

        }
        else
        {
            owner.TransitionToIdle();
        }

    }

    protected void InitializeAbility(ActiveSkillData.SkillType skilltype, NPCStateMachine owner)
    {
        owner.CurrentSkill = this;

        if (skilltype == ActiveSkillData.SkillType.Basic)
        {
            ModifiedCastTime = skillData.CastTime / owner.npc.stats.TotalAS;
            ModifiedRecoveryTime = skillData.RecoveryTime / owner.npc.stats.TotalAS;
        }

        AttackerDamage = owner.npc.stats.TotalDamage;

        owner.NpcRB.linearVelocity = Vector2.zero;
        SpawnPosition = owner.transform.position;

        StartCoroutine(CoolDownn(skillData.skillType, skillData.CoolDown, owner));
    }
    IEnumerator CoolDownn(ActiveSkillData.SkillType type, float coolDown, NPCStateMachine owner)
    {
        float modifiedCooldown = coolDown / owner.npc.stats.TotalCDR;

        yield return new WaitForSeconds(modifiedCooldown);

        switch (type)
        {
            case ActiveSkillData.SkillType.Basic: owner.CanBasic = true; break;
            case ActiveSkillData.SkillType.Mobility: owner.CanMobility = true; break;
            case ActiveSkillData.SkillType.Ultimate: owner.CanUltimate = true; break;
        }
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
            CharacterStats stats = gameObject.GetComponentInParent<CharacterStats>();
            circle.Init(stats, time);
        }

        SquareTelegraph square = attackInstance.GetComponent<SquareTelegraph>();
        if (square != null)
        {
            CharacterStats stats = gameObject.GetComponentInParent<CharacterStats>();
            square.Init(stats, time);
        }
    }
    protected void Attack()
    {
        NetworkObject attacker = GetComponentInParent<NetworkObject>();

        GameObject attackInstance = Instantiate(skillData.SkillPrefab, SpawnPosition + AimOffset, AimRotation);
        NetworkObject attackNetObj = attackInstance.GetComponent<NetworkObject>();
        attackNetObj.Spawn();

        Rigidbody2D attackRB = attackInstance.GetComponent<Rigidbody2D>();
        if (attackRB != null)
        {
            //attackRB.AddForce(AimDirection * skillData.SkillForce, ForceMode2D.Impulse);
        }

        DamageOnTrigger damageOnTrigger = attackInstance.GetComponent<DamageOnTrigger>();
        if (damageOnTrigger != null)
        {
            //damageOnTrigger.attacker = attacker;
            //damageOnTrigger.AbilityDamage = AttackerDamage + skillData.SkillDamage;
            //damageOnTrigger.IgnoreNPC = true;
            //damageOnTrigger.IgnorePlayer = true;
        }

        InterruptOnTrigger interruptOnTrigger = attackInstance.GetComponent<InterruptOnTrigger>();
        if (interruptOnTrigger != null)
        {
            interruptOnTrigger.attacker = attacker;
            interruptOnTrigger.IgnoreEnemy = true;
        }

        KnockbackOnTrigger knockbackOnTrigger = attackInstance.GetComponent<KnockbackOnTrigger>();
        if (knockbackOnTrigger != null)
        {
            //knockbackOnTrigger.attacker = attacker;
            //knockbackOnTrigger.Amount = skillData.KnockBackForce;
            //knockbackOnTrigger.Duration = skillData.KnockBackDuration;
            //knockbackOnTrigger.Direction = AimDirection.normalized;
            //knockbackOnTrigger.IgnoreNPC = true;
            //knockbackOnTrigger.IgnorePlayer = true;
        }

        SlowOnTrigger slow = attackInstance.GetComponent<SlowOnTrigger>();
        if (slow != null)
        {
            //slow.attacker = attacker;
            //slow.Duration = skillData.SlowDuration;
            //slow.Stacks = skillData.SlowStacks;
            //slow.IgnoreNPC = true;
            //slow.IgnorePlayer = true;
        }

        DestroyOnDeath death = attackInstance.GetComponent<DestroyOnDeath>();
        if (death != null) death.stats = GetComponentInParent<CharacterStats>();

        DespawnDelay despawnDelay = attackInstance.GetComponent<DespawnDelay>();
        //if (despawnDelay != null) despawnDelay.StartCoroutine(despawnDelay.DespawnAfterDuration(skillData.SkillDuration));
    }

    [ServerRpc]
    public void AttackServerRpc(Vector2 spawnPosition, Vector2 aimOffset, Vector2 aimDirection, Quaternion aimRotation, float damage)
    {
        SpawnPosition = spawnPosition;
        AimOffset = aimOffset;
        AimDirection = aimDirection;
        AimRotation = aimRotation;
        AttackerDamage = damage;
        Attack();
    }
}
