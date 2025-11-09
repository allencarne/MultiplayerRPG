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
    [SerializeField] protected GameObject AttackPrefab_;
    [SerializeField] protected GameObject TelegraphPrefab_;

    [Header("Stats")]
    [SerializeField] protected int AbilityDamage_;
    [SerializeField] protected float AttackRange_;
    [SerializeField] protected float ProjectileForce_;
    [SerializeField] protected float ProjectileDuration_;

    [Header("Time")]
    [SerializeField] protected float CastTime;
    [SerializeField] protected float ActionTime;
    [SerializeField] protected float ImpactTime;
    [SerializeField] protected float RecoveryTime;

    [Header("CoolDown")]
    [SerializeField] protected float CoolDown;

    [Header("Knockback")]
    [SerializeField] protected float KnockBackAmount_;
    [SerializeField] protected float KnockBackDuration_;

    [Header("Slow")]
    [SerializeField] protected int SlowStacks_;
    [SerializeField] protected float SlowDuration_;

    [Header("StateTimer")]
    [HideInInspector] protected float stateTimer;
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

        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0f) StateTransition(owner);
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
        stateTimer = duration;
    }
    public void DoneState(bool isStaggered, EnemyStateMachine owner)
    {
        StartCoroutine(CoolDownn(skillType, CoolDown, owner));
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
        switch (skilltype)
        {
            case SkillType.Basic: owner.CanBasic = false; break;
            case SkillType.Special: owner.CanSpecial = false; break;
            case SkillType.Ultimate: owner.CanUltimate = false; break;
        }
        owner.IsAttacking = true;
        owner.CurrentSkill = this;

        owner.EnemyRB.linearVelocity = Vector2.zero;
        SpawnPosition = owner.transform.position;
    }
    IEnumerator CoolDownn(SkillType type, float coolDown, EnemyStateMachine owner)
    {
        float modifiedCooldown = coolDown / owner.enemy.CurrentCDR;

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

    protected void Telegraph(bool useOffset, bool useRotation)
    {
        if (TelegraphPrefab_ == null) return;

        Vector2 position = useOffset ? SpawnPosition + AimOffset : SpawnPosition;
        Quaternion rotation = useRotation ? AimRotation : Quaternion.identity;

        GameObject attackInstance = Instantiate(TelegraphPrefab_, position, rotation);
        NetworkObject attackNetObj = attackInstance.GetComponent<NetworkObject>();
        attackNetObj.Spawn();

        CircleTelegraph circle = attackInstance.GetComponent<CircleTelegraph>();
        if (circle != null)
        {
            circle.FillSpeed = ModifiedCastTime + ActionTime;
            circle.crowdControl = gameObject.GetComponentInParent<CrowdControl>();
            circle.enemy = gameObject.GetComponentInParent<Enemy>();
        }

        SquareTelegraph square = attackInstance.GetComponent<SquareTelegraph>();
        if (square != null)
        {
            square.FillSpeed = ModifiedCastTime + ActionTime;
            square.crowdControl = gameObject.GetComponentInParent<CrowdControl>();
            square.enemy = gameObject.GetComponentInParent<Enemy>();
        }
    }
    protected void Attack(NetworkObject attacker, bool useOffset, bool useRotation)
    {
        Vector2 position = useOffset ? SpawnPosition + AimOffset : SpawnPosition;
        Quaternion rotation = useRotation ? AimRotation : Quaternion.identity;

        GameObject attackInstance = Instantiate(AttackPrefab_, position, rotation);
        NetworkObject attackNetObj = attackInstance.GetComponent<NetworkObject>();
        attackNetObj.Spawn();

        Enemy enemy = attacker.GetComponent<Enemy>();

        Rigidbody2D attackRB = attackInstance.GetComponent<Rigidbody2D>();
        if (attackRB != null)
        {
            attackRB.AddForce(AimDirection * ProjectileForce_, ForceMode2D.Impulse);
        }

        DamageOnTrigger damageOnTrigger = attackInstance.GetComponent<DamageOnTrigger>();
        if (damageOnTrigger != null)
        {
            damageOnTrigger.attacker = attacker;
            damageOnTrigger.AbilityDamage = enemy.CurrentDamage + AbilityDamage_;
            damageOnTrigger.IgnoreEnemy = true;
        }

        KnockbackOnTrigger knockbackOnTrigger = attackInstance.GetComponent<KnockbackOnTrigger>();
        if (knockbackOnTrigger != null)
        {
            knockbackOnTrigger.attacker = attacker;
            knockbackOnTrigger.Amount = KnockBackAmount_;
            knockbackOnTrigger.Duration = KnockBackDuration_;
            knockbackOnTrigger.Direction = AimDirection.normalized;
            knockbackOnTrigger.IgnoreEnemy = true;
        }

        SlowOnTrigger slow = attackInstance.GetComponent<SlowOnTrigger>();
        if (slow != null)
        {
            slow.attacker = attacker;
            slow.Duration = SlowDuration_;
            slow.Stacks = SlowStacks_;
            slow.IgnoreEnemy = true;
        }

        DestroyOnDeath death = attackInstance.GetComponent<DestroyOnDeath>();
        if (death != null) death.enemy = GetComponentInParent<Enemy>();

        DespawnDelay despawnDelay = attackInstance.GetComponent<DespawnDelay>();
        if (despawnDelay != null) despawnDelay.StartCoroutine(despawnDelay.DespawnAfterDuration(ProjectileDuration_));
    }
}
