using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class NPCStateMachine : StateMachine
{
    [Header("States")]
    public NPCState state;

    [Header("Skills")]
    [SerializeField] NPCSkill basicSkill;
    [SerializeField] NPCSkill specialSkill;
    [SerializeField] NPCSkill ultimateSkill;
    [HideInInspector] public NPCSkill CurrentSkill;

    [Header("Status Effects")]
    //public CrowdControl CrowdControl;
    //public Buffs Buffs;
    //public DeBuffs DeBuffs;

    [Header("Bools")]
    public bool IsEnemyInRange = false;
    public bool IsAttacking = false;
    public bool isResetting = false;
    public bool IsSliding = false;
    public bool CanBasic = true;
    public bool CanMobility = false;
    public bool CanUltimate = false;

    [Header("Variables")]
    public Vector2 StartingPosition;

    [Header("Radius")]
    public float BasicRadius;
    public float DeAggroRadius;

    [Header("Components")]
    //[SerializeField] Collider2D Collider2D;
    //public Rigidbody2D RigidBody2D;
    public NPC npc;
    public LayerMask obstacleLayerMask;

    [Header("Patrol")]
    public int PatrolIndex = 0;

    public Transform Target;
    public Transform SecondTarget { get; set; }

    [HideInInspector] public UnityEvent OnSpawn;

    public enum SkillType
    {
        Basic,
        Special,
        Ultimate,
    }

    public void Initialize()
    {
        if (!IsServer) return;
        StartingPosition = transform.position;
        SetState(new NPCSpawnState(this));
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

    public void SetState(NPCState newState)
    {
        state?.ExitState();
        state = newState;
        state.EnterState();
    }

    public void SetSkill(SkillType newSkill)
    {
        switch (newSkill)
        {
            case SkillType.Basic: CurrentSkill = basicSkill; break;
            case SkillType.Special: CurrentSkill = specialSkill; break;
            case SkillType.Ultimate: CurrentSkill = ultimateSkill; break;
        }

        SetState(new NPCAttackState(this, CurrentSkill));
    }

    public void Interrupt()
    {
        if (npc.stats.isDead) return;
        if (CurrentSkill == null) return;
        if (CurrentSkill.currentState != ActiveSkillData.SkillPhase.Cast) return;

        npc.stats.OnInterrupted?.Invoke();

        CastBar.StartInterrupt();
        CurrentSkill.DoneState(false, this);
    }

    public void Stagger()
    {
        if (npc.stats.isDead) return;

        CastBar.StartInterrupt();

        if (CurrentSkill != null)
        {
            CurrentSkill.DoneState(true, this);
        }
        else
        {
            SetState(new NPCStaggerState(this));
        }
    }

    public void TransitionToIdle()
    {
        if (npc.Data.npcClass == NPCClass.Patrol)
        {
            SetState(new PatrolIdleState(this));
        }
        else
        {
            SetState(new NPCIdleState(this));
        }
    }

    public void TransitionToChase()
    {
        if (npc.Data.npcClass == NPCClass.Patrol)
        {
            SetState(new PatrolChaseState(this));
        }
        else
        {
            SetState(new NPCChaseState(this));
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (npc.stats.isDead) return;
        if (state is NPCResetState) return;

        if (other.CompareTag("Enemy"))
        {
            if (other.GetComponent<Enemy>().IsDummy) return;

            if (Target == null)
            {
                Target = other.transform;
                IsEnemyInRange = true;
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
                RigidBody2D.linearVelocity = Vector2.zero;
                return;
            }
        }

        Vector2 direction = GetDirectionAroundObstacle(_targetPos);
        RigidBody2D.linearVelocity = direction * npc.stats.TotalSpeed;
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

    #region RPC

    public void SetColliderAndSprites(bool isEnabled)
    {
        if (!IsServer) return;
        Collider2D.enabled = isEnabled;
        ApplyColliderStateClientRpc(isEnabled);
    }

    [ClientRpc]
    void ApplyColliderStateClientRpc(bool isEnabled)
    {
        Collider2D.enabled = isEnabled;
    }

    #endregion

    #region Animation
    /*

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
        if (IsServer) npc.net_FacingDirection.Value = direction;

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
    */
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
            RigidBody2D.linearVelocity = Vector2.Lerp(startVelocity, Vector2.zero, t);

            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        RigidBody2D.linearVelocity = Vector2.zero;
        IsSliding = false;
    }

    #endregion

    protected override ActiveSkillData GetSkillData(ActiveSkillData.SkillType type, int index) => type switch
    {
        ActiveSkillData.SkillType.Basic => basicSkill.skillData,
        ActiveSkillData.SkillType.Offensive => specialSkill.skillData,
        ActiveSkillData.SkillType.Ultimate => ultimateSkill.skillData,
        _ => null
    };
}
