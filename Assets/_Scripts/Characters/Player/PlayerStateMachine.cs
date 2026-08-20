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

            Indicator.DestroyAllIndicators();
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
            Indicator.InstantiateIndicator(skills.offensiveAbilities[player.OffensiveIndex].IndicatorPrefab, "Offensive");
        }
        else
        {
            Indicator.DestroyIndicator("Offensive");
        }

        if (!Input.HasBufferedOffensiveInput) return;

        IsAttacking = true;
        CanOffensive = false;

        Indicator.DestroyAllIndicators();
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
            Indicator.InstantiateIndicator(skills.mobilityAbilities[player.MobilityIndex].IndicatorPrefab, "Mobility");
        }
        else
        {
            Indicator.DestroyIndicator("Mobility");
        }

        if (!Input.HasBufferedMobilityInput) return;

        IsAttacking = true;
        CanMobility = false;

        Indicator.DestroyAllIndicators();
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
            Indicator.InstantiateIndicator(skills.defensiveAbilities[player.DefensiveIndex].IndicatorPrefab, "Defensive");
        }
        else
        {
            Indicator.DestroyIndicator("Defensive");
        }

        if (!Input.HasBufferedDefensiveInput) return;

        IsAttacking = true;
        CanDefensive = false;

        Indicator.DestroyAllIndicators();
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
            Indicator.InstantiateIndicator(skills.utilityAbilities[player.UtilityIndex].IndicatorPrefab, "Utility");
        }
        else
        {
            Indicator.DestroyIndicator("Utility");
        }

        if (!Input.HasBufferedUtilityInput) return;

        IsAttacking = true;
        CanUtility = false;

        Indicator.DestroyAllIndicators();
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
            Indicator.InstantiateIndicator(skills.ultimateAbilities[player.UltimateIndex].IndicatorPrefab, "Ultimate");
        }
        else
        {
            Indicator.DestroyIndicator("Ultimate");
        }

        if (!Input.HasBufferedUltimateInput) return;

        IsAttacking = true;
        CanUltimate = false;

        Indicator.DestroyAllIndicators();
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
        SkillData data = GetSkillData(context.SkillType, context.SkillIndex);
        SkillEffect effect = data.GetEffects(context.Phase)[context.EffectIndex];

        if (effect is not NetworkedSpawnEffect spawnEffect)
        {
            Debug.LogError($"Effect {context.EffectIndex} is not a NetworkedSpawnEffect.");

            return;
        }

        spawnEffect.SpawnServer(this, context);
    }

    SkillData GetSkillData(SkillData.SkillType type, int index) => type switch
    {
        SkillData.SkillType.Basic => skills.basicAbilities[index],
        SkillData.SkillType.Offensive => skills.offensiveAbilities[index],
        SkillData.SkillType.Mobility => skills.mobilityAbilities[index],
        SkillData.SkillType.Defensive => skills.defensiveAbilities[index],
        SkillData.SkillType.Utility => skills.utilityAbilities[index],
        SkillData.SkillType.Ultimate => skills.ultimateAbilities[index],
        _ => null
    };

    public SkillContext ResolveServerContext(SkillContext context)
    {
        context.AttackerId = OwnerClientId;
        context.IsBasic = context.SkillType == SkillData.SkillType.Basic;
        context.AttackerDamage = Stats.TotalDamage;
        SkillData data = GetSkillData(context.SkillType, context.SkillIndex);
        context.AimRotation = Quaternion.Euler(0, 0, Mathf.Atan2(context.AimDirection.y, context.AimDirection.x) * Mathf.Rad2Deg);
        context.AimOffset = context.AimDirection.normalized * data.SkillRange;
        return context;
    }

    public float GetSkillRange(SkillContext context)
    {
        SkillData data = GetSkillData(context.SkillType,context.SkillIndex);
        return data != null ? data.SkillRange : 0f;
    }
}