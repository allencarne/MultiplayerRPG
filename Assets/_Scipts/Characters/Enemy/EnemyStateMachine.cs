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

    public bool CanDash = false;
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
            case State.Death: state = State.Death; enemyDeathState.StartState(this); break;
            case State.Basic: state = State.Basic; enemyBasicAbility.AbilityStart(this); break;
            case State.Special: state = State.Special; enemySpecialAbility.AbilityStart(this); break;
            case State.Ultimate: state = State.Ultimate; enemyUltimateAbility.AbilityStart(this); break;
        }
    }

    public void MoveTowardsTarget(Vector2 _targetPos)
    {
        if (CrowdControl.immobilize.IsImmobilized) return;
        Vector2 direction = GetDirectionAroundObstacle(_targetPos);
        EnemyRB.linearVelocity = direction * enemy.BaseSpeed;
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

    public void Death()
    {
        SetState(State.Death);
    }

    [ClientRpc]
    public void HandleDeathClientRPC()
    {
        Collider.enabled = false;
        enemy.CastBar.gameObject.SetActive(false);
        enemy.shadowSprite.enabled = false;
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
        if (!collision.gameObject.CompareTag("Player")) return;
        if (enemy.isDummy) return;
        if (Buffs.phase.IsPhased) return;

        Player player = collision.gameObject.GetComponent<Player>();
        CrowdControl cc = player.GetComponent<CrowdControl>();

        if (player != null && cc != null)
        {
            player.TakeDamage(1, DamageType.Flat, NetworkObject);
            Vector2 dir = player.transform.position - transform.position;
            cc.knockBack.KnockBack(dir, 5, .3f);
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

    public void HandlePotentialInterrupt()
    {
        if (!CrowdControl.IsInterrupted) return;
        if (enemy.CastBar.castBarFill.color != Color.black) return;

        if (IsServer)
        {
            enemy.CastBar.InterruptCastBar();
        }
        else
        {
            enemy.CastBar.InterruptServerRpc();
        }

        IsAttacking = false;
        SetState(State.Idle);
        return;
    }

    public IEnumerator CoolDownTime(SkillType type, float skillCoolDown)
    {
        yield return new WaitForSeconds(skillCoolDown);

        switch (type)
        {
            case SkillType.Basic: CanBasic = true; break;
            case SkillType.Special: CanSpecial = true; break;
            case SkillType.Ultimate: CanUltimate = true; break;
        }
    }

    public IEnumerator CastTime(SkillType type, float castTime, float impactTime, float recoveryTime, EnemyAbility ability)
    {
        yield return new WaitForSeconds(castTime);

        if (!IsAttacking) yield break;
        if (enemy.isDead) yield break;

        switch (type)
        {
            case SkillType.Basic: EnemyAnimator.Play("Basic Impact"); break;
            case SkillType.Special: EnemyAnimator.Play("Special Impact"); break;
            case SkillType.Ultimate: EnemyAnimator.Play("Ultimate Impact"); break;
        }

        StartCoroutine(ImpactTime(type, impactTime, recoveryTime, ability));
    }

    IEnumerator ImpactTime(SkillType type, float impactTime, float recoveryTime, EnemyAbility ability)
    {
        ability.Impact(this);

        yield return new WaitForSeconds(impactTime);

        if (!IsAttacking) yield break;
        if (enemy.isDead) yield break;

        // Start Recovery
        switch (type)
        {
            case SkillType.Basic: EnemyAnimator.Play("Basic Recovery"); break;
            case SkillType.Special: EnemyAnimator.Play("Special Recovery"); break;
            case SkillType.Ultimate: EnemyAnimator.Play("Ultimate Recovery"); break;
        }

        enemy.CastBar.StartRecovery(recoveryTime, enemy.CurrentAttackSpeed);

        StartCoroutine(RecoveryTime(recoveryTime));
    }

    public IEnumerator RecoveryTime(float recoverTime)
    {
        yield return new WaitForSeconds(recoverTime);

        if (!IsAttacking) yield break;
        if (enemy.isDead) yield break;

        IsAttacking = false;
        SetState(State.Idle);
    }
}
