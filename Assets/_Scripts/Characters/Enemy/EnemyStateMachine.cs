using UnityEngine;
using Unity.Netcode;

public class EnemyStateMachine : StateMachine
{
    [Header("States")]
    public EnemyState state;

    [Header("Skills")]
    [SerializeField] EnemySkill enemyBasicAbility;
    [SerializeField] EnemySkill enemySpecialAbility;
    [SerializeField] EnemySkill enemyUltimateAbility;
    [HideInInspector] public EnemySkill CurrentSkill;

    [Header("Scripts")]
    //public CrowdControl CrowdControl;
    //public Buffs Buffs;
    //public DeBuffs DeBuffs;
    public EnemyDrops Drops;

    [Header("Components")]
    public Enemy enemy { get; private set; }
    //public Rigidbody2D RigidBody2D;
    //public Collider2D Collider2D;

    [Header("Variables")]
    public int AttemptsCount { get; set; }
    public bool IsPlayerInRange { get; set; }
    public Vector2 StartingPosition { get; set; }
    public Vector2 WanderPosition { get; set; }
    public LayerMask obstacleLayerMask;

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

    public enum SkillType 
    { 
        Basic,
        Special,
        Ultimate,
    }

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        RigidBody2D = GetComponent<Rigidbody2D>();
        //EnemyAnimator = GetComponentInChildren<Animator>();
        Collider2D = GetComponent<Collider2D>();
    }

    private void Start()
    {
        SetState(new EnemySpawnState(this));

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
        if (!IsServer) return;
        state.UpdateState();
    }

    private void FixedUpdate()
    {
        if (!IsServer) return;
        state.FixedUpdateState();
    }

    public void SetState(EnemyState newState)
    {
        state?.ExitState();
        state = newState;
        state.EnterState();
    }

    public void SetSkill(SkillType newSkill)
    {
        switch (newSkill)
        {
            case SkillType.Basic: CurrentSkill = enemyBasicAbility; break;
            case SkillType.Special: CurrentSkill = enemySpecialAbility; break;
            case SkillType.Ultimate: CurrentSkill = enemyUltimateAbility; break;
        }

        SetState(new EnemyAttackState(this, CurrentSkill));
    }

    public void Interrupt()
    {
        if (enemy.stats.isDead) return;
        if (CurrentSkill == null) return;
        if (CurrentSkill.currentState != ActiveSkillData.SkillPhase.Cast) return;

        enemy.stats.OnInterrupted?.Invoke();

        enemy.CastBar.StartInterrupt();
        CurrentSkill.DoneState(false, this);
    }

    public void Stagger()
    {
        if (enemy.stats.isDead) return;

        enemy.CastBar.StartInterrupt();

        if (CurrentSkill != null)
        {
            CurrentSkill.DoneState(true, this);
        }
        else
        {
            SetState(new EnemyStaggerState(this));
        }
    }

    public void TransitionToIdle()
    {
        switch (enemy.Data.Enemy_Type)
        {
            case EnemyType.Enemy:
                SetState(new EnemyIdleState(this));
                break;

            case EnemyType.Dummy:
                SetState(new DummyIdleState(this));
                break;
        }
    }

    public void TransitionToReset()
    {
        switch (enemy.Data.Enemy_Type)
        {
            case EnemyType.Enemy:
                SetState(new EnemyResetState(this));
                break;

            case EnemyType.Dummy:
                SetState(new DummyResetState(this));
                break;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (enemy.stats.isDead) return;
        if (state is EnemyResetState) return;

        if (other.CompareTag("Player") || other.CompareTag("NPC"))
        {
            if (Target == null)
            {
                Target = other.transform;
                IsPlayerInRange = true;
            }
            else if (SecondTarget == null && Target != other.transform)
            {
                SecondTarget = other.transform;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (enemy == null || enemy.Data == null) return;

        Gizmos.color = Color.darkBlue;
        Gizmos.DrawWireSphere(StartingPosition, enemy.Data.WanderRadius);

        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(StartingPosition, enemy.Data.DeAggroRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, enemy.Data.BasicRadius);

        Gizmos.color = Color.orange;
        Gizmos.DrawWireSphere(transform.position, enemy.Data.SpecialRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, enemy.Data.UltimateRadius);

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
                    RigidBody2D.linearVelocity = Vector2.zero;
                    return;
                }
            }
            else
            {
                if (distanceToTarget <= 1.2f)
                {
                    RigidBody2D.linearVelocity = Vector2.zero;
                    return;
                }
            }
        }

        Vector2 direction = GetDirectionAroundObstacle(_targetPos);
        RigidBody2D.linearVelocity = direction * enemy.stats.TotalSpeed;
    }

    public Vector2 GetDirectionAroundObstacle(Vector2 targetPos)
    {
        Vector2 currentPos = transform.position;
        Vector2 direction = (targetPos - currentPos).normalized;
        Vector2 bestDirection = Vector2.zero;

        float distance = 2f;
        float castOffset = 0f;
        int rayCount = 21;
        float coneSpread = 225;

        // Straight ray
        Vector2 castOrigin = currentPos + direction * castOffset;
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

            castOrigin = currentPos + dir * castOffset;
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