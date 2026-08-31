using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerStateMachine : NetworkBehaviour
{
    [Header("States")]
    private PlayerState state;

    [Header("Skills")]
    [HideInInspector] public PlayerSkill CurrentSkill;
    [SerializeField] ClassSkillSelector setSkills;
    [HideInInspector] public ClassSkillSet skills;
    public SkillBarUI[] coolDownTracker;

    PlayerPassive firstPassiveInstance;
    PlayerPassive secondPassiveInstance;
    PlayerPassive thirdPassiveInstance;

    [Header("Animator")]
    public CharacterAnimator Animator;

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
    public Indicator Indicator;

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
        setSkills.RestoreSelections();
    }

    private void Update()
    {
        if (!IsSpawned) return;
        if (!IsFullySpawned) return;
        if (skills == null || player == null) return;

        state.UpdateState();
        firstPassiveInstance?.UpdatePassive(this);
        secondPassiveInstance?.UpdatePassive(this);
        thirdPassiveInstance?.UpdatePassive(this);
    }

    private void FixedUpdate()
    {
        if (!IsSpawned) return;
        if (!IsFullySpawned) return;
        if (skills == null || player == null) return;

        state.FixedUpdateState();
        firstPassiveInstance?.FixedUpdatePassive(this);
        secondPassiveInstance?.FixedUpdatePassive(this);
        thirdPassiveInstance?.FixedUpdatePassive(this);
    }

    public void Interrupt()
    {
        if (Stats.isDead) return;
        if (CurrentSkill == null) return;
        if (CurrentSkill.currentState != ActiveSkillData.SkillPhase.Cast) return;

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
            ActiveSkillData data = skills.basicAbilities[player.BasicIndex];

            StartAbility(data, player.BasicIndex);

            CanBasic = false;
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

        ActiveSkillData data = skills.offensiveAbilities[player.OffensiveIndex];

        Indicator.HandleAbilityIndicator(data, "Offensive", Input.IsOffensiveHeld, Input, playerInput.currentControlScheme);

        if (!Input.HasBufferedOffensiveInput) return;

        StartAbility(data, player.OffensiveIndex);
        CanOffensive = false;
        Input.HasBufferedOffensiveInput = false;
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

        ActiveSkillData data = skills.mobilityAbilities[player.MobilityIndex];

        Indicator.HandleAbilityIndicator(data, "Mobility", Input.IsMobilityHeld, Input, playerInput.currentControlScheme);

        if (!Input.HasBufferedMobilityInput) return;

        StartAbility(data, player.MobilityIndex);
        CanMobility = false;
        Input.HasBufferedMobilityInput = false;
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

        ActiveSkillData data = skills.defensiveAbilities[player.DefensiveIndex];

        Indicator.HandleAbilityIndicator(data, "Defensive", Input.IsDefensiveHeld, Input, playerInput.currentControlScheme);

        if (!Input.HasBufferedDefensiveInput) return;

        StartAbility(data, player.DefensiveIndex);
        CanDefensive = false;
        Input.HasBufferedDefensiveInput = false;
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

        ActiveSkillData data = skills.utilityAbilities[player.UtilityIndex];

        Indicator.HandleAbilityIndicator(data, "Utility", Input.IsUtilityHeld, Input, playerInput.currentControlScheme);

        if (!Input.HasBufferedUtilityInput) return;

        StartAbility(data, player.UtilityIndex);
        CanUtility = false;
        Input.HasBufferedUtilityInput = false;
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

        ActiveSkillData data = skills.ultimateAbilities[player.UltimateIndex];

        Indicator.HandleAbilityIndicator(data, "Ultimate", Input.IsUltimateHeld, Input, playerInput.currentControlScheme);

        if (!Input.HasBufferedUltimateInput) return;

        StartAbility(data, player.UltimateIndex);
        CanUltimate = false;
        Input.HasBufferedUltimateInput = false;
    }

    public void SetFirstPassive(PassiveSkillData data, int index)
    {
        Debug.Log("Test");
        firstPassiveInstance?.EndPassive(this);
        firstPassiveInstance = new PlayerPassive(data, index);
        firstPassiveInstance.StartPassive(this);
    }

    public void SetSecondPassive(PassiveSkillData data, int index)
    {
        secondPassiveInstance?.EndPassive(this);
        secondPassiveInstance = new PlayerPassive(data, index);
        secondPassiveInstance.StartPassive(this);
    }

    public void SetThirdPassive(PassiveSkillData data, int index)
    {
        thirdPassiveInstance?.EndPassive(this);
        thirdPassiveInstance = new PlayerPassive(data, index);
        thirdPassiveInstance.StartPassive(this);
    }

    private void StartAbility(ActiveSkillData data, int index)
    {
        IsAttacking = true;
        PlayerSkill skill = new PlayerSkill(data, index);

        if (data.TargetingMode == ActiveSkillData.Targeting.Ground)
        {
            skill.GroundTargetPosition = Indicator.LastGroundPosition;
        }

        Indicator.DestroyAllIndicators();
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

    public void RequestSpawn(SkillContext context, NetworkedSpawnEffect effect)
    {
        if (IsServer)
        {
            context = ResolveServerContext(context);
            effect.SpawnServer(this, context);
        }
        else
        {
            RequestSpawnServerRpc(context);
        }
    }

    public void SpawnSingle(NetworkedSpawnEffect effect, SkillContext context)
    {
        if (!IsServer) return;

        GameObject instance = Instantiate(effect.Prefab,context.SpawnPosition + context.AimOffset,context.AimRotation);

        NetworkObject networkObject = instance.GetComponent<NetworkObject>();

        if (networkObject == null)
        {
            Debug.LogError($"SpawnEffect prefab '{effect.Prefab.name}' " + "does not have a NetworkObject.");
            Destroy(instance);
            return;
        }

        networkObject.Spawn();

        effect.Configure(instance,this,context);
    }

    [ServerRpc]
    void RequestSpawnServerRpc(SkillContext context)
    {
        context = ResolveServerContext(context);
        ActiveSkillData data = GetSkillData(context.SkillType, context.SkillIndex);
        SkillEffect effect = data.GetEffects(context.Phase)[context.EffectIndex];

        if (effect is not NetworkedSpawnEffect spawnEffect)
        {
            Debug.LogError($"Effect {context.EffectIndex} is not a NetworkedSpawnEffect.");

            return;
        }

        spawnEffect.SpawnServer(this, context);
    }

    ActiveSkillData GetSkillData(ActiveSkillData.SkillType type, int index) => type switch
    {
        ActiveSkillData.SkillType.Basic => skills.basicAbilities[index],
        ActiveSkillData.SkillType.Offensive => skills.offensiveAbilities[index],
        ActiveSkillData.SkillType.Mobility => skills.mobilityAbilities[index],
        ActiveSkillData.SkillType.Defensive => skills.defensiveAbilities[index],
        ActiveSkillData.SkillType.Utility => skills.utilityAbilities[index],
        ActiveSkillData.SkillType.Ultimate => skills.ultimateAbilities[index],
        _ => null
    };

    public SkillContext ResolveServerContext(SkillContext context)
    {
        context.AttackerId = OwnerClientId;
        context.IsBasic = context.SkillType == ActiveSkillData.SkillType.Basic;
        context.AttackerDamage = Stats.TotalDamage;
        ActiveSkillData data = GetSkillData(context.SkillType, context.SkillIndex);

        context.AimRotation = Quaternion.Euler(0, 0, Mathf.Atan2(context.AimDirection.y, context.AimDirection.x) * Mathf.Rad2Deg);
        context.AimOffset = data.TargetingMode == ActiveSkillData.Targeting.Ground
            ? Vector2.zero
            : context.AimDirection.normalized * data.SkillRange;

        return context;
    }

    public float GetSkillRange(SkillContext context)
    {
        ActiveSkillData data = GetSkillData(context.SkillType,context.SkillIndex);
        return data != null ? data.SkillRange : 0f;
    }

    public bool IsGroundTargeted(SkillContext context)
    {
        ActiveSkillData data = GetSkillData(context.SkillType, context.SkillIndex);
        return data != null && data.TargetingMode == ActiveSkillData.Targeting.Ground;
    }
}