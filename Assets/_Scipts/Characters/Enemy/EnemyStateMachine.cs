using UnityEngine;
using System.Collections;
using Unity.Netcode;

public class EnemyStateMachine : NetworkBehaviour
{
    [Header("States")]
    [SerializeField] EnemyState enemySpawnState;
    [SerializeField] EnemyState enemyIdleState;
    [SerializeField] EnemyState enemyWanderState;
    [SerializeField] EnemyState enemyChaseState;
    [SerializeField] EnemyState enemyResetState;
    [SerializeField] EnemyState enemyDeathState;

    [Header("Ability")]
    [SerializeField] EnemyAbility enemyBasicAbility;
    [SerializeField] EnemyAbility enemySpecialAbility;
    [SerializeField] EnemyAbility enemyUltimateAbility;

    public LayerMask obstacleLayerMask;

    [Header("Components")]
    public Enemy enemy { get; private set; }
    public Rigidbody2D EnemyRB { get; private set; }
    public Animator EnemyAnimator { get; private set; }
    public Collider2D Collider { get; private set; }

    [Header("Variables")]
    public int AttemptsCount { get; set; }
    public bool IsPlayerInRange { get; set; }

    [Header("Radius")]
    [SerializeField] float wanderRadius; public float WanderRadius => wanderRadius;
    [SerializeField] float basicRadius; public float BasicRadius => basicRadius;
    [SerializeField] float specialRadius; public float SpecialRadius => specialRadius;
    [SerializeField] float ultimateRadius; public float UltimateRadius => ultimateRadius;
    [SerializeField] float deAggroRadius; public float DeAggroRadius => deAggroRadius;

    public bool IsAttacking = false;
    public bool CanBasic = true;
    public bool CanSpecial = true;
    public bool CanUltimate = true;

    public Vector2 StartingPosition { get; set; }
    public Vector2 WanderPosition { get; set; }
    public Transform Target { get; set; }
    public CrowdControl CrowdControl;
    public Buffs Buffs;
    public DeBuffs DeBuffs;
    public Coroutine ImpactCoroutine;
    public Coroutine RecoveryCoroutine;

    public enum State
    {
        Spawn,
        Idle,
        Wander,
        Chase,
        Reset,
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
        Collider = GetComponent<Collider2D>();
    }

    private void Start()
    {
        enemySpawnState.StartState(this);

        StartingPosition = transform.position;
    }

    private void Update()
    {
        switch (state)
        {
            case State.Spawn: enemySpawnState.UpdateState(this); break;

            case State.Idle: enemyIdleState.UpdateState(this); break;

            case State.Wander: enemyWanderState.UpdateState(this); break;

            case State.Chase: enemyChaseState.UpdateState(this); break;

            case State.Reset: enemyResetState.UpdateState(this); break;

            case State.Death: enemyDeathState.UpdateState(this); break;

            case State.Basic: enemyBasicAbility.AbilityUpdate(this); break;

            case State.Special: enemySpecialAbility.AbilityUpdate(this); break;

            case State.Ultimate: enemyUltimateAbility.AbilityUpdate(this); break;
        }
    }

    private void FixedUpdate()
    {
        if (CrowdControl.IsImmobilized) return;

        switch (state)
        {
            case State.Spawn: enemySpawnState.FixedUpdateState(this); break;

            case State.Idle: enemyIdleState.FixedUpdateState(this); break;

            case State.Wander: enemyWanderState.FixedUpdateState(this); break;

            case State.Chase: enemyChaseState.FixedUpdateState(this); break;

            case State.Reset: enemyResetState.FixedUpdateState(this); break;

            case State.Death: enemyDeathState.FixedUpdateState(this); break;

            case State.Basic: enemyBasicAbility.AbilityFixedUpdate(this); break;

            case State.Special: enemySpecialAbility.AbilityFixedUpdate(this); break;

            case State.Ultimate: enemyUltimateAbility.AbilityFixedUpdate(this); break;
        }
    }

    public void SetState(State newState)
    {
        if (enemy.Health.Value <= 0) return;

        switch (newState)
        {
            case State.Spawn: state = State.Spawn; enemySpawnState.StartState(this); break;

            case State.Idle: state = State.Idle; enemyIdleState.StartState(this); break;

            case State.Wander: state = State.Wander; enemyWanderState.StartState(this); break;

            case State.Chase: state = State.Chase; enemyChaseState.StartState(this); break;

            case State.Reset: state = State.Reset; enemyResetState.StartState(this); break;

            case State.Death: state = State.Death; enemyDeathState.StartState(this); break;

            case State.Basic: state = State.Basic; enemyBasicAbility.AbilityStart(this); break;

            case State.Special: state = State.Special; enemySpecialAbility.AbilityStart(this); break;

            case State.Ultimate: state = State.Ultimate; enemyUltimateAbility.AbilityStart(this); break;
        }
    }

    public void MoveTowardsTarget(Vector3 _targetPos)
    {
        Vector2 direction = GetDirectionAroundObstacle(_targetPos);

        //Vector2 direction = (_targetPos - transform.position).normalized;
        EnemyRB.linearVelocity = direction * enemy.BaseSpeed;
    }

    public Vector3 GetDirectionAroundObstacle(Vector3 targetPos)
    {
        Vector2 baseDirection = (targetPos - transform.position).normalized;
        float rayDistance = 2f;
        float rayThickness = 0.3f;
        int rayCount = 13;
        float expandedConeAngle = 180f;

        Vector3 bestDirection = Vector3.zero;

        // First: straight line check using thick ray
        RaycastHit2D centerHit = Physics2D.CircleCast(transform.position, rayThickness, baseDirection, rayDistance, obstacleLayerMask);
        Debug.DrawRay(transform.position, baseDirection * rayDistance, centerHit ? Color.red : Color.green);

        if (!centerHit)
            return baseDirection;

        // If blocked: scan with wide cone
        float angleIncrement = expandedConeAngle / (rayCount - 1);
        for (int i = 0; i < rayCount; i++)
        {
            float angleOffset = -expandedConeAngle / 2f + angleIncrement * i;
            Vector2 dir = Quaternion.Euler(0, 0, angleOffset) * baseDirection;
            RaycastHit2D hit = Physics2D.CircleCast(transform.position, rayThickness, dir, rayDistance, obstacleLayerMask);

            Debug.DrawRay(transform.position, dir * rayDistance, hit ? Color.red : Color.green);

            if (!hit && bestDirection == Vector3.zero)
            {
                bestDirection = dir;
            }
        }

        return bestDirection == Vector3.zero ? Vector3.zero : bestDirection.normalized;
    }

    public void Death()
    {
        SetState(State.Death);
    }

    public void DespawnEnemy()
    {
        GetComponent<NetworkObject>().Despawn();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Target = other.transform;
            IsPlayerInRange = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (enemy.isDummy) return;
        if (!collision.gameObject.CompareTag("Player")) return;

        Player player = collision.gameObject.GetComponent<Player>();
        CrowdControl cc = player.GetComponent<CrowdControl>();

        if (player != null && cc != null)
        {
            player.TakeDamage(1, DamageType.Flat, NetworkObject);
            Vector2 dir = player.transform.position - transform.position;
            cc.KnockBack(dir, 15, .3f);
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

    public void HandlePotentialInterrupt(Coroutine coroutine)
    {
        if (CrowdControl.IsInterrupted)
        {
            if (enemy.CastBar.castBarFill.color == Color.black)
            {
                if (IsServer)
                {
                    enemy.CastBar.InterruptCastBar();
                }
                else
                {
                    enemy.CastBar.InterruptServerRpc();
                }

                if (coroutine != null)
                {
                    StopCoroutine(coroutine);
                }

                IsAttacking = false;
                SetState(State.Idle);
                return;
            }
        }
    }
}
