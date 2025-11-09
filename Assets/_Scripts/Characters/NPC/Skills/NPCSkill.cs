using System.Collections;
using Unity.Netcode;
using UnityEngine;

public abstract class NPCSkill : NetworkBehaviour
{
    public enum State { Cast, Action, Impact, Recovery, Done }
    [HideInInspector] public State currentState;
    public enum SkillType { Basic, Special, Ultimate }
    public SkillType skillType;
    public enum WeaponType { Sword, Staff, Bow, Dagger }
    public WeaponType weaponType;

    [Header("Prefab")]
    [SerializeField] protected GameObject SkillPrefab;
    [SerializeField] protected GameObject TelegraphPrefab;

    [Header("Stats")]
    [SerializeField] protected int SkillDamage;
    [SerializeField] protected float SkillRange;
    [SerializeField] protected float SkillForce;
    [SerializeField] protected float SkillDuration;

    [Header("Time")]
    [SerializeField] protected float CastTime;
    [SerializeField] protected float ActionTime;
    [SerializeField] protected float ImpactTime;
    [SerializeField] protected float RecoveryTime;

    [Header("CoolDown")]
    [SerializeField] protected float CoolDown;

    [Header("Knockback")]
    [SerializeField] protected float KnockBackForce;
    [SerializeField] protected float KnockBackDuration;

    [Header("Slow")]
    [SerializeField] protected int SlowStacks;
    [SerializeField] protected float SlowDuration;

    [Header("StateTimer")]
    [HideInInspector] protected float StateTimer;
    [HideInInspector] protected float ModifiedCastTime;

    [Header("Aim")]
    [HideInInspector] protected Vector2 SpawnPosition;
    [HideInInspector] protected Vector2 AimDirection;
    [HideInInspector] protected Vector2 AimOffset;
    [HideInInspector] protected Quaternion AimRotation;
    [HideInInspector] protected int AttackerDamage;

    public virtual void StartSkill(NPCStateMachine owner)
    {

    }
    public virtual void UpdateSkill(NPCStateMachine owner)
    {
        if (currentState == State.Done) return;

        StateTimer -= Time.deltaTime;
        if (StateTimer <= 0f) StateTransition(owner);
    }
    public virtual void FixedUpdateSkill(NPCStateMachine owner)
    {

    }

    public virtual void CastState(NPCStateMachine owner)
    {

    }
    public virtual void ActionState(NPCStateMachine owner)
    {

    }
    public virtual void ImpactState(NPCStateMachine owner)
    {

    }
    public virtual void RecoveryState(NPCStateMachine owner)
    {

    }

    protected void StateTransition(NPCStateMachine owner, bool hasAction = false)
    {
        switch (currentState)
        {
            case State.Cast:
                if (hasAction)
                {
                    ActionState(owner);
                    ChangeState(State.Action, ActionTime);
                }
                else
                {
                    ImpactState(owner);
                    ChangeState(State.Impact, ImpactTime);
                }
                break;

            case State.Action:
                ImpactState(owner);
                ChangeState(State.Impact, ImpactTime);
                break;

            case State.Impact:
                RecoveryState(owner);
                ChangeState(State.Recovery, RecoveryTime);
                break;

            case State.Recovery:
                DoneState(false, owner);
                break;
        }
    }
    protected void ChangeState(State next, float duration)
    {
        currentState = next;
        StateTimer = duration;
    }
    public void DoneState(bool isStaggered, NPCStateMachine owner)
    {
        StartCoroutine(CoolDownn(skillType, CoolDown, owner));
        currentState = State.Done;
        owner.IsAttacking = false;
        owner.CurrentSkill = null;

        if (isStaggered)
        {
            owner.SetState(NPCStateMachine.State.Stagger);

        }
        else
        {
            owner.SetState(NPCStateMachine.State.Idle);
        }

    }

    protected void InitializeAbility(SkillType skilltype, NPCStateMachine owner)
    {
        switch (skilltype)
        {
            case SkillType.Basic: owner.CanBasic = false; break;
            case SkillType.Special: owner.CanSpecial = false; break;
            case SkillType.Ultimate: owner.CanUltimate = false; break;
        }
        owner.IsAttacking = true;
        owner.CurrentSkill = this;

        AttackerDamage = owner.npc.CurrentDamage;

        owner.NpcRB.linearVelocity = Vector2.zero;
        SpawnPosition = owner.transform.position;
    }
    IEnumerator CoolDownn(SkillType type, float coolDown, NPCStateMachine owner)
    {
        float modifiedCooldown = coolDown / owner.npc.CurrentCDR;

        yield return new WaitForSeconds(modifiedCooldown);

        switch (type)
        {
            case SkillType.Basic: owner.CanBasic = true; break;
            case SkillType.Special: owner.CanSpecial = true; break;
            case SkillType.Ultimate: owner.CanUltimate = true; break;
        }
    }
    protected void Animate(NPCStateMachine owner, WeaponType weapon, SkillType skill, State state)
    {
        string _weapon = "";
        string _skill = "";
        string _state = "";

        switch (weapon)
        {
            case WeaponType.Sword: _weapon = weapon.ToString(); break;
            case WeaponType.Staff: _weapon = weapon.ToString(); break;
            case WeaponType.Bow: _weapon = weapon.ToString(); break;
            case WeaponType.Dagger: _weapon = weapon.ToString(); break;
        }

        switch (skill)
        {
            case SkillType.Basic: _skill = "Basic"; break;
            case SkillType.Special: _skill = "Special"; break;
            case SkillType.Ultimate: _skill = "Ultimate"; break;
        }

        switch (state)
        {
            case State.Cast: _state = "Cast"; break;
            case State.Action: _state = "Action"; break;
            case State.Impact: _state = "Impact"; break;
            case State.Recovery: _state = "Recovery"; break;
            case State.Done: _state = "Done"; break;
        }

        owner.BodyAnimator.Play(_weapon + " " + _skill + " " + _state);
        owner.HairAnimator.Play(_weapon + " " + _skill + " " + _state + " " + owner.npc.hairIndex);
        owner.EyesAnimator.Play(_weapon + " " + _skill + " " + _state);
        owner.SwordAnimator.Play(_weapon + " " + _skill + " " + _state);

        owner.HeadAnimator.Play(_weapon + " " + _skill + " " + _state + " " + owner.npc.HeadIndex);
        owner.ChestAnimator.Play(_weapon + " " + _skill + " " + _state + " " + owner.npc.ChestIndex);
        owner.LegsAnimator.Play(_weapon + " " + _skill + " " + _state + " " + owner.npc.LegsIndex);
    }

    protected void Telegraph(bool useOffset, bool useRotation)
    {
        if (TelegraphPrefab == null) return; 

        Vector2 position = useOffset ? SpawnPosition + AimOffset : SpawnPosition;
        Quaternion rotation = useRotation ? AimRotation : Quaternion.identity;

        GameObject attackInstance = Instantiate(TelegraphPrefab, position, rotation);
        NetworkObject attackNetObj = attackInstance.GetComponent<NetworkObject>();
        attackNetObj.Spawn();

        CircleTelegraph circle = attackInstance.GetComponent<CircleTelegraph>();
        if (circle != null)
        {
            circle.FillSpeed = ModifiedCastTime + ActionTime;
            circle.crowdControl = gameObject.GetComponentInParent<CrowdControl>();
            circle.npc = gameObject.GetComponentInParent<NPC>();
        }

        SquareTelegraph square = attackInstance.GetComponent<SquareTelegraph>();
        if (square != null)
        {
            square.FillSpeed = ModifiedCastTime + ActionTime;
            square.crowdControl = gameObject.GetComponentInParent<CrowdControl>();
            square.npc = gameObject.GetComponentInParent<NPC>();
        }
    }
    protected void Attack()
    {
        NetworkObject attacker = GetComponentInParent<NetworkObject>();

        GameObject attackInstance = Instantiate(SkillPrefab, SpawnPosition + AimOffset, AimRotation);
        NetworkObject attackNetObj = attackInstance.GetComponent<NetworkObject>();
        attackNetObj.Spawn();

        Debug.Log("NPC Damage " + AttackerDamage);
        Debug.Log("Skill Damage " + SkillDamage);

        Rigidbody2D attackRB = attackInstance.GetComponent<Rigidbody2D>();
        if (attackRB != null)
        {
            attackRB.AddForce(AimDirection * SkillForce, ForceMode2D.Impulse);
        }

        DamageOnTrigger damageOnTrigger = attackInstance.GetComponent<DamageOnTrigger>();
        if (damageOnTrigger != null)
        {
            damageOnTrigger.attacker = attacker;
            damageOnTrigger.AbilityDamage = AttackerDamage + SkillDamage;
            damageOnTrigger.IgnoreNPC = true;
            damageOnTrigger.IgnorePlayer = true;
        }

        KnockbackOnTrigger knockbackOnTrigger = attackInstance.GetComponent<KnockbackOnTrigger>();
        if (knockbackOnTrigger != null)
        {
            knockbackOnTrigger.attacker = attacker;
            knockbackOnTrigger.Amount = KnockBackForce;
            knockbackOnTrigger.Duration = KnockBackDuration;
            knockbackOnTrigger.Direction = AimDirection.normalized;
            knockbackOnTrigger.IgnoreNPC = true;
            knockbackOnTrigger.IgnorePlayer = true;
        }

        SlowOnTrigger slow = attackInstance.GetComponent<SlowOnTrigger>();
        if (slow != null)
        {
            slow.attacker = attacker;
            slow.Duration = SlowDuration;
            slow.Stacks = SlowStacks;
            slow.IgnoreNPC = true;
            slow.IgnorePlayer = true;
        }

        DestroyOnDeath death = attackInstance.GetComponent<DestroyOnDeath>();
        if (death != null) death.npc = GetComponentInParent<NPC>();

        DespawnDelay despawnDelay = attackInstance.GetComponent<DespawnDelay>();
        if (despawnDelay != null) despawnDelay.StartCoroutine(despawnDelay.DespawnAfterDuration(SkillDuration));
    }

    [ServerRpc]
    public void AttackServerRpc(Vector2 spawnPosition, Vector2 aimOffset, Vector2 aimDirection, Quaternion aimRotation, int damage)
    {
        SpawnPosition = spawnPosition;
        AimOffset = aimOffset;
        AimDirection = aimDirection;
        AimRotation = aimRotation;
        AttackerDamage = damage;
        Attack();
    }
}
