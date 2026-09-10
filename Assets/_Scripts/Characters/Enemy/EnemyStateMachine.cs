using UnityEngine;
using Unity.Netcode;

public class EnemyStateMachine : StateMachine
{
    [Header("States")]
    public EnemyState state;

    [Header("Skills")]
    [HideInInspector] public ActiveSkill CurrentSkill;
    PassiveSkill passiveInstance;

    [Header("Scripts")]
    public EnemyDrops Drops;

    [Header("Components")]
    public Enemy enemy { get; private set; }

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
            case SkillType.Basic: CurrentSkill = new ActiveSkill(enemy.Data.BasicAbility, 0); break;
            case SkillType.Special: CurrentSkill = new ActiveSkill(enemy.Data.SpecialAbility, 0); break;
            case SkillType.Ultimate: CurrentSkill = new ActiveSkill(enemy.Data.UltimateAbility, 0); break;
        }

        SetState(new EnemyAttackState(this, CurrentSkill));
    }

    public void SetPassive(PassiveSkillData passiveData, int index = 0)
    {
        // End any existing passive first
        passiveInstance?.EndPassive(this);

        if (passiveData == null)
        {
            passiveInstance = null;
            return;
        }

        passiveInstance = new PassiveSkill(passiveData, index);
        passiveInstance.StartPassive(this);
    }

    public void ClearPassive()
    {
        passiveInstance?.EndPassive(this);
        passiveInstance = null;
    }

    public void Interrupt()
    {
        if (enemy.stats.isDead) return;
        if (CurrentSkill == null) return;
        if (CurrentSkill.currentState != ActiveSkillData.SkillPhase.Cast) return;

        enemy.stats.OnInterrupted?.Invoke();

        CastBar.StartInterrupt();
        CurrentSkill.DoneState(false, this);
    }

    public void Stagger()
    {
        if (enemy.stats.isDead) return;

        CastBar.StartInterrupt();

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

        Vector2 direction = Vector2.zero;
        if (Pathfinding != null)
        {
            direction = Pathfinding.GetDirectionAroundObstacle(transform.position, _targetPos, obstacleLayerMask);
        }
        else
        {
            direction = (_targetPos - (Vector2)transform.position).normalized;
        }

        RigidBody2D.linearVelocity = direction * enemy.stats.TotalSpeed;
    }

    protected override ActiveSkillData GetSkillData(ActiveSkillData.SkillType type, int index) => type switch
    {
        ActiveSkillData.SkillType.Basic => enemy.Data.BasicAbility,
        ActiveSkillData.SkillType.Mobility => enemy.Data.SpecialAbility,
        ActiveSkillData.SkillType.Ultimate => enemy.Data.UltimateAbility,
        _ => null
    };
}