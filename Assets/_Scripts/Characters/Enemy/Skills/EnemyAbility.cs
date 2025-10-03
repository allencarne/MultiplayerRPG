using System.Collections;
using Unity.Netcode;
using UnityEngine;

public abstract class EnemyAbility : NetworkBehaviour
{
    [Header("Attack")]
    public GameObject AttackPrefab_;
    public GameObject TelegraphPrefab_;
    public int AbilityDamage_;
    public float AttackRange_;

    [Header("Time")]
    public float CastTime;
    public float ImpactTime;
    public float RecoveryTime;

    [Header("CoolDown")]
    public float CoolDown;

    [Header("Action")]
    public float ActionTime;

    [Header("Knockback")]
    [SerializeField] protected float KnockBackAmount_;
    [SerializeField] protected float KnockBackDuration_;

    [HideInInspector] protected float stateTimer;
    [HideInInspector] protected float ModifiedCastTime;

    [HideInInspector] protected Vector2 SpawnPosition;
    [HideInInspector] protected Vector2 AimDirection;
    [HideInInspector] protected Quaternion AimRotation;
    [HideInInspector] protected Vector2 AimOffset;

    public enum State { Cast, Action, Impact, Recovery, Done }
    public State currentState;

    public enum SkillType { Basic, Special, Ultimate }
    public SkillType skillType;

    public abstract void AbilityStart(EnemyStateMachine owner);
    public abstract void AbilityUpdate(EnemyStateMachine owner);
    public abstract void AbilityFixedUpdate(EnemyStateMachine owner);


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
    public void DoneState(bool isStaggered, EnemyStateMachine owner)
    {
        StartCoroutine(CoolDownn(skillType, CoolDown, owner));
        currentState = State.Done;
        owner.IsAttacking = false;
        owner.currentAbility = null;

        if (isStaggered)
        {
            owner.SetState(EnemyStateMachine.State.Hurt);
        }
        else
        {
            owner.SetState(EnemyStateMachine.State.Idle);
        }

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
    protected void InitializeAbility(SkillType skilltype, EnemyStateMachine owner)
    {
        switch (skilltype)
        {
            case SkillType.Basic: owner.CanBasic = false; break;
            case SkillType.Special: owner.CanSpecial = false; break;
            case SkillType.Ultimate: owner.CanUltimate = false; break;
        }
        owner.IsAttacking = true;
        owner.currentAbility = this;
    }
    protected void ChangeState(State next, float duration)
    {
        currentState = next;
        stateTimer = duration;
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


    protected void Telegraph(bool useOffset, bool useRotation)
    {
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

        DamageOnTrigger damageOnTrigger = attackInstance.GetComponent<DamageOnTrigger>();
        if (damageOnTrigger != null)
        {
            damageOnTrigger.attacker = attacker;
            damageOnTrigger.AbilityDamage = AbilityDamage_;
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

        DestroyOnDeath death = attackInstance.GetComponent<DestroyOnDeath>();
        if (death != null) death.enemy = GetComponentInParent<Enemy>();

        DespawnDelay despawnDelay = attackInstance.GetComponent<DespawnDelay>();
        if (despawnDelay != null) despawnDelay.StartCoroutine(despawnDelay.DespawnAfterDuration(ActionTime));
    }




    public virtual void Impact(EnemyStateMachine owner)
    {
        // Remove Later
    }
}
