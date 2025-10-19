using UnityEngine;
using Unity.Netcode;

public class EnemyStateMachine : NetworkBehaviour
{
    [Header("States")]
    [SerializeField] EnemyState enemySpawnState;
    [SerializeField] EnemyState enemyIdleState;
    [SerializeField] EnemyState enemyWanderState;
    [SerializeField] EnemyState enemyChaseState;
    [SerializeField] EnemyState enemyResetState;
    [SerializeField] EnemyState enemyHurtState;
    [SerializeField] EnemyState enemyDeathState;

    [Header("Ability")]
    [SerializeField] EnemyAbility enemyBasicAbility;
    [SerializeField] EnemyAbility enemySpecialAbility;
    [SerializeField] EnemyAbility enemyUltimateAbility;
    public EnemyAbility currentAbility;

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

    public bool CanDash = false;
    public bool IsAttacking = false;
    public bool CanBasic = true;
    public bool CanSpecial = true;
    public bool CanUltimate = true;

    public LayerMask obstacleLayerMask;
    public Vector2 StartingPosition { get; set; }
    public Vector2 WanderPosition { get; set; }
    public Transform Target { get; set; }
    public CrowdControl CrowdControl;
    public Buffs Buffs;
    public DeBuffs DeBuffs;
    public EnemyDrops Drops;

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

    public enum SkillType 
    { 
        Basic,
        Special,
        Ultimate,
    }

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
            case State.Hurt: enemyHurtState.UpdateState(this); break;
            case State.Death: enemyDeathState.UpdateState(this); break;
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
            case State.Hurt: enemyHurtState.FixedUpdateState(this); break;
            case State.Death: enemyDeathState.FixedUpdateState(this); break;
            case State.Basic: enemyBasicAbility.AbilityFixedUpdate(this); break;
            case State.Special: enemySpecialAbility.AbilityFixedUpdate(this); break;
            case State.Ultimate: enemyUltimateAbility.AbilityFixedUpdate(this); break;
        }
    }

    public void SetState(State newState)
    {
        if (enemy.isDead) return;

        switch (newState)
        {
            case State.Spawn: state = State.Spawn; enemySpawnState.StartState(this); break;
            case State.Idle: state = State.Idle; enemyIdleState.StartState(this); break;
            case State.Wander: state = State.Wander; enemyWanderState.StartState(this); break;
            case State.Chase: state = State.Chase; enemyChaseState.StartState(this); break;
            case State.Reset: state = State.Reset; enemyResetState.StartState(this); break;
            case State.Hurt: state = State.Hurt; enemyHurtState.StartState(this); break;
            case State.Death: state = State.Death; enemyDeathState.StartState(this); break;
            case State.Basic: state = State.Basic; enemyBasicAbility.AbilityStart(this); break;
            case State.Special: state = State.Special; enemySpecialAbility.AbilityStart(this); break;
            case State.Ultimate: state = State.Ultimate; enemyUltimateAbility.AbilityStart(this); break;
        }
    }

    public void MoveTowardsTarget(Vector2 _targetPos)
    {
        if (CrowdControl.immobilize.IsImmobilized) return;

        float distanceToTarget = Vector2.Distance(transform.position, _targetPos);

        if (Target != null)
        {
            if (distanceToTarget <= 1.2f)
            {
                EnemyRB.linearVelocity = Vector2.zero;
                return;
            }
        }

        Vector2 direction = GetDirectionAroundObstacle(_targetPos);
        EnemyRB.linearVelocity = direction * enemy.CurrentSpeed;
    }

    public Vector2 GetDirectionAroundObstacle(Vector2 targetPos)
    {
        Vector2 currentPos = transform.position;
        Vector2 direction = (targetPos - currentPos).normalized;
        Vector2 bestDirection = Vector2.zero;
        float distance = 2f;
        float thickness = 0.3f;
        int rayCount = 13;
        float coneSpread = 180f;

        // straight Ray Cast
        RaycastHit2D centerRay = Physics2D.CircleCast(transform.position, thickness, direction, distance, obstacleLayerMask);
        Debug.DrawRay(transform.position, direction * distance, centerRay ? Color.red : Color.green);

        // If straight path is clear
        if (!centerRay)
        {
            return direction;
        }

        // Get Spread angle
        float angleIncrement = coneSpread / (rayCount - 1);

        for (int i = 0; i < rayCount; i++)
        {
            // Get Offset
            float angleOffset = -coneSpread / 2f + angleIncrement * i;
            Vector2 dir = Quaternion.Euler(0, 0, angleOffset) * direction;

            // Ray in offset Direction
            RaycastHit2D hit = Physics2D.CircleCast(transform.position, thickness, dir, distance, obstacleLayerMask);
            Debug.DrawRay(transform.position, dir * distance, hit ? Color.red : Color.green);

            // If path is clear
            if (!hit && bestDirection == Vector2.zero)
            {
                bestDirection = dir;
            }
        }

        // If we found a valid direction
        return bestDirection == Vector2.zero ? Vector2.zero : bestDirection.normalized;
    }

    public void Interrupt()
    {
        if (currentAbility == null) return;
        if (currentAbility.currentState != EnemyAbility.State.Cast) return;

        enemy.CastBar.InterruptCastBar();
        currentAbility.DoneState(false, this);
    }

    public void Stagger()
    {
        enemy.CastBar.InterruptCastBar();

        if (currentAbility != null)
        {
            currentAbility.DoneState(true, this);
        }
        else
        {
            SetState(State.Hurt);
        }
    }

    public void DespawnEnemy()
    {
        GetComponent<NetworkObject>().Despawn();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("NPC"))
        {
            Target = other.transform;
            IsPlayerInRange = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player") || !collision.gameObject.CompareTag("NPC")) return;
        if (enemy.isDummy) return;
        if (Buffs.phase.IsPhased) return;

        Player player = collision.gameObject.GetComponent<Player>();
        CrowdControl playerCC = player.GetComponent<CrowdControl>();

        if (player != null && playerCC != null)
        {
            player.TakeDamage(1, DamageType.Flat, NetworkObject);
            Vector2 dir = player.transform.position - transform.position;
            playerCC.knockBack.KnockBack(dir, 5, .3f);
        }

        NPC npc = collision.gameObject.GetComponent<NPC>();
        CrowdControl npcCC = npc.GetComponent<CrowdControl>();

        if (npc != null && npcCC != null)
        {
            npc.TakeDamage(1, DamageType.Flat, NetworkObject);
            Vector2 dir = npc.transform.position - transform.position;
            npcCC.knockBack.KnockBack(dir, 5, .3f);
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

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(WanderPosition, 0.2f);
    }
}