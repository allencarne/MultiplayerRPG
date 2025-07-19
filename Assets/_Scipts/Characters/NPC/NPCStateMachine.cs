using UnityEngine;

public class NPCStateMachine : MonoBehaviour
{
    [Header("States")]
    [SerializeField] NPCState spawnState;
    [SerializeField] NPCState idleState;
    [SerializeField] NPCState interactState;
    [SerializeField] NPCState wanderState;
    [SerializeField] NPCState chaseState;
    [SerializeField] NPCState resetState;
    [SerializeField] NPCState hurtState;
    [SerializeField] NPCState deathState;
    [SerializeField] NPCState basicState;
    [SerializeField] NPCState specialState;
    [SerializeField] NPCState ultimateState;

    [Header("Private Components")]
    public NPC npc;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Collider2D Collider;
    public Animator BodyAnimator;
    public Animator EyesAnimator;
    public Animator HairAnimator;

    [Header("Public Components")]
    [HideInInspector] public Transform Target;
    public CrowdControl CrowdControl;
    public Buffs Buffs;
    public DeBuffs DeBuffs;

    public enum State
    {
        Spawn,
        Idle,
        Interact,
        Wander,
        Chase,
        Reset,
        Hurt,
        Death,
        Basic,
        Special,
        Ultimate,
    }

    public State state = State.Spawn;

    private void Start()
    {
        spawnState.StartState(this);
    }

    private void Update()
    {
        switch (state)
        {
            case State.Spawn: spawnState.UpdateState(this); break;
            case State.Idle: idleState.UpdateState(this); break;
            case State.Interact: interactState.UpdateState(this); break;
            case State.Wander: wanderState.UpdateState(this); break;
            case State.Chase: chaseState.UpdateState(this); break;
            case State.Reset: resetState.UpdateState(this); break;
            case State.Hurt: hurtState.UpdateState(this); break;
            case State.Death: deathState.UpdateState(this); break;
            case State.Basic: basicState.UpdateState(this); break;
            case State.Special: specialState.UpdateState(this); break;
            case State.Ultimate: ultimateState.UpdateState(this); break;
        }
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case State.Spawn: spawnState.FixedUpdateState(this); break;
            case State.Idle: idleState.FixedUpdateState(this); break;
            case State.Interact: interactState.FixedUpdateState(this); break;
            case State.Wander: wanderState.FixedUpdateState(this); break;
            case State.Chase: chaseState.FixedUpdateState(this); break;
            case State.Reset: resetState.FixedUpdateState(this); break;
            case State.Hurt: hurtState.FixedUpdateState(this); break;
            case State.Death: deathState.FixedUpdateState(this); break;
            case State.Basic: basicState.FixedUpdateState(this); break;
            case State.Special: specialState.FixedUpdateState(this); break;
            case State.Ultimate: ultimateState.FixedUpdateState(this); break;
        }
    }

    public void SetState(State newState)
    {
        if (npc.isDead) return;

        switch (newState)
        {
            case State.Spawn: state = State.Spawn; spawnState.StartState(this); break;
            case State.Idle: state = State.Idle; idleState.StartState(this); break;
            case State.Interact: state = State.Interact; interactState.StartState(this); break;
            case State.Wander: state = State.Wander; wanderState.StartState(this); break;
            case State.Chase: state = State.Chase; chaseState.StartState(this); break;
            case State.Reset: state = State.Reset; resetState.StartState(this); break;
            case State.Hurt: state = State.Hurt; hurtState.StartState(this); break;
            case State.Death: state = State.Death; deathState.StartState(this); break;
            case State.Basic: state = State.Basic; basicState.FixedUpdateState(this); break;
            case State.Special: state = State.Special; specialState.FixedUpdateState(this); break;
            case State.Ultimate: state = State.Ultimate; ultimateState.FixedUpdateState(this); break;
        }
    }

    public void Hurt()
    {
        SetState(State.Hurt);
    }

    public void Death()
    {
        SetState(State.Death);
    }
}
