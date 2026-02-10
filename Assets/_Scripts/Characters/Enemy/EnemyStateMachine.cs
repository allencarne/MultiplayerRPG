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
    [SerializeField] EnemyState enemyStaggerState;
    [SerializeField] EnemyState enemyDeathState;

    [Header("Skills")]
    [SerializeField] EnemySkill enemyBasicAbility;
    [SerializeField] EnemySkill enemySpecialAbility;
    [SerializeField] EnemySkill enemyUltimateAbility;
    [HideInInspector] public EnemySkill CurrentSkill;

    [Header("Scripts")]
    public CrowdControl CrowdControl;
    public Buffs Buffs;
    public DeBuffs DeBuffs;
    public EnemyDrops Drops;

    [Header("Components")]
    public Enemy enemy { get; private set; }
    public Rigidbody2D EnemyRB { get; private set; }
    public Animator EnemyAnimator { get; private set; }
    public Collider2D Collider { get; private set; }

    [Header("Variables")]
    public int AttemptsCount { get; set; }
    public bool IsPlayerInRange { get; set; }
    public Vector2 StartingPosition { get; set; }
    public Vector2 WanderPosition { get; set; }
    public LayerMask obstacleLayerMask;

    [Header("Enemy Radius")]
    [SerializeField] float wanderRadius; public float WanderRadius => wanderRadius;
    [SerializeField] float deAggroRadius; public float DeAggroRadius => deAggroRadius;

    [Header("Attack Radius")]
    [SerializeField] float basicRadius; public float BasicRadius => basicRadius;
    [SerializeField] float specialRadius; public float SpecialRadius => specialRadius;
    [SerializeField] float ultimateRadius; public float UltimateRadius => ultimateRadius;

    [Header("Bools")]
    public bool IsAttacking = false;
    public bool isResetting = false;
    public bool CanDash = false;
    public bool CanBasic = true;
    public bool CanSpecial = true;
    public bool CanUltimate = true;

    [Header("Start Buffs")]
    public bool hasMightOnStart = false;
    public bool hasSwiftnessOnStart = false;
    public bool hasAlacrityOnStart = false;
    public bool hasProtectionOnStart = false;

    public Transform Target { get; set; }
    public Transform SecondTarget { get; set; }

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

        int randomNumber = Random.Range(0, 100);
        if (randomNumber <= 5)
        {
            Buffs.might.StartMight(1, -1);
            hasMightOnStart = true;
        }

        int randomNumber2 = Random.Range(0, 100);
        if (randomNumber2 <= 5)
        {
            Buffs.swiftness.StartSwiftness(1, -1);
            hasSwiftnessOnStart = true;
        }

        int randomNumber3 = Random.Range(0, 100);
        if (randomNumber3 <= 5)
        {
            Buffs.alacrity.StartAlacrity(1, -1);
            hasAlacrityOnStart = true;
        }

        int randomNumber4 = Random.Range(0, 100);
        if (randomNumber4 <= 5)
        {
            Buffs.protection.StartProtection(1, -1);
            hasProtectionOnStart = true;
        }
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
            case State.Hurt: enemyStaggerState.UpdateState(this); break;
            case State.Death: enemyDeathState.UpdateState(this); break;
            case State.Basic: enemyBasicAbility.UpdateSkill(this); break;
            case State.Special: enemySpecialAbility.UpdateSkill(this); break;
            case State.Ultimate: enemyUltimateAbility.UpdateSkill(this); break;
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
            case State.Hurt: enemyStaggerState.FixedUpdateState(this); break;
            case State.Death: enemyDeathState.FixedUpdateState(this); break;
            case State.Basic: enemyBasicAbility.FixedUpdateSkill(this); break;
            case State.Special: enemySpecialAbility.FixedUpdateSkill(this); break;
            case State.Ultimate: enemyUltimateAbility.FixedUpdateSkill(this); break;
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
            case State.Hurt: state = State.Hurt; enemyStaggerState.StartState(this); break;
            case State.Death: state = State.Death; enemyDeathState.StartState(this); break;
            case State.Basic: state = State.Basic; enemyBasicAbility.StartSkill(this); break;
            case State.Special: state = State.Special; enemySpecialAbility.StartSkill(this); break;
            case State.Ultimate: state = State.Ultimate; enemyUltimateAbility.StartSkill(this); break;
        }
    }

    public void Interrupt()
    {
        if (enemy.stats.isDead) return;
        if (CurrentSkill == null) return;
        if (CurrentSkill.currentState != EnemySkill.State.Cast) return;

        enemy.CastBar.StartInterrupt();
        CurrentSkill.DoneState(false, this);
    }

    public void Stagger()
    {
        if (enemy.stats.isDead) return;
        if (Buffs.immoveable.IsImmovable) return;

        enemy.CastBar.StartInterrupt();

        if (CurrentSkill != null)
        {
            CurrentSkill.DoneState(true, this);
        }
        else
        {
            SetState(State.Hurt);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (enemy.stats.isDead) return;
        if (state == State.Reset) return;

        if (other.CompareTag("Player") || other.CompareTag("NPC"))
        {
            if (Target == null)
            {
                Target = other.transform;
                IsPlayerInRange = true;
            }

            if (Target == other.transform) return;

            if (SecondTarget == null && Target != null)
            {
                SecondTarget = other.transform;
            }
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

    public void DespawnEnemy()
    {
        if (!IsServer) return;

        NetworkObject netObj = GetComponent<NetworkObject>();
        if (netObj == null)
        {
            Debug.LogError($"[{gameObject.name}] NetworkObject is null!");
            return;
        }

        if (!netObj.IsSpawned)
        {
            Debug.LogWarning($"[{gameObject.name}] Already despawned!");
            return;
        }

        GetComponent<NetworkObject>().Despawn();
    }

    #region Pathing

    public void MoveTowardsTarget(Vector2 _targetPos, bool isReset = false)
    {
        if (CrowdControl.immobilize.IsImmobilized) return;

        float distanceToTarget = Vector2.Distance(transform.position, _targetPos);

        if (Target != null)
        {
            if (isReset)
            {
                if (distanceToTarget <= 0.5f)
                {
                    EnemyRB.linearVelocity = Vector2.zero;
                    return;
                }
            }
            else
            {
                if (distanceToTarget <= 1.2f)
                {
                    EnemyRB.linearVelocity = Vector2.zero;
                    return;
                }
            }
        }

        Vector2 direction = GetDirectionAroundObstacle(_targetPos);
        EnemyRB.linearVelocity = direction * enemy.stats.TotalSpeed;
    }

    public Vector2 GetDirectionAroundObstacle(Vector2 targetPos)
    {
        Vector2 currentPos = transform.position;
        Vector2 direction = (targetPos - currentPos).normalized;
        Vector2 bestDirection = Vector2.zero;

        float distance = .5f;
        int rayCount = 21;
        float coneSpread = 225;

        // Straight ray
        Vector2 castOrigin = currentPos + direction;
        RaycastHit2D centerRay = Physics2D.Raycast(castOrigin, direction, distance, obstacleLayerMask);
        Debug.DrawRay(castOrigin, direction * distance, centerRay ? Color.red : Color.green);

        // I straight path is clear
        if (!centerRay) return direction;

        // Spread
        float angleIncrement = coneSpread / (rayCount - 1);
        float bestScore = -Mathf.Infinity;

        for (int i = 0; i < rayCount; i++)
        {
            float angleOffset = -coneSpread / 2f + angleIncrement * i;
            Vector2 dir = Quaternion.Euler(0, 0, angleOffset) * direction;

            castOrigin = currentPos + dir;
            RaycastHit2D hit = Physics2D.Raycast(castOrigin, dir, distance, obstacleLayerMask);
            Debug.DrawRay(castOrigin, dir * distance, hit ? Color.red : Color.green);

            if (!hit)
            {
                // Score based on alignment with target direction
                float score = Vector2.Dot(dir, direction);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestDirection = dir;
                }
            }
        }

        // Return best valid direction
        return bestDirection == Vector2.zero ? Vector2.zero : bestDirection.normalized;
    }

    #endregion
}