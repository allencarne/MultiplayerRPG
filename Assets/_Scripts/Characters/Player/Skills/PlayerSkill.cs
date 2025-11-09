using System.Collections;
using Unity.Netcode;
using UnityEngine;

public abstract class PlayerSkill : NetworkBehaviour
{
    public enum State { Cast, Action, Impact, Recovery, Done }
    [HideInInspector] public State currentState;
    public enum SkillType { Basic, Offensive, Mobility, Defensive, Utility, Ultimate }
    public SkillType skillType;
    public enum WeaponType { Sword, Staff, Bow, Dagger }
    public WeaponType weaponType;

    [Header("UI")]
    public Sprite SkillIcon;
    public GameObject IndicatorPrefab;
    [TextArea] public string Description;

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
    public float CoolDown;

    [Header("Knockback")]
    [SerializeField] protected float KnockBackForce;
    [SerializeField] protected float KnockBackDuration;

    [Header("Stun")]
    [SerializeField] protected float stunDuration;

    [Header("Slow")]
    [SerializeField] protected int SlowStacks;
    [SerializeField] protected float SlowDuration;

    [Header("Slow")]
    [SerializeField] protected int SlideForce;
    [SerializeField] protected float SlideDuration;

    [Header("StateTimer")]
    [HideInInspector] protected float StateTimer;
    [HideInInspector] protected float ModifiedCastTime;

    [Header("Aim")]
    [HideInInspector] protected Vector2 SpawnPosition;
    [HideInInspector] protected Vector2 AimDirection;
    [HideInInspector] protected Vector2 AimOffset;
    [HideInInspector] protected Quaternion AimRotation;
    [HideInInspector] protected int AttackerDamage;
    bool isBasic = false;

    public virtual void StartSkill(PlayerStateMachine owner)
    {

    }
    public virtual void UpdateSkill(PlayerStateMachine owner)
    {
        if (currentState == State.Done) return;

        StateTimer -= Time.deltaTime;
        if (StateTimer <= 0f) StateTransition(owner);
    }
    public virtual void FixedUpdateSkill(PlayerStateMachine owner)
    {

    }

    public virtual void CastState(PlayerStateMachine owner)
    {

    }
    public virtual void ActionState(PlayerStateMachine owner)
    {

    }
    public virtual void ImpactState(PlayerStateMachine owner)
    {

    }
    public virtual void RecoveryState(PlayerStateMachine owner)
    {

    }

    protected void StateTransition(PlayerStateMachine owner, bool hasAction = false)
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
    public void DoneState(bool isStaggered, PlayerStateMachine owner)
    {
        currentState = State.Done;
        owner.IsAttacking = false;
        owner.CurrentSkill = null;

        if (isStaggered)
        {
            owner.SetState(PlayerStateMachine.State.Stagger);

        }
        else
        {
            owner.SetState(PlayerStateMachine.State.Idle);
        }

    }

    protected void InitializeAbility(SkillType skilltype, PlayerStateMachine owner)
    {
        switch (skilltype)
        {
            case SkillType.Basic: owner.CanBasic = false; isBasic = true; break;
            case SkillType.Offensive: owner.CanOffensive = false; break;
            case SkillType.Mobility: owner.CanMobility = false; break;
            case SkillType.Defensive: owner.CanDefensive = false; break;
            case SkillType.Utility: owner.CanUtility = false; break;
            case SkillType.Ultimate: owner.CanUltimate = false; break;
        }
        owner.IsAttacking = true;
        owner.CurrentSkill = this;

        AttackerDamage = owner.player.CurrentDamage.Value;

        owner.PlayerRB.linearVelocity = Vector2.zero;
        SpawnPosition = owner.transform.position;

        StartCoroutine(CoolDownn(skillType, CoolDown, owner));
    }
    IEnumerator CoolDownn(SkillType type, float coolDown, PlayerStateMachine owner)
    {
        float modifiedCooldown = coolDown / owner.player.CurrentCDR.Value;
        owner.coolDownTracker.SkillCoolDown(skillType, modifiedCooldown);

        yield return new WaitForSeconds(modifiedCooldown);

        switch (type)
        {
            case SkillType.Basic: owner.CanBasic = true; break;
            case SkillType.Offensive: owner.CanOffensive = true; break;
            case SkillType.Mobility: owner.CanMobility = true; break;
            case SkillType.Defensive: owner.CanDefensive = true; break;
            case SkillType.Utility: owner.CanUtility = true; break;
            case SkillType.Ultimate: owner.CanUltimate = true; break;
        }
    }
    protected void Animate(PlayerStateMachine owner, WeaponType weapon, SkillType type, State state)
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

        switch (type)
        {
            case SkillType.Basic: _skill = "Basic"; break;
            case SkillType.Offensive: _skill = "Offensive"; break;
            case SkillType.Mobility: _skill = "Mobility"; break;
            case SkillType.Defensive: _skill = "Defensive"; break;
            case SkillType.Utility: _skill = "Utility"; break;
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
        owner.HairAnimator.Play(_weapon + " " + _skill + " " + _state + " " + owner.player.hairIndex);
        owner.EyesAnimator.Play(_weapon + " " + _skill + " " + _state);
        owner.SwordAnimator.Play(_weapon + " " + _skill + " " + _state);

        owner.HeadAnimator.Play(_weapon + " " + _skill + " " + _state + " " + owner.Equipment.HeadAnimIndex);
        owner.ChestAnimator.Play(_weapon + " " + _skill + " " + _state + " " + owner.Equipment.ChestAnimIndex);
        owner.LegsAnimator.Play(_weapon + " " + _skill + " " + _state + " " + owner.Equipment.LegsAnimIndex);
    }

    protected void Telegraph(float time, bool useOffset, bool useRotation)
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
            circle.FillSpeed = time;
            circle.crowdControl = gameObject.GetComponentInParent<CrowdControl>();
            circle.player = gameObject.GetComponentInParent<Player>();
        }

        SquareTelegraph square = attackInstance.GetComponent<SquareTelegraph>();
        if (square != null)
        {
            square.FillSpeed = time;
            square.crowdControl = gameObject.GetComponentInParent<CrowdControl>();
            square.player = gameObject.GetComponentInParent<Player>();
        }
    }
    protected void Attack(ulong attackerID)
    {
        NetworkObject attacker = NetworkManager.Singleton.ConnectedClients[attackerID].PlayerObject;
        //NetworkObject attacker = GetComponentInParent<NetworkObject>();

        GameObject attackInstance = Instantiate(SkillPrefab, SpawnPosition + AimOffset, AimRotation);
        NetworkObject attackNetObj = attackInstance.GetComponent<NetworkObject>();
        attackNetObj.Spawn();

        Rigidbody2D attackRB = attackInstance.GetComponent<Rigidbody2D>();
        if (attackRB != null)
        {
            attackRB.AddForce(AimDirection * SkillForce, ForceMode2D.Impulse);
        }

        DamageOnTrigger damageOnTrigger = attackInstance.GetComponent<DamageOnTrigger>();
        if (damageOnTrigger != null)
        {
            damageOnTrigger.IsBasic = isBasic;
            damageOnTrigger.attacker = attacker;
            damageOnTrigger.AbilityDamage = AttackerDamage + SkillDamage;
            damageOnTrigger.IgnorePlayer = true;
            damageOnTrigger.IgnoreNPC = true;
        }

        KnockbackOnTrigger knockbackOnTrigger = attackInstance.GetComponent<KnockbackOnTrigger>();
        if (knockbackOnTrigger != null)
        {
            knockbackOnTrigger.attacker = attacker;
            knockbackOnTrigger.Amount = KnockBackForce;
            knockbackOnTrigger.Duration = KnockBackDuration;
            knockbackOnTrigger.Direction = AimDirection.normalized;
            knockbackOnTrigger.IgnorePlayer = true;
            knockbackOnTrigger.IgnoreNPC = true;
        }

        StunOnTrigger stunOnTrigger = attackInstance.GetComponent<StunOnTrigger>();
        if (stunOnTrigger != null)
        {
            stunOnTrigger.attacker = attacker;
            stunOnTrigger.Duration = stunDuration;
            stunOnTrigger.IgnorePlayer = true;
            stunOnTrigger.IgnoreNPC = true;
        }

        SlowOnTrigger slow = attackInstance.GetComponent<SlowOnTrigger>();
        if (slow != null)
        {
            slow.attacker = attacker;
            slow.Duration = SlowDuration;
            slow.Stacks = SlowStacks;
            slow.IgnorePlayer = true;
            slow.IgnoreNPC = true;
        }

        DestroyOnDeath death = attackInstance.GetComponent<DestroyOnDeath>();
        if (death != null) death.player = GetComponentInParent<Player>();

        DespawnDelay despawnDelay = attackInstance.GetComponent<DespawnDelay>();
        if (despawnDelay != null) despawnDelay.StartCoroutine(despawnDelay.DespawnAfterDuration(SkillDuration));
    }

    [ServerRpc]
    public void AttackServerRpc(Vector2 spawnPosition, Vector2 aimOffset, Vector2 aimDirection, Quaternion aimRotation, int damage, ulong attackerID)
    {
        SpawnPosition = spawnPosition;
        AimOffset = aimOffset;
        AimDirection = aimDirection;
        AimRotation = aimRotation;
        AttackerDamage = damage;
        Attack(attackerID);
    }
}
