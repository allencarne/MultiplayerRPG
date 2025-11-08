using System.Collections;
using Unity.Netcode;
using UnityEngine;

public abstract class NPCSkill : NetworkBehaviour
{
    public enum State { Cast, Action, Impact, Recovery, Done }
    [HideInInspector] public State currentState;
    public enum SkillType { Basic, Special, Ultimate }
    public SkillType skillType;

    [Header("Prefab")]
    [SerializeField] protected GameObject SkillPrefab;
    [SerializeField] protected GameObject TelegraphPrefab;

    [Header("Stats")]
    [SerializeField] protected int SkillDamage;
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

    [Header("Knockback")]
    [SerializeField] protected float KnockBackForce;
    [SerializeField] protected float KnockBackDuration;

    [Header("Slow")]
    [SerializeField] protected int SlowStacks;
    [SerializeField] protected float SlowDuration;

    [Header("StateTimer")]
    [HideInInspector] protected float StateTimer;
    [HideInInspector] protected float ModifiedCastTime;

    [Header("Aim")]
    [HideInInspector] protected Vector2 SpawnPosition;
    [HideInInspector] protected Vector2 AimDirection;
    [HideInInspector] protected Vector2 AimOffset;
    [HideInInspector] protected Quaternion AimRotation;

    public virtual void AbilityStart(NPCStateMachine owner)
    {

    }
    public virtual void AbilityUpdate(NPCStateMachine owner)
    {

    }
    public virtual void AbilityFixedUpdate(NPCStateMachine owner)
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
    public void DoneState(bool isStaggered, NPCStateMachine owner)
    {
        StartCoroutine(CoolDownn(skillType, CoolDown, owner));
        currentState = State.Done;
        owner.IsAttacking = false;
        owner.currentSkill = null;

        if (isStaggered)
        {
            owner.SetState(NPCStateMachine.State.Hurt);

        }
        else
        {
            owner.SetState(NPCStateMachine.State.Idle);
        }

    }

    protected void InitializeAbility(SkillType skilltype, NPCStateMachine owner)
    {
        switch (skilltype)
        {
            case SkillType.Basic: owner.CanBasic = false; break;
            case SkillType.Special: owner.CanSpecial = false; break;
            case SkillType.Ultimate: owner.CanUltimate = false; break;
        }
        owner.IsAttacking = true;
        owner.currentSkill = this;
    }
    IEnumerator CoolDownn(SkillType type, float coolDown, NPCStateMachine owner)
    {
        float modifiedCooldown = coolDown / owner.npc.CurrentCDR;

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
        Vector2 position = useOffset ? SpawnPosition + AimOffset : SpawnPosition;
        Quaternion rotation = useRotation ? AimRotation : Quaternion.identity;

        GameObject attackInstance = Instantiate(TelegraphPrefab, position, rotation);
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

        GameObject attackInstance = Instantiate(SkillPrefab, position, rotation);
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
            damageOnTrigger.AbilityDamage = enemy.CurrentDamage + SkillDamage;
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

        SlowOnTrigger slow = attackInstance.GetComponent<SlowOnTrigger>();
        if (slow != null)
        {
            slow.attacker = attacker;
            slow.Duration = SlowDuration;
            slow.Stacks = SlowStacks;
            slow.IgnoreEnemy = true;
        }

        DestroyOnDeath death = attackInstance.GetComponent<DestroyOnDeath>();
        if (death != null) death.enemy = GetComponentInParent<Enemy>();

        DespawnDelay despawnDelay = attackInstance.GetComponent<DespawnDelay>();
        if (despawnDelay != null) despawnDelay.StartCoroutine(despawnDelay.DespawnAfterDuration(SkillDuration));
    }
}
