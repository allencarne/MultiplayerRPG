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

    protected void DoneState(EnemyStateMachine owner)
    {
        StartCoroutine(owner.CoolDown(skillType, CoolDown));
        currentState = State.Done;
        owner.IsAttacking = false;
        owner.currentAbility = null;
        owner.SetState(EnemyStateMachine.State.Idle);
    }

    public abstract void AbilityStart(EnemyStateMachine owner);
    public abstract void AbilityUpdate(EnemyStateMachine owner);
    public abstract void AbilityFixedUpdate(EnemyStateMachine owner);
    public virtual void Impact(EnemyStateMachine owner)
    {

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
}
