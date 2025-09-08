using System.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
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

    [Header("Animators")]
    public Animator BodyAnimator;
    public Animator EyesAnimator;
    public Animator HairAnimator;
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
    Coroutine CurrentAttack;

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
        if (npc.IsDead) return;

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
            case State.Basic: state = State.Basic; basicState.StartState(this); break;
            case State.Special: state = State.Special; specialState.StartState(this); break;
            case State.Ultimate: state = State.Ultimate; ultimateState.StartState(this); break;
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

    public void HandlePotentialInterrupt()
    {
        if (!CrowdControl.IsInterrupted) return;
        if (npc.CastBar.castBarFill.color != Color.black) return;

        if (IsServer)
        {
            npc.CastBar.InterruptCastBar();
        }
        else
        {
            npc.CastBar.InterruptServerRpc();
        }

        if (CurrentAttack != null)
        {
            StopCoroutine(CurrentAttack);
            CurrentAttack = null;
        }

        IsAttacking = false;
        return;
    }

    public void MoveTowardsTarget(Vector2 _targetPos)
    {
        if (CrowdControl.immobilize.IsImmobilized) return;
        Vector2 direction = GetDirectionAroundObstacle(_targetPos);
        NpcRB.linearVelocity = direction * npc.CurrentSpeed;
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
            if (other.GetComponent<Enemy>().isDummy) return;

            Target = other.transform;
            IsEnemyInRange = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(StartingPosition, DeAggroRadius);
    }

    [ServerRpc]
    public void RequestDisableColliderServerRpc()
    {
        Collider.enabled = false;
        npc.SwordSprite.enabled = false;
        npc.EyeSprite.enabled = false;
        npc.HairSprite.enabled = false;
        ApplyColliderStateClientRpc(false);
    }

    [ServerRpc]
    public void RequestEnableColliderServerRpc()
    {
        Collider.enabled = true;
        npc.SwordSprite.enabled = true;
        npc.EyeSprite.enabled = true;
        npc.HairSprite.enabled = true;
        ApplyColliderStateClientRpc(true);
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
        npc.GiveHeal(100, HealType.Percentage);
    }

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
        SwordAnimator.SetFloat("Horizontal", direction.x);
        SwordAnimator.SetFloat("Vertical", direction.y);

        BodyAnimator.SetFloat("Horizontal", direction.x);
        BodyAnimator.SetFloat("Vertical", direction.y);

        EyesAnimator.SetFloat("Horizontal", direction.x);
        EyesAnimator.SetFloat("Vertical", direction.y);

        HairAnimator.SetFloat("Horizontal", direction.x);
        HairAnimator.SetFloat("Vertical", direction.y);
    }

    public void AnimateCast(Vector2 snappedDirection)
    {
        BodyAnimator.SetFloat("Horizontal", snappedDirection.x);
        BodyAnimator.SetFloat("Vertical", snappedDirection.y);
        BodyAnimator.Play("Sword_Attack_C");

        SwordAnimator.SetFloat("Horizontal", snappedDirection.x);
        SwordAnimator.SetFloat("Vertical", snappedDirection.y);
        SwordAnimator.Play("Sword_Attack_C");

        EyesAnimator.SetFloat("Horizontal", snappedDirection.x);
        EyesAnimator.SetFloat("Vertical", snappedDirection.y);
        EyesAnimator.Play("Sword_Attack_C");

        HairAnimator.SetFloat("Horizontal", snappedDirection.x);
        HairAnimator.SetFloat("Vertical", snappedDirection.y);
        HairAnimator.Play("Sword_Attack_C_" + npc.hairIndex);
    }

    public void StartCastBar(float castTime)
    {
        if (IsServer)
        {
            npc.CastBar.StartCast(castTime, npc.CurrentAttackSpeed);
        }
        else
        {
            npc.CastBar.StartCastServerRpc(castTime, npc.CurrentAttackSpeed);
        }
    }

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

    public IEnumerator CoolDownTime(SkillType type, float baseCooldown)
    {
        float modifiedCoolDown = baseCooldown / npc.CurrentCDR;

        yield return new WaitForSeconds(modifiedCoolDown);

        switch (type)
        {
            case SkillType.Basic: CanBasic = true; break;
            case SkillType.Special: CanSpecial = true; break;
            case SkillType.Ultimate: CanUltimate = true; break;
        }
    }

    public void StartCast(float modifiedCastTime, float recoveryTime, NPCState ability)
    {
        CurrentAttack = StartCoroutine(CastTime(modifiedCastTime, recoveryTime, ability));
    }

    IEnumerator CastTime(float modifiedCastTime, float recoveryTime, NPCState ability)
    {
        yield return new WaitForSeconds(modifiedCastTime);

        if (!IsAttacking) yield break;
        if (npc.IsDead) yield break;

        BodyAnimator.Play("Sword_Attack_I");
        SwordAnimator.Play("Sword_Attack_I");
        EyesAnimator.Play("Sword_Attack_I");
        HairAnimator.Play("Sword_Attack_I_" + npc.hairIndex);

        StartCoroutine(ImpactTime(recoveryTime, ability));
    }

    IEnumerator ImpactTime(float recoveryTime, NPCState ability)
    {
        yield return new WaitForSeconds(.1f);

        if (!IsAttacking) yield break;
        if (npc.IsDead) yield break;

        ability.Impact(this);

        BodyAnimator.Play("Sword_Attack_R");
        SwordAnimator.Play("Sword_Attack_R");
        EyesAnimator.Play("Sword_Attack_R");
        HairAnimator.Play("Sword_Attack_R_" + npc.hairIndex);

        // Start Recovery
        if (IsServer)
        {
            npc.CastBar.StartRecovery(recoveryTime, npc.CurrentAttackSpeed);
        }
        else
        {
            npc.CastBar.StartRecoveryServerRpc(recoveryTime, npc.CurrentAttackSpeed);
        }

        StartCoroutine(RecoveryTime(recoveryTime));
    }

    IEnumerator RecoveryTime(float modifiedRecoveryTime)
    {
        yield return new WaitForSeconds(modifiedRecoveryTime);

        if (!IsAttacking) yield break;
        if (npc.IsDead) yield break;

        IsAttacking = false;
        if (state != State.Hurt)
        {
            SetState(State.Idle);
        }
    }
}
