using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class NPCStateMachine : NetworkBehaviour
{
    [Header("States")]
    [SerializeField] NPCState spawnState;
    [SerializeField] NPCState idleState;
    [SerializeField] NPCState wanderState;
    [SerializeField] NPCState chaseState;
    [SerializeField] NPCState resetState;
    [SerializeField] NPCState StaggerState;
    [SerializeField] NPCState deathState;

    [Header("Skills")]
    [SerializeField] NPCSkill basicSkill;
    [SerializeField] NPCSkill specialSkill;
    [SerializeField] NPCSkill ultimateSkill;
    [HideInInspector] public NPCSkill CurrentSkill;

    [Header("Animators")]
    public Animator HeadAnimator;
    public Animator BodyAnimator;

    public Animator ChestAnimator;
    public Animator LegsAnimator;

    public Animator SwordAnimator;

    [Header("Status Effects")]
    public CrowdControl CrowdControl;
    public Buffs Buffs;
    public DeBuffs DeBuffs;

    [Header("Bools")]
    public bool IsEnemyInRange = false;
    public bool IsAttacking = false;
    public bool IsSliding = false;
    public bool CanBasic = true;
    public bool CanSpecial = false;
    public bool CanUltimate = false;

    [Header("Variables")]
    public Vector2 StartingPosition;

    [Header("Radius")]
    public float BasicRadius;
    public float DeAggroRadius;

    [Header("Components")]
    public Transform Target;
    [SerializeField] Collider2D Collider;
    public NPC npc;
    public Rigidbody2D NpcRB;
    public LayerMask obstacleLayerMask;

    [Header("Patrol")]
    public int PatrolIndex = 0;
    public bool PatrolForward = true;

    public enum State
    {
        Spawn,
        Idle,
        Wander,
        Chase,
        Reset,
        Stagger,
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

    private void Start()
    {
        spawnState.StartState(this);
        StartingPosition = transform.position;
    }

    private void Update()
    {
        switch (state)
        {
            case State.Spawn: spawnState.UpdateState(this); break;
            case State.Idle: idleState.UpdateState(this); break;
            case State.Wander: wanderState.UpdateState(this); break;
            case State.Chase: chaseState.UpdateState(this); break;
            case State.Reset: resetState.UpdateState(this); break;
            case State.Stagger: StaggerState.UpdateState(this); break;
            case State.Death: deathState.UpdateState(this); break;
            case State.Basic: basicSkill.UpdateSkill(this); break;
            case State.Special: specialSkill.UpdateSkill(this); break;
            case State.Ultimate: ultimateSkill.UpdateSkill(this); break;
        }
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case State.Spawn: spawnState.FixedUpdateState(this); break;
            case State.Idle: idleState.FixedUpdateState(this); break;
            case State.Wander: wanderState.FixedUpdateState(this); break;
            case State.Chase: chaseState.FixedUpdateState(this); break;
            case State.Reset: resetState.FixedUpdateState(this); break;
            case State.Stagger: StaggerState.FixedUpdateState(this); break;
            case State.Death: deathState.FixedUpdateState(this); break;
            case State.Basic: basicSkill.FixedUpdateSkill(this); break;
            case State.Special: specialSkill.FixedUpdateSkill(this); break;
            case State.Ultimate: ultimateSkill.FixedUpdateSkill(this); break;
        }
    }

    public void SetState(State newState)
    {
        if (npc.IsDead) return;

        switch (newState)
        {
            case State.Spawn: state = State.Spawn; spawnState.StartState(this); break;
            case State.Idle: state = State.Idle; idleState.StartState(this); break;
            case State.Wander: state = State.Wander; wanderState.StartState(this); break;
            case State.Chase: state = State.Chase; chaseState.StartState(this); break;
            case State.Reset: state = State.Reset; resetState.StartState(this); break;
            case State.Stagger: state = State.Stagger; StaggerState.StartState(this); break;
            case State.Death: state = State.Death; deathState.StartState(this); break;
            case State.Basic: state = State.Basic; basicSkill.StartSkill(this); break;
            case State.Special: state = State.Special; specialSkill.StartSkill(this); break;
            case State.Ultimate: state = State.Ultimate; ultimateSkill.StartSkill(this); break;
        }
    }

    public void Interrupt()
    {
        if (CurrentSkill == null) return;
        if (CurrentSkill.currentState != NPCSkill.State.Cast) return;

        npc.CastBar.StartInterrupt();
        CurrentSkill.DoneState(false, this);
    }

    public void Stagger()
    {
        if (Buffs.immoveable.IsImmovable) return;

        npc.CastBar.StartInterrupt();

        if (CurrentSkill != null)
        {
            CurrentSkill.DoneState(true, this);
        }
        else
        {
            SetState(State.Stagger);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (state == State.Reset) return;
            if (other.GetComponent<Enemy>().IsDummy) return;

            Target = other.transform;
            IsEnemyInRange = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(StartingPosition, DeAggroRadius);
    }

    #region Pathing

    public void MoveTowardsTarget(Vector2 _targetPos)
    {
        if (CrowdControl.immobilize.IsImmobilized) return;

        float distanceToTarget = Vector2.Distance(transform.position, _targetPos);

        if (Target != null)
        {
            if (distanceToTarget <= 1.2f)
            {
                NpcRB.linearVelocity = Vector2.zero;
                return;
            }
        }

        Vector2 direction = GetDirectionAroundObstacle(_targetPos);
        NpcRB.linearVelocity = direction * npc.stats.TotalSpeed;
    }

    public Vector2 GetDirectionAroundObstacle(Vector2 targetPos)
    {
        Vector2 currentPos = transform.position;
        Vector2 direction = (targetPos - currentPos).normalized;
        Vector2 bestDirection = Vector2.zero;

        // Tunable parameters
        float distance = 3f;        // how far ahead to check
        float thickness = 0.25f;    // character radius
        int rayCount = 17;          // number of rays in cone
        float coneSpread = 120f;    // angle span in degrees

        // Straight ray
        RaycastHit2D centerRay = Physics2D.CircleCast(currentPos, thickness, direction, distance, obstacleLayerMask);
        Debug.DrawRay(currentPos, direction * distance, centerRay ? Color.red : Color.green);

        // If straight path is clear
        if (!centerRay)
            return direction;

        // Spread calculation
        float angleIncrement = coneSpread / (rayCount - 1);
        float bestScore = -Mathf.Infinity;

        for (int i = 0; i < rayCount; i++)
        {
            float angleOffset = -coneSpread / 2f + angleIncrement * i;
            Vector2 dir = Quaternion.Euler(0, 0, angleOffset) * direction;

            RaycastHit2D hit = Physics2D.CircleCast(currentPos, thickness, dir, distance, obstacleLayerMask);
            Debug.DrawRay(currentPos, dir * distance, hit ? Color.red : Color.green);

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

    #region RPC

    [ServerRpc]
    public void RequestDisableColliderServerRpc(bool isEnabled)
    {
        Collider.enabled = isEnabled;
        npc.SwordSprite.enabled = isEnabled;
        npc.EyeSprite.enabled = isEnabled;
        npc.HairSprite.enabled = isEnabled;
        ApplyColliderStateClientRpc(isEnabled);
    }

    [ClientRpc]
    void ApplyColliderStateClientRpc(bool isEnabled)
    {
        Collider.enabled = isEnabled;
        npc.SwordSprite.enabled = isEnabled;
        npc.EyeSprite.enabled = isEnabled;
        npc.HairSprite.enabled = isEnabled;
    }

    [ServerRpc]
    public void RequestRespawnServerRpc()
    {
        npc.stats.GiveHeal(100, HealType.Percentage);
    }

    #endregion

    #region Animation

    public Vector2 SnapDirection(Vector2 direction)
    {
        // This Code allows the Last Input direction to be animated

        // Check if the x component of the direction is greater in magnitude than the y component
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            // Snap to the horizontal axis by setting the y component to 0
            direction.y = 0;

            // Normalize the x component to either 1 or -1 depending on its original sign
            direction.x = Mathf.Sign(direction.x);
        }
        else
        {
            // Snap to the vertical axis by setting the x component to 0
            direction.x = 0;

            // Normalize the y component to either 1 or -1 depending on its original sign
            direction.y = Mathf.Sign(direction.y);
        }

        // Return the modified direction vector, now snapped to either horizontal or vertical
        return direction;
    }

    public void SetAnimDir(Vector2 direction)
    {
        HeadAnimator.SetFloat("Horizontal", direction.x);
        HeadAnimator.SetFloat("Vertical", direction.y);

        BodyAnimator.SetFloat("Horizontal", direction.x);
        BodyAnimator.SetFloat("Vertical", direction.y);

        ChestAnimator.SetFloat("Horizontal", direction.x);
        ChestAnimator.SetFloat("Vertical", direction.y);

        LegsAnimator.SetFloat("Horizontal", direction.x);
        LegsAnimator.SetFloat("Vertical", direction.y);

        SwordAnimator.SetFloat("Horizontal", direction.x);
        SwordAnimator.SetFloat("Vertical", direction.y);
    }

    #endregion

    #region Slide

    public void StartSlide()
    {
        IsSliding = true;
    }

    public IEnumerator SlideDuration(Vector2 aimDirection, float slideForce, float slideDuration)
    {
        float elapsed = 0f;
        Vector2 startVelocity = aimDirection * slideForce;

        while (elapsed < slideDuration)
        {
            float t = elapsed / slideDuration;
            NpcRB.linearVelocity = Vector2.Lerp(startVelocity, Vector2.zero, t);

            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        NpcRB.linearVelocity = Vector2.zero;
        IsSliding = false;
    }

    #endregion
}
