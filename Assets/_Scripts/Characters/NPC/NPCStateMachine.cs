using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class NPCStateMachine : StateMachine
{
    [Header("States")]
    public NPCState state;

    [Header("Skills")]
    [HideInInspector] public ActiveSkill CurrentSkill;
    PassiveSkill passiveInstance;

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


    [Header("Components")]
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
            case SkillType.Basic: CurrentSkill = new ActiveSkill(npc.Data.BasicAbility, 0); break;
            case SkillType.Special: CurrentSkill = new ActiveSkill(npc.Data.SpecialAbility, 0); ; break;
            case SkillType.Ultimate: CurrentSkill = new ActiveSkill(npc.Data.UltimateAbility, 0); ; break;
        }

        SetState(new NPCAttackState(this, CurrentSkill));
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
        Gizmos.DrawWireSphere(StartingPosition, npc.Data.DeAggroRadius);
    }

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

        Vector2 direction = Vector2.zero;
        if (Pathfinding != null)
        {
            direction = Pathfinding.GetDirectionAroundObstacle(transform.position, _targetPos, obstacleLayerMask);
        }
        else
        {
            direction = (_targetPos - (Vector2)transform.position).normalized;
        }

        RigidBody2D.linearVelocity = direction * npc.stats.TotalSpeed;
    }

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

    protected override ActiveSkillData GetSkillData(ActiveSkillData.SkillType type, int index) => type switch
    {
        ActiveSkillData.SkillType.Basic => npc.Data.BasicAbility,
        ActiveSkillData.SkillType.Mobility => npc.Data.SpecialAbility,
        ActiveSkillData.SkillType.Ultimate => npc.Data.UltimateAbility,
        _ => null
    };
}
