using Unity.Netcode;
using UnityEngine;

public abstract class EnemyAbility : NetworkBehaviour
{
    public GameObject AttackPrefab_;
    public GameObject TelegraphPrefab_;
    public int AbilityDamage_;
    public float AttackRange_;

    public float CastTime;
    public float ActionTime;
    public float ImpactTime;
    public float RecoveryTime;
    public float CoolDown;

    [HideInInspector] protected float stateTimer;
    [HideInInspector] protected float ModifiedCastTime;

    [HideInInspector] protected Vector2 SpawnPosition;
    [HideInInspector] protected Vector2 AimDirection;
    [HideInInspector] protected Quaternion AimRotation;

    public enum State { Cast, Action, Impact, Recovery, Done }
    public State currentState;

    public enum SkillType { Basic, Special, Ultimate }
    public SkillType skillType;

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
}
