using UnityEngine;
using System.Collections;

public class EnemyStateMachine : MonoBehaviour
{
    [Header("States")]
    [SerializeField] EnemyState enemySpawnState;
    [SerializeField] EnemyState enemyIdleState;
    [SerializeField] EnemyState enemyWanderState;
    [SerializeField] EnemyState enemyChaseState;
    [SerializeField] EnemyState enemyResetState;

    [Header("Ability")]
    [SerializeField] EnemyAbility enemyBasicAbility;
    [SerializeField] EnemyAbility enemySpecialAbility;
    [SerializeField] EnemyAbility enemyUltimateAbility;

    [Header("Components")]
    public Enemy enemy { get; private set; }
    public Rigidbody2D EnemyRB { get; private set; }
    public Animator EnemyAnimator { get; private set; }

    [Header("Variables")]
    public float IdleTime { get; set; }
    public int AttemptsCount { get; set; }
    public bool IsPlayerInRange { get; set; }

    [Header("Radius")]
    [SerializeField] float wanderRadius; public float WanderRadius => wanderRadius;
    [SerializeField] float basicRadius; public float BasicRadius => basicRadius;
    [SerializeField] float specialRadius; public float SpecialRadius => specialRadius;
    [SerializeField] float ultimateRadius; public float UltimateRadius => ultimateRadius;
    [SerializeField] float deAggroRadius; public float DeAggroRadius => deAggroRadius;

    public bool CanBasic = true;
    public bool CanSpecial = true;
    public bool CanUltimate = true;

    public Vector2 StartingPosition { get; set; }
    public Vector2 WanderPosition { get; set; }
    public Transform Target { get; set; }

    public enum State
    {
        Spawn,
        Idle,
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

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        EnemyRB = GetComponent<Rigidbody2D>();
        EnemyAnimator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        enemySpawnState.StartState(this);

        StartingPosition = transform.position;
    }

    private void Update()
    {
        //Debug.Log(enemyState);

        switch (state)
        {
            case State.Spawn: enemySpawnState.UpdateState(this); break;

            case State.Idle: enemyIdleState.UpdateState(this); break;

            case State.Wander: enemyWanderState.UpdateState(this); break;

            case State.Chase: enemyChaseState.UpdateState(this); break;

            case State.Reset: enemyResetState.UpdateState(this); break;

            case State.Hurt: HurtState(); break;

            case State.Death: DeathState(); break;

            case State.Basic: enemyBasicAbility.AbilityUpdate(this); break;

            case State.Special: enemySpecialAbility.AbilityUpdate(this); break;

            case State.Ultimate: enemyUltimateAbility.AbilityUpdate(this); break;

        }
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case State.Spawn: enemySpawnState.FixedUpdateState(this); break;

            case State.Idle: enemyIdleState.FixedUpdateState(this); break;

            case State.Wander: enemyWanderState.FixedUpdateState(this); break;

            case State.Chase: enemyChaseState.FixedUpdateState(this); break;

            case State.Reset: enemyResetState.FixedUpdateState(this); break;

            case State.Hurt: break;

            case State.Death: break;

            case State.Basic: enemyBasicAbility.AbilityFixedUpdate(this); break;

            case State.Special: enemySpecialAbility.AbilityFixedUpdate(this); break;

            case State.Ultimate: enemyUltimateAbility.AbilityFixedUpdate(this); break;

        }
    }

    public void SetState(State newState)
    {
        switch (newState)
        {
            case State.Spawn: state = State.Spawn; enemySpawnState.StartState(this); break;

            case State.Idle: state = State.Idle; enemyIdleState.StartState(this); break;

            case State.Wander: state = State.Wander; enemyWanderState.StartState(this); break;

            case State.Chase: state = State.Chase; enemyChaseState.StartState(this); break;

            case State.Reset: state = State.Reset; enemyResetState.StartState(this); break;

            case State.Hurt:
                break;
            case State.Death:
                break;
            case State.Basic: state = State.Basic; enemyBasicAbility.AbilityStart(this); break;

            case State.Special: state = State.Special; enemySpecialAbility.AbilityStart(this); break;

            case State.Ultimate: state = State.Ultimate; enemyUltimateAbility.AbilityStart(this); break;

        }
    }

    void HurtState()
    {

    }

    void DeathState()
    {

    }

    public void MoveTowardsTarget(Vector3 _targetPos)
    {
        Vector2 direction = (_targetPos - transform.position).normalized;
        EnemyRB.linearVelocity = direction * enemy.BaseSpeed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Target = other.transform;
            IsPlayerInRange = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(StartingPosition, WanderRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, BasicRadius);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, SpecialRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, UltimateRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(StartingPosition, DeAggroRadius);
    }
}
