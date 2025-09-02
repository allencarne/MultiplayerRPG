using Unity.Netcode;
using UnityEngine;

public class NPCStateMachine : NetworkBehaviour
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
    public LayerMask obstacleLayerMask;

    public bool IsEnemyInRange { get; set; }

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

    public void MoveTowardsTarget(Vector2 _targetPos)
    {
        if (CrowdControl.immobilize.IsImmobilized) return;
        Vector2 direction = GetDirectionAroundObstacle(_targetPos);
        rb.linearVelocity = direction * npc.BaseSpeed;
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

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Target = other.transform;
            IsEnemyInRange = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Enemy")) return;
        if (Buffs.phase.IsPhased) return;

        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        CrowdControl cc = enemy.GetComponent<CrowdControl>();

        if (enemy != null && cc != null)
        {
            enemy.TakeDamage(1, DamageType.Flat, NetworkObject);
            Vector2 dir = enemy.transform.position - transform.position;
            cc.knockBack.KnockBack(dir, 5, .3f);
        }
    }
}
