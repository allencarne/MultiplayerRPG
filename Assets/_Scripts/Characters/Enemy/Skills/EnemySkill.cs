using System.Collections;
using Unity.Netcode;
using UnityEngine;

public abstract class EnemySkill : NetworkBehaviour
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

    public virtual void StartSkill(EnemyStateMachine owner)
    {

    }
    public virtual void UpdateSkill(EnemyStateMachine owner)
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
    public virtual void FixedUpdateSkill(EnemyStateMachine owner)
    {

    }

    public virtual void CastState(EnemyStateMachine owner)
    {

    }
    public virtual void ActionState(EnemyStateMachine owner)
    {

    }
    public virtual void ImpactState(EnemyStateMachine owner)
    {

    }
    public virtual void RecoveryState(EnemyStateMachine owner)
    {

    }

    protected void StateTransition(EnemyStateMachine owner, bool hasAction = false)
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
    public void DoneState(bool isStaggered, EnemyStateMachine owner)
    {
        currentState = State.Done;
        owner.IsAttacking = false;
        owner.CurrentSkill = null;

        if (isStaggered)
        {
            owner.SetState(EnemyStateMachine.State.Stagger);
            return;
        }

        if (owner.Target == null)
        {
            owner.enemy.PatienceBar.Patience.Value = 0;
            owner.IsPlayerInRange = false;
            owner.Target = null;
            owner.SetState(EnemyStateMachine.State.Reset);
        }
        else
        {
            owner.SetState(EnemyStateMachine.State.Idle);
        }
    }

    protected void InitializeAbility(SkillData.SkillType skilltype, EnemyStateMachine owner)
    {
        owner.CurrentSkill = this;

        if (skilltype == SkillData.SkillType.Basic)
        {
            //IsBasic = true;
            ModifiedCastTime = skillData.CastTime / owner.enemy.stats.TotalAS;
            ModifiedRecoveryTime = skillData.RecoveryTime / owner.enemy.stats.TotalAS;
        }

        owner.EnemyRB.linearVelocity = Vector2.zero;
        SpawnPosition = owner.transform.position;

        StartCoroutine(CoolDownn(skillData.skillType, skillData.CoolDown, owner));
    }
    IEnumerator CoolDownn(SkillData.SkillType type, float coolDown, EnemyStateMachine owner)
    {
        float modifiedCooldown = coolDown / owner.enemy.stats.TotalCDR;

        yield return new WaitForSeconds(modifiedCooldown);

        switch (type)
        {
            case SkillData.SkillType.Basic: owner.CanBasic = true; break;
            case SkillData.SkillType.Mobility: owner.CanSpecial = true; break;
            case SkillData.SkillType.Ultimate: owner.CanUltimate = true; break;
        }
    }
    protected void Animate(EnemyStateMachine owner, SkillData.SkillType type, State state)
    {
        string animationType = "";
        string animationState = "";

        switch (type)
        {
            case SkillData.SkillType.Basic: animationType = "Basic"; break;
            case SkillData.SkillType.Mobility: animationType = "Special"; break;
            case SkillData.SkillType.Ultimate: animationType = "Ultimate"; break;
        }

        switch (state)
        {
            case State.Cast: animationState = "Cast"; break;
            case State.Action: animationState = "Action"; break;
            case State.Impact: animationState = "Impact"; break;
            case State.Recovery: animationState = "Recovery"; break;
            case State.Done: animationState = "Done"; break;
        }

        owner.EnemyAnimator.Play(animationType + " " + animationState);
    }

    protected void Telegraph(float time, bool useOffset, bool useRotation)
    {
        if (skillData.TelegraphPrefab == null) return;

        Vector2 position = useOffset ? SpawnPosition + AimOffset : SpawnPosition;
        Quaternion rotation = useRotation ? AimRotation : Quaternion.identity;

        GameObject attackInstance = Instantiate(skillData.TelegraphPrefab, position, rotation);
        attackInstance.transform.localScale *= transform.lossyScale.x;
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
    protected void Attack(NetworkObject attacker, bool useOffset, bool useRotation)
    {
        Vector2 position = useOffset ? SpawnPosition + AimOffset : SpawnPosition;
        Quaternion rotation = useRotation ? AimRotation : Quaternion.identity;

        GameObject attackInstance = Instantiate(skillData.SkillPrefab, position, rotation);
        attackInstance.transform.localScale *= transform.lossyScale.x;
        NetworkObject attackNetObj = attackInstance.GetComponent<NetworkObject>();
        attackNetObj.Spawn();

        Enemy enemy = attacker.GetComponent<Enemy>();

        Rigidbody2D attackRB = attackInstance.GetComponent<Rigidbody2D>();
        if (attackRB != null)
        {
            attackRB.AddForce(AimDirection * skillData.SkillForce, ForceMode2D.Impulse);
        }

        DamageOnTrigger damageOnTrigger = attackInstance.GetComponent<DamageOnTrigger>();
        if (damageOnTrigger != null)
        {
            damageOnTrigger.attacker = attacker;
            damageOnTrigger.AbilityDamage = enemy.stats.TotalDamage + skillData.SkillDamage;
            damageOnTrigger.IgnoreEnemy = true;
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
            knockbackOnTrigger.attacker = attacker;
            knockbackOnTrigger.Amount = skillData.KnockBackForce;
            knockbackOnTrigger.Duration = skillData.KnockBackDuration;
            knockbackOnTrigger.Direction = AimDirection.normalized;
            knockbackOnTrigger.IgnoreEnemy = true;
        }

        StunOnTrigger stunOnTrigger = attackInstance.GetComponent<StunOnTrigger>();
        if (stunOnTrigger != null)
        {
            stunOnTrigger.attacker = attacker;
            stunOnTrigger.Duration = skillData.StunDuration;
            stunOnTrigger.IgnoreEnemy = true;
        }

        SlowOnTrigger slow = attackInstance.GetComponent<SlowOnTrigger>();
        if (slow != null)
        {
            slow.attacker = attacker;
            slow.Duration = skillData.SlowDuration;
            slow.Stacks = skillData.SlowStacks;
            slow.IgnoreEnemy = true;
        }

        FollowTarget target = attackInstance.GetComponent<FollowTarget>();
        if (target != null) target.Target = transform;

        DestroyOnDeath death = attackInstance.GetComponent<DestroyOnDeath>();
        if (death != null) death.stats = GetComponentInParent<CharacterStats>();

        DespawnDelay despawnDelay = attackInstance.GetComponent<DespawnDelay>();
        if (despawnDelay != null) despawnDelay.StartCoroutine(despawnDelay.DespawnAfterDuration(skillData.SkillDuration));
    }
}
