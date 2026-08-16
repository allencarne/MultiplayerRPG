using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerStateMachine : NetworkBehaviour
{
    [Header("States")]
    private PlayerState state;

    [Header("Skills")]
    [HideInInspector] public PlayerSkill CurrentSkill;
    [SerializeField] SetSkillPanel setSkills;
    [HideInInspector] public SkillPanel skills;
    public SkillBarCoolDowns coolDownTracker;

    [Header("Animators")]
    public Animator PlayerHeadAnimator;
    public Animator BodyAnimator;
    public Animator ChestAnimator;
    public Animator LegsAnimator;
    public Animator WeaponAnimator;

    [Header("Scrips")]
    public Player player;
    public PlayerStats Stats;
    public PlayerHead playerHead;
    public PlayerCustomization customization;
    public PlayerInputHandler Input;
    public PlayerEquipment Equipment;

    [Header("UI")]
    public EnduranceBar EnduranceBar;
    public CastBar CastBar;

    [Header("Components")]
    public Collider2D Collider;
    public Rigidbody2D PlayerRB;
    public Transform Aimer;
    public PlayerInput playerInput;

    [Header("Status Effects")]
    public CrowdControl CrowdControl;
    public Buffs Buffs;
    public DeBuffs DeBuffs;
    public Mobility Mobility;

    [Header("Variables")]
    [HideInInspector] public Vector2 LastMoveDirection = Vector2.zero;
    [HideInInspector] public bool CanRoll = true;
    public bool IsFullySpawned = false;
    public bool IsAttacking = false;
    public bool CanBasic = true;
    public bool CanOffensive = true;
    public bool CanMobility = true;
    public bool CanDefensive = true;
    public bool CanUtility = true;
    public bool CanUltimate = true;

    [Header("Indicator")]
    string indicatorType = null;
    GameObject indicator;

    [HideInInspector] public UnityEvent OnSpawn;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        SetState(new PlayerSpawnState(this));
    }

    public void SetState(PlayerState newState)
    {
        state?.ExitState();
        state = newState;
        state.EnterState();
    }

    public void SkillsOnSpawn()
    {
        setSkills.SetClassSkills();
        if (skills == null) return;

        switch (Stats.playerClass)
        {
            case PlayerStats.PlayerClass.Beginner:

                if (player.FirstPassiveIndex > -1 && player.FirstPassiveIndex <= skills.firstPassive.Length)
                {
                    if (Stats.PlayerLevel.Value >= skills.passive1Req)
                    {
                        setSkills.begginerSkills.FirstPassiveButton(player.FirstPassiveIndex);
                    }
                }

                if (player.SecondPassiveIndex > -1 && player.SecondPassiveIndex <= skills.secondPassive.Length)
                {
                    if (Stats.PlayerLevel.Value >= skills.passive2Req)
                    {
                        setSkills.begginerSkills.SecondPassiveButton(player.FirstPassiveIndex);
                    }
                }

                if (player.ThirdPassiveIndex > -1 && player.ThirdPassiveIndex <= skills.thirdPassive.Length)
                {
                    if (Stats.PlayerLevel.Value >= skills.passive3Req)
                    {
                        setSkills.begginerSkills.ThirdPassiveButton(player.FirstPassiveIndex);
                    }
                }

                if (player.BasicIndex > -1 && player.BasicIndex <= skills.basicAbilities.Length)
                {
                    if (Stats.PlayerLevel.Value >= skills.basicReq)
                    {
                        setSkills.begginerSkills.BasicButton(player.BasicIndex);
                    }
                    
                }

                if (player.OffensiveIndex > -1 && player.OffensiveIndex <= skills.offensiveAbilities.Length)
                {
                    if (Stats.PlayerLevel.Value >= skills.offensiveReq)
                    {
                        setSkills.begginerSkills.OffensiveButton(player.OffensiveIndex);
                    }
                }

                if (player.MobilityIndex > -1 && player.MobilityIndex <= skills.mobilityAbilities.Length)
                {
                    if (Stats.PlayerLevel.Value >= skills.mobilityReq)
                    {
                        setSkills.begginerSkills.MobilityButton(player.MobilityIndex);
                    }
                    
                }

                if (player.DefensiveIndex > -1 && player.DefensiveIndex <= skills.defensiveAbilities.Length)
                {
                    if (Stats.PlayerLevel.Value >= skills.defensiveReq)
                    {
                        setSkills.begginerSkills.DefensiveButton(player.DefensiveIndex);
                    }
                    
                }

                if (player.UtilityIndex > -1 && player.UtilityIndex <= skills.utilityAbilities.Length)
                {
                    if (Stats.PlayerLevel.Value >= skills.utilityReq)
                    {
                        setSkills.begginerSkills.UtilityButton(player.UtilityIndex);
                    }
                    
                }

                if (player.UltimateIndex > -1 && player.UltimateIndex <= skills.ultimateAbilities.Length)
                {
                    if (Stats.PlayerLevel.Value >= skills.ultimateReq)
                    {
                        setSkills.begginerSkills.UltimateButton(player.UltimateIndex);
                    }
                }

                break;
            case PlayerStats.PlayerClass.Warrior:
                break;
            case PlayerStats.PlayerClass.Magician:
                break;
            case PlayerStats.PlayerClass.Archer:
                break;
            case PlayerStats.PlayerClass.Rogue:
                break;
        }
    }

    private void Update()
    {
        if (!IsSpawned) return;
        if (!IsFullySpawned) return;
        if (skills == null || player == null) return;

        state.UpdateState();

        if (player.FirstPassiveIndex > -1 && player.FirstPassiveIndex <= skills.firstPassive.Length)
        {
            //skills.firstPassive[player.FirstPassiveIndex].UpdateSkill(this);
        }

        if (player.SecondPassiveIndex > -1 && player.SecondPassiveIndex <= skills.secondPassive.Length)
        {
            //skills.secondPassive[player.SecondPassiveIndex].UpdateSkill(this);
        }

        if (player.ThirdPassiveIndex > -1 && player.ThirdPassiveIndex <= skills.thirdPassive.Length)
        {
            //skills.thirdPassive[player.ThirdPassiveIndex].UpdateSkill(this);
        }
    }

    private void FixedUpdate()
    {
        if (!IsSpawned) return;
        if (!IsFullySpawned) return;
        if (skills == null || player == null) return;

        state.FixedUpdateState();

        if (player.FirstPassiveIndex > -1 && player.FirstPassiveIndex <= skills.firstPassive.Length)
        {
            //skills.firstPassive[player.FirstPassiveIndex].FixedUpdateSkill(this);
        }

        if (player.SecondPassiveIndex > -1 && player.SecondPassiveIndex <= skills.secondPassive.Length)
        {
            //skills.secondPassive[player.SecondPassiveIndex].FixedUpdateSkill(this);
        }

        if (player.ThirdPassiveIndex > -1 && player.ThirdPassiveIndex <= skills.thirdPassive.Length)
        {
            //skills.thirdPassive[player.ThirdPassiveIndex].FixedUpdateSkill(this);
        }
    }

    public void Interrupt()
    {
        if (Stats.isDead) return;
        if (CurrentSkill == null) return;
        if (CurrentSkill.currentState != PlayerSkill.State.Cast) return;

        Stats.OnInterrupted?.Invoke();

        player.CastBar.StartInterrupt();
        CurrentSkill.DoneState(false, this);
    }

    public void Stagger()
    {
        if (Stats.isDead) return;

        player.CastBar.StartInterrupt();

        if (CurrentSkill != null)
        {
            CurrentSkill.DoneState(true, this);
        }
        else
        {
            SetState(new PlayerStaggerState(this));
        }
    }

    public void Roll()
    {
        if (CrowdControl.stun.IsStunned) return;
        if (!CanRoll) return;

        if (Input.RollInput)
        {
            if (Stats.Endurance.Value >= 50)
            {
                SetState(new PlayerRollState(this));
            }
        }
    }

    public void BasicAbility()
    {
        if (!IsFullySpawned) return;
        if (!CanBasic) return;
        if (IsAttacking) return;
        if (!Equipment.IsWeaponEquipped) return;
        if (player.BasicIndex < 0) return;
        if (player.BasicIndex >= skills.basicAbilities.Length) return;
        if (CrowdControl.disarm.IsDisarmed) return;

        if (Input.BasicAbilityInput)
        {
            IsAttacking = true;
            CanBasic = false;

            DestroyAllIndicators();
            PlayerSkill skill = new PlayerSkill(skills.basicAbilities[player.BasicIndex], player.BasicIndex);
            SetState(new PlayerAttackState(this, skill));
        }
    }

    public void OffensiveAbility()
    {
        if (!IsFullySpawned) return;
        if (!CanOffensive) return;
        if (IsAttacking) return;
        if (!Equipment.IsWeaponEquipped) return;
        if (player.OffensiveIndex < 0) return;
        if (player.OffensiveIndex >= skills.offensiveAbilities.Length) return;
        if (CrowdControl.silence.IsSilenced) return;

        if (Input.IsOffensiveHeld)
        {
            InstantiateIndicator(skills.offensiveAbilities[player.OffensiveIndex].IndicatorPrefab, "Offensive");
        }
        else
        {
            DestroyIndicator("Offensive");
        }

        if (!Input.HasBufferedOffensiveInput) return;

        IsAttacking = true;
        CanOffensive = false;

        DestroyAllIndicators();
        Input.HasBufferedOffensiveInput = false;

        PlayerSkill skill = new PlayerSkill(skills.offensiveAbilities[player.OffensiveIndex], player.OffensiveIndex);
        SetState(new PlayerAttackState(this, skill));
    }

    public void MobilityAbility()
    {
        if (!IsFullySpawned) return;
        if (!CanMobility) return;
        if (IsAttacking) return;
        if (!Equipment.IsWeaponEquipped) return;
        if (player.MobilityIndex < 0) return;
        if (player.MobilityIndex >= skills.mobilityAbilities.Length) return;
        if (CrowdControl.silence.IsSilenced) return;

        if (Input.IsMobilityHeld)
        {
            InstantiateIndicator(skills.mobilityAbilities[player.MobilityIndex].IndicatorPrefab, "Mobility");
        }
        else
        {
            DestroyIndicator("Mobility");
        }

        if (!Input.HasBufferedMobilityInput) return;

        IsAttacking = true;
        CanMobility = false;

        DestroyAllIndicators();
        Input.HasBufferedMobilityInput = false;

        PlayerSkill skill = new PlayerSkill(skills.mobilityAbilities[player.MobilityIndex], player.MobilityIndex);
        SetState(new PlayerAttackState(this, skill));
    }

    public void DefensiveAbility()
    {
        if (!IsFullySpawned) return;
        if (!CanDefensive) return;
        if (IsAttacking) return;
        if (!Equipment.IsWeaponEquipped) return;
        if (player.DefensiveIndex < 0) return;
        if (player.DefensiveIndex >= skills.defensiveAbilities.Length) return;
        if (CrowdControl.silence.IsSilenced) return;

        if (Input.IsDefensiveHeld)
        {
            InstantiateIndicator(skills.defensiveAbilities[player.DefensiveIndex].IndicatorPrefab, "Defensive");
        }
        else
        {
            DestroyIndicator("Defensive");
        }

        if (!Input.HasBufferedDefensiveInput) return;

        IsAttacking = true;
        CanDefensive = false;

        DestroyAllIndicators();
        Input.HasBufferedDefensiveInput = false;

        PlayerSkill skill = new PlayerSkill(skills.defensiveAbilities[player.DefensiveIndex], player.DefensiveIndex);
        SetState(new PlayerAttackState(this, skill));
    }

    public void UtilityAbility()
    {
        if (!IsFullySpawned) return;
        if (!CanUtility) return;
        if (IsAttacking) return;
        if (!Equipment.IsWeaponEquipped) return;
        if (player.UtilityIndex < 0) return;
        if (player.UtilityIndex >= skills.utilityAbilities.Length) return;
        if (CrowdControl.silence.IsSilenced) return;

        if (Input.IsUtilityHeld)
        {
            InstantiateIndicator(skills.utilityAbilities[player.UtilityIndex].IndicatorPrefab, "Utility");
        }
        else
        {
            DestroyIndicator("Utility");
        }

        if (!Input.HasBufferedUtilityInput) return;

        IsAttacking = true;
        CanUtility = false;

        DestroyAllIndicators();
        Input.HasBufferedUtilityInput = false;

        PlayerSkill skill = new PlayerSkill(skills.utilityAbilities[player.UtilityIndex], player.UtilityIndex);
        SetState(new PlayerAttackState(this, skill));
    }

    public void UltimateAbility()
    {
        if (!IsFullySpawned) return;
        if (!CanUltimate) return;
        if (IsAttacking) return;
        if (!Equipment.IsWeaponEquipped) return;
        if (player.UltimateIndex < 0) return;
        if (player.UltimateIndex >= skills.ultimateAbilities.Length) return;
        if (CrowdControl.silence.IsSilenced) return;

        if (Input.IsUltimateHeld)
        {
            InstantiateIndicator(skills.ultimateAbilities[player.UltimateIndex].IndicatorPrefab, "Ultimate");
        }
        else
        {
            DestroyIndicator("Ultimate");
        }

        if (!Input.HasBufferedUltimateInput) return;

        IsAttacking = true;
        CanUltimate = false;

        DestroyAllIndicators();
        Input.HasBufferedUltimateInput = false;

        PlayerSkill skill = new PlayerSkill(skills.ultimateAbilities[player.UltimateIndex], player.UltimateIndex);
        SetState(new PlayerAttackState(this, skill));
    }

    [ServerRpc]
    public void RequestSetColliderServerRpc(bool isEnabled)
    {
        Collider.enabled = isEnabled;
        ApplyColliderClientRpc(isEnabled);
    }

    [ClientRpc]
    void ApplyColliderClientRpc(bool isEnabled)
    {
        Collider.enabled = isEnabled;
    }

    [ServerRpc]
    public void RequestRespawnServerRpc()
    {
        Stats.isDead = false;
        Stats.GiveHeal(100, HealType.Percentage);
    }

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
        PlayerHeadAnimator.SetFloat("Horizontal", direction.x);
        PlayerHeadAnimator.SetFloat("Vertical", direction.y);

        BodyAnimator.SetFloat("Horizontal", direction.x);
        BodyAnimator.SetFloat("Vertical", direction.y);

        ChestAnimator.SetFloat("Horizontal", direction.x);
        ChestAnimator.SetFloat("Vertical", direction.y);

        LegsAnimator.SetFloat("Horizontal", direction.x);
        LegsAnimator.SetFloat("Vertical", direction.y);

        WeaponAnimator.SetFloat("Horizontal", direction.x);
        WeaponAnimator.SetFloat("Vertical", direction.y);

        customization.net_FacingDirection.Value = direction;
        playerHead.SetHead(direction);
    }

    #endregion

    #region Indicators

    void InstantiateIndicator(GameObject prefab, string type)
    {
        if (indicator != null && indicatorType != type)
        {
            Destroy(indicator);
            indicator = null;
        }

        if (indicator == null)
        {
            indicator = Instantiate(prefab, transform.position, Aimer.rotation, transform);
            indicatorType = type;
        }
        else
        {
            indicator.transform.rotation = Aimer.rotation;
        }
    }

    void DestroyIndicator(string type)
    {
        if (indicator != null && indicatorType == type)
        {
            Destroy(indicator);
            indicator = null;
            indicatorType = null;
        }
    }

    void DestroyAllIndicators()
    {
        DestroyIndicator("Offensive");
        DestroyIndicator("Mobility");
        DestroyIndicator("Defensive");
        DestroyIndicator("Utility");
        DestroyIndicator("Ultimate");
    }

    #endregion

    public void RequestAttack(SkillContext context, SpawnEffect effect)
    {
        if (IsServer)
        {
            Attack(context, effect);
        }
        else
        {
            AttackServerRpc(context);
        }
    }

    public void Attack(SkillContext context, SpawnEffect effect)
    {
        NetworkObject attacker = NetworkManager.Singleton.ConnectedClients[context.AttackerId].PlayerObject;

        GameObject attackInstance = Instantiate(effect.Prefab, context.SpawnPosition + context.AimOffset, context.AimRotation);
        NetworkObject attackNetObj = attackInstance.GetComponent<NetworkObject>();
        attackNetObj.Spawn();

        Rigidbody2D attackRB = attackInstance.GetComponent<Rigidbody2D>();
        if (attackRB != null)
        {
            attackRB.AddForce(context.AimDirection * effect.Force, ForceMode2D.Impulse);
        }

        SkillEffectRelay relay = attackInstance.GetComponent<SkillEffectRelay>();
        if (relay != null) relay.Initialize(this, context, effect.OnTriggerEffects, effect.IgnorePlayer, effect.IgnoreEnemy, effect.IgnoreNPC);

        /*
        DamageOnTrigger damageOnTrigger = attackInstance.GetComponent<DamageOnTrigger>();
        if (damageOnTrigger != null)
        {
            damageOnTrigger.CanGenerateFury = context.IsBasic;
            damageOnTrigger.attacker = attacker;
            damageOnTrigger.AbilityDamage = context.AttackerDamage + effect.Damage;
            damageOnTrigger.IgnorePlayer = true;
            damageOnTrigger.IgnoreNPC = true;


            if (CurrentSkill.skillData.HealAmount > 0)
            {
                damageOnTrigger.HealAmount = skillData.HealAmount;
                damageOnTrigger.CanHeal = true;
            }
        }

        InterruptOnTrigger interruptOnTrigger = attackInstance.GetComponent<InterruptOnTrigger>();
        if (interruptOnTrigger != null)
        {
            interruptOnTrigger.attacker = attacker;
            interruptOnTrigger.IgnorePlayer = true;
            interruptOnTrigger.IgnoreNPC = true;
        }

        KnockbackOnTrigger knockbackOnTrigger = attackInstance.GetComponent<KnockbackOnTrigger>();
        if (knockbackOnTrigger != null)
        {
            knockbackOnTrigger.attacker = attacker;
            knockbackOnTrigger.Amount = skillData.KnockBackForce;
            knockbackOnTrigger.Duration = skillData.KnockBackDuration;
            knockbackOnTrigger.Direction = context.AimDirection.normalized;
            knockbackOnTrigger.IgnorePlayer = true;
            knockbackOnTrigger.IgnoreNPC = true;
        }

        StunOnTrigger stunOnTrigger = attackInstance.GetComponent<StunOnTrigger>();
        if (stunOnTrigger != null)
        {
            stunOnTrigger.attacker = attacker;
            stunOnTrigger.Duration = skillData.StunDuration;
            stunOnTrigger.IgnorePlayer = true;
            stunOnTrigger.IgnoreNPC = true;
        }

        SlowOnTrigger slow = attackInstance.GetComponent<SlowOnTrigger>();
        if (slow != null)
        {
            slow.attacker = attacker;
            slow.Duration = skillData.SlowDuration;
            slow.Stacks = skillData.SlowStacks;
            slow.IgnorePlayer = true;
            slow.IgnoreNPC = true;
        }
        */

        FollowTarget target = attackInstance.GetComponent<FollowTarget>();
        if (target != null) target.Target = transform;

        DestroyOnDeath death = attackInstance.GetComponent<DestroyOnDeath>();
        if (death != null) death.stats = GetComponentInParent<CharacterStats>();

        DespawnDelay despawnDelay = attackInstance.GetComponent<DespawnDelay>();
        if (despawnDelay != null) despawnDelay.StartCoroutine(despawnDelay.DespawnAfterDuration(effect.Duration));
    }

    
    [ServerRpc]
    public void AttackServerRpc(SkillContext context)
    {
        SkillData data = ResolveSkillData(context.SkillType, context.SkillIndex);
        SkillEffect[] phaseEffects = data.GetEffects(context.Phase); // small switch, see below
        SpawnEffect effect = (SpawnEffect)phaseEffects[context.EffectIndex];
        Attack(context, effect);
    }

    SkillData ResolveSkillData(SkillData.SkillType type, int index) => type switch
    {
        SkillData.SkillType.Basic => skills.basicAbilities[index],
        SkillData.SkillType.Offensive => skills.offensiveAbilities[index],
        SkillData.SkillType.Mobility => skills.mobilityAbilities[index],
        SkillData.SkillType.Defensive => skills.defensiveAbilities[index],
        SkillData.SkillType.Utility => skills.utilityAbilities[index],
        SkillData.SkillType.Ultimate => skills.ultimateAbilities[index],
        _ => null
    };

    public void Telegraph(float time, bool useOffset, bool useRotation)
    {
        if (CurrentSkill.skillData.TelegraphPrefab == null) return;

        Vector2 position = useOffset ? CurrentSkill.context.SpawnPosition + CurrentSkill.context.AimOffset : CurrentSkill.context.SpawnPosition;
        Quaternion rotation = useRotation ? CurrentSkill.context.AimRotation : Quaternion.identity;

        GameObject attackInstance = Instantiate(CurrentSkill.skillData.TelegraphPrefab, position, rotation);
        NetworkObject attackNetObj = attackInstance.GetComponent<NetworkObject>();
        attackNetObj.Spawn();

        CircleTelegraph circle = attackInstance.GetComponent<CircleTelegraph>();
        if (circle != null)
        {
            circle.stats = gameObject.GetComponentInParent<CharacterStats>();
            circle.Init();

            circle.FillSpeed = time;
        }

        SquareTelegraph square = attackInstance.GetComponent<SquareTelegraph>();
        if (square != null)
        {
            square.stats = gameObject.GetComponentInParent<CharacterStats>();
            square.Init();

            square.FillSpeed = time;
        }
    }
}