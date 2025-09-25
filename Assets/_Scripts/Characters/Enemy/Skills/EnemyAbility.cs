using Unity.Netcode;
using UnityEngine;

public abstract class EnemyAbility : NetworkBehaviour
{
    public float CastTime;
    public float ImpactTime;
    public float RecoveryTime;
    public float CoolDown;
    protected float stateTimer;

    public enum State { Cast, Impact, Recovery, Done }
    public State currentState;

    public enum SkillType { Basic, Special, Ultimate }
    public SkillType skillType;

    public abstract void AbilityStart(EnemyStateMachine owner);
    public abstract void AbilityUpdate(EnemyStateMachine owner);
    public abstract void AbilityFixedUpdate(EnemyStateMachine owner);
    public virtual void Impact(EnemyStateMachine owner)
    {

    }
}
