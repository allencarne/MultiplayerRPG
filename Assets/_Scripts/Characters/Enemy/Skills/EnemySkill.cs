using System.Collections;
using Unity.Netcode;
using UnityEngine;

public abstract class EnemySkill : NetworkBehaviour
{
    public enum State { Cast, Action, Impact, Recovery, Done }
    [HideInInspector] public State currentState;
    public enum SkillType { Basic, Special, Ultimate }
    public SkillType skillType;

    [Header("Prefab")]
    [SerializeField] protected GameObject SkillPrefab;
    [SerializeField] protected GameObject TelegraphPrefab;

    [Header("Stats")]
    [SerializeField] protected float SkillDamage;
    [SerializeField] protected float SkillRange;
    [SerializeField] protected float SkillForce;
    [SerializeField] protected float SkillDuration;

    [Header("Time")]
    [SerializeField] protected float CastTime;
    [SerializeField] protected float ActionTime;
    [SerializeField] protected float ImpactTime;
    [SerializeField] protected float RecoveryTime;

    [Header("CoolDown")]
    [SerializeField] protected float CoolDown;

    [Header("Slide")]
    [SerializeField] protected int SlideForce;
    [SerializeField] protected float SlideDuration;

    [Header("Slow")]
    [SerializeField] protected int SlowStacks;
    [SerializeField] protected float SlowDuration;

    [Header("Knockback")]
    [SerializeField] protected float KnockBackForce;
    [SerializeField] protected float KnockBackDuration;

    [Header("Stun")]
    [SerializeField] protected float StunDuration;

    [Header("StateTimer")]
    [HideInInspector] protected float StateTimer;
    [HideInInspector] protected float ModifiedCastTime;

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
        if (ActionTime > 0)
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
                    ChangeState(State.Action, ActionTime);
                }
                else
                {
                    ImpactState(owner);
                    ChangeState(State.Impact, ImpactTime);
                }
                break;

            case State.Action:
                ImpactState(owner);
                ChangeState(State.Impact, ImpactTime);
                break;

            case State.Impact:
                RecoveryState(owner);
                ChangeState(State.Recovery, RecoveryTime);
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
            owner.SetState(EnemyStateMachine.State.Hurt);

        }
        else
        {
            owner.SetState(EnemyStateMachine.State.Idle);
        }

    }

    protected void InitializeAbility(SkillType skilltype, EnemyStateMachine owner)
    {
        owner.CurrentSkill = this;

        owner.EnemyRB.linearVelocity = Vector2.zero;
        SpawnPosition = owner.transform.position;

        StartCoroutine(CoolDownn(skillType, CoolDown, owner));
    }
    IEnumerator CoolDownn(SkillType type, float coolDown, EnemyStateMachine owner)
    {
        float modifiedCooldown = coolDown / owner.enemy.stats.TotalCDR;

        yield return new WaitForSeconds(modifiedCooldown);

        switch (type)
        {
            case SkillType.Basic: owner.CanBasic = true; break;
            case SkillType.Special: owner.CanSpecial = true; break;
            case SkillType.Ultimate: owner.CanUltimate = true; break;
        }
    }
    protected void Animate(EnemyStateMachine owner, SkillType type, State state)
    {
        string animationType = "";
        string animationState = "";

        switch (type)
        {
            case SkillType.Basic: animationType = "Basic"; break;
            case SkillType.Special: animationType = "Special"; break;
            case SkillType.Ultimate: animationType = "Ultimate"; break;
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
        if (TelegraphPrefab == null) return;

        Vector2 position = useOffset ? SpawnPosition + AimOffset : SpawnPosition;
        Quaternion rotation = useRotation ? AimRotation : Quaternion.identity;

        GameObject attackInstance = Instantiate(TelegraphPrefab, position, rotation);
        attackInstance.transform.localScale *= transform.lossyScale.x;
        NetworkObject attackNetObj = attackInstance.GetComponent<NetworkObject>();
        attackNetObj.Spawn();

        CircleTelegraph circle = attackInstance.GetComponent<CircleTelegraph>();
        if (circle != null)
        {
            circle.stats = gameObject.GetComponentInParent<CharacterStats>();

            circle.FillSpeed = time;
            circle.crowdControl = gameObject.GetComponentInParent<CrowdControl>();
        }

        SquareTelegraph square = attackInstance.GetComponent<SquareTelegraph>();
        if (square != null)
        {
            square.stats = gameObject.GetComponentInParent<CharacterStats>();

            square.FillSpeed = time;
            square.crowdControl = gameObject.GetComponentInParent<CrowdControl>();
        }
    }
    protected void Attack(NetworkObject attacker, bool useOffset, bool useRotation)
    {
        Vector2 position = useOffset ? SpawnPosition + AimOffset : SpawnPosition;
        Quaternion rotation = useRotation ? AimRotation : Quaternion.identity;

        GameObject attackInstance = Instantiate(SkillPrefab, position, rotation);
        attackInstance.transform.localScale *= transform.lossyScale.x;
        NetworkObject attackNetObj = attackInstance.GetComponent<NetworkObject>();
        attackNetObj.Spawn();

        Enemy enemy = attacker.GetComponent<Enemy>();

        Rigidbody2D attackRB = attackInstance.GetComponent<Rigidbody2D>();
        if (attackRB != null)
        {
            attackRB.AddForce(AimDirection * SkillForce, ForceMode2D.Impulse);
        }

        DamageOnTrigger damageOnTrigger = attackInstance.GetComponent<DamageOnTrigger>();
        if (damageOnTrigger != null)
        {
            damageOnTrigger.attacker = attacker;
            damageOnTrigger.AbilityDamage = enemy.stats.TotalDamage + SkillDamage;
            damageOnTrigger.IgnoreEnemy = true;
        }

        KnockbackOnTrigger knockbackOnTrigger = attackInstance.GetComponent<KnockbackOnTrigger>();
        if (knockbackOnTrigger != null)
        {
            knockbackOnTrigger.attacker = attacker;
            knockbackOnTrigger.Amount = KnockBackForce;
            knockbackOnTrigger.Duration = KnockBackDuration;
            knockbackOnTrigger.Direction = AimDirection.normalized;
            knockbackOnTrigger.IgnoreEnemy = true;
        }

        StunOnTrigger stunOnTrigger = attackInstance.GetComponent<StunOnTrigger>();
        if (stunOnTrigger != null)
        {
            stunOnTrigger.attacker = attacker;
            stunOnTrigger.Duration = StunDuration;
            stunOnTrigger.IgnoreNPC = true;
        }

        SlowOnTrigger slow = attackInstance.GetComponent<SlowOnTrigger>();
        if (slow != null)
        {
            slow.attacker = attacker;
            slow.Duration = SlowDuration;
            slow.Stacks = SlowStacks;
            slow.IgnoreEnemy = true;
        }

        FollowTarget target = attackInstance.GetComponent<FollowTarget>();
        if (target != null) target.Target = transform;

        DestroyOnDeath death = attackInstance.GetComponent<DestroyOnDeath>();
        if (death != null) death.stats = GetComponentInParent<CharacterStats>();

        DespawnDelay despawnDelay = attackInstance.GetComponent<DespawnDelay>();
        if (despawnDelay != null) despawnDelay.StartCoroutine(despawnDelay.DespawnAfterDuration(SkillDuration));
    }
}
