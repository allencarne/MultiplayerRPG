using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerStateMachine : NetworkBehaviour
{
    [Header("States")]
    [SerializeField] PlayerSpawnState playerSpawnState;
    [SerializeField] PlayerIdleState playerIdleState;
    [SerializeField] PlayerRunState playerRunState;
    [SerializeField] PlayerRollState playerRollState;
    [SerializeField] PlayerStaggerState PlayerStaggerState;
    [SerializeField] PlayerDeathState playerDeathState;

    [Header("Skills")]
    [HideInInspector] public PlayerSkill CurrentSkill;
    [SerializeField] SetSkillPanel setSkills;
    [HideInInspector] public SkillPanel skills;
    public SkillBarCoolDowns coolDownTracker;

    [Header("Animators")]
    public Animator SwordAnimator;
    public Animator BodyAnimator;
    //public Animator HairAnimator;
    //public Animator EyesAnimator;
    public Animator HeadAnimator;
    public Animator ChestAnimator;
    public Animator LegsAnimator;

    public Animator PlayerHeadAnimator;
    public PlayerHead playerHead;

    [Header("Scrips")]
    public Player player;
    public PlayerStats Stats;
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

    [Header("Status Effects")]
    public CrowdControl CrowdControl;
    public Buffs Buffs;
    public DeBuffs DeBuffs;

    [Header("Variables")]
    [HideInInspector] public Vector2 LastMoveDirection = Vector2.zero;
    [HideInInspector] public bool CanRoll = true;
    public bool IsAttacking = false;
    public bool IsSliding = false;
    public bool CanBasic = true;
    public bool CanOffensive = true;
    public bool CanMobility = true;
    public bool CanDefensive = true;
    public bool CanUtility = true;
    public bool CanUltimate = true;
    bool canPickup = true;

    [Header("Indicator")]
    string indicatorType = null;
    GameObject indicator;

    public enum State
    {
        Spawn,
        Idle,
        Run,
        Roll,
        Stagger,
        Death,
        Basic,
        Offensive,
        Mobility,
        Defensive,
        Ultility,
        Ultimate,
    }

    public State state = State.Spawn;

    public enum SkillType
    {
        Basic,
        Offensive,
        Mobility,
        Defensive,
        Utility,
        Ultimate,
    }

    private void Start()
    {
        playerSpawnState.StartState(this);

        PlayerHeadAnimator.SetFloat("Vertical", -1);
        BodyAnimator.SetFloat("Vertical", -1);
        //HairAnimator.SetFloat("Vertical", -1);
        //EyesAnimator.SetFloat("Vertical", -1);
        SwordAnimator.SetFloat("Vertical", -1);

        setSkills.SetSkills();

        if (skills == null) return;

        switch (Stats.playerClass)
        {
            case PlayerStats.PlayerClass.Beginner:

                if (player.FirstPassiveIndex > -1 && player.FirstPassiveIndex <= skills.firstPassive.Length)
                {
                    setSkills.begginerSkills.FirstPassiveButton(player.FirstPassiveIndex);
                }

                if (player.SecondPassiveIndex > -1 && player.SecondPassiveIndex <= skills.secondPassive.Length)
                {
                    setSkills.begginerSkills.SecondPassiveButton(player.FirstPassiveIndex);
                }

                if (player.ThirdPassiveIndex > -1 && player.ThirdPassiveIndex <= skills.thirdPassive.Length)
                {
                    setSkills.begginerSkills.ThirdPassiveButton(player.FirstPassiveIndex);
                }

                if (player.BasicIndex > -1 && player.BasicIndex <= skills.basicAbilities.Length)
                {
                    setSkills.begginerSkills.BasicButton(player.BasicIndex);
                }

                if (player.OffensiveIndex > -1 && player.OffensiveIndex <= skills.offensiveAbilities.Length)
                {
                    setSkills.begginerSkills.OffensiveButton(player.OffensiveIndex);
                }

                if (player.MobilityIndex > -1 && player.MobilityIndex <= skills.mobilityAbilities.Length)
                {
                    setSkills.begginerSkills.MobilityButton(player.MobilityIndex);
                }

                if (player.DefensiveIndex > -1 && player.DefensiveIndex <= skills.defensiveAbilities.Length)
                {
                    setSkills.begginerSkills.DefensiveButton(player.DefensiveIndex);
                }

                if (player.UtilityIndex > -1 && player.UtilityIndex <= skills.utilityAbilities.Length)
                {
                    setSkills.begginerSkills.UtilityButton(player.UtilityIndex);
                }

                if (player.UltimateIndex > -1 && player.UltimateIndex <= skills.ultimateAbilities.Length)
                {
                    setSkills.begginerSkills.UltimateButton(player.UltimateIndex);
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
        switch (state)
        {
            case State.Spawn: playerSpawnState.UpdateState(this); break;
            case State.Idle: playerIdleState.UpdateState(this); break;
            case State.Run: playerRunState.UpdateState(this); break;
            case State.Roll: playerRollState.UpdateState(this); break;
            case State.Stagger: PlayerStaggerState.UpdateState(this); break;
            case State.Death: playerDeathState.UpdateState(this); break;
            case State.Basic: skills.basicAbilities[player.BasicIndex].UpdateSkill(this); break;
            case State.Offensive: skills.offensiveAbilities[player.OffensiveIndex].UpdateSkill(this); break;
            case State.Mobility: skills.mobilityAbilities[player.MobilityIndex].UpdateSkill(this); break;
            case State.Defensive: skills.defensiveAbilities[player.DefensiveIndex].UpdateSkill(this); break;
            case State.Ultility: skills.utilityAbilities[player.UtilityIndex].UpdateSkill(this); break;
            case State.Ultimate: skills.ultimateAbilities[player.UltimateIndex].UpdateSkill(this); break;
        }

        if (player.FirstPassiveIndex > -1 && player.FirstPassiveIndex <= skills.firstPassive.Length)
        {
            skills.firstPassive[player.FirstPassiveIndex].UpdateSkill(this);
        }

        if (player.SecondPassiveIndex > -1 && player.SecondPassiveIndex <= skills.secondPassive.Length)
        {
            skills.secondPassive[player.SecondPassiveIndex].UpdateSkill(this);
        }

        if (player.ThirdPassiveIndex > -1 && player.ThirdPassiveIndex <= skills.thirdPassive.Length)
        {
            skills.thirdPassive[player.ThirdPassiveIndex].UpdateSkill(this);
        }
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case State.Spawn: playerSpawnState.FixedUpdateState(this); break;
            case State.Idle: playerIdleState.FixedUpdateState(this); break;
            case State.Run: playerRunState.FixedUpdateState(this); break;
            case State.Roll: playerRollState.FixedUpdateState(this); break;
            case State.Stagger: PlayerStaggerState.FixedUpdateState(this); break;
            case State.Death: playerDeathState.FixedUpdateState(this); break;
            case State.Basic: skills.basicAbilities[player.BasicIndex].FixedUpdateSkill(this); break;
            case State.Offensive: skills.offensiveAbilities[player.OffensiveIndex].FixedUpdateSkill(this); break;
            case State.Mobility: skills.mobilityAbilities[player.MobilityIndex].FixedUpdateSkill(this); break;
            case State.Defensive: skills.defensiveAbilities[player.DefensiveIndex].FixedUpdateSkill(this); break;
            case State.Ultility: skills.utilityAbilities[player.UtilityIndex].FixedUpdateSkill(this); break;
            case State.Ultimate: skills.ultimateAbilities[player.UltimateIndex].FixedUpdateSkill(this); break;
        }

        if (player.FirstPassiveIndex > -1 && player.FirstPassiveIndex <= skills.firstPassive.Length)
        {
            skills.firstPassive[player.FirstPassiveIndex].FixedUpdateSkill(this);
        }

        if (player.SecondPassiveIndex > -1 && player.SecondPassiveIndex <= skills.secondPassive.Length)
        {
            skills.secondPassive[player.SecondPassiveIndex].FixedUpdateSkill(this);
        }

        if (player.ThirdPassiveIndex > -1 && player.ThirdPassiveIndex <= skills.thirdPassive.Length)
        {
            skills.thirdPassive[player.ThirdPassiveIndex].FixedUpdateSkill(this);
        }
    }

    public void SetState(State newState)
    {
        switch (newState)
        {
            case State.Spawn: state = State.Spawn; playerSpawnState.StartState(this); break;
            case State.Idle: state = State.Idle; playerIdleState.StartState(this); break;
            case State.Run: state = State.Run; playerRunState.StartState(this); break;
            case State.Roll: state = State.Roll; playerRollState.StartState(this); break;
            case State.Stagger: state = State.Stagger; PlayerStaggerState.StartState(this); break;
            case State.Death: state = State.Death; playerDeathState.StartState(this); break;
        }
    }

    public void Interrupt()
    {
        if (CurrentSkill == null) return;
        if (CurrentSkill.currentState != PlayerSkill.State.Cast) return;

        player.CastBar.StartInterrupt();
        CurrentSkill.DoneState(false, this);
    }

    public void Stagger()
    {
        if (Buffs.immoveable.IsImmovable) return;

        player.CastBar.StartInterrupt();

        if (CurrentSkill != null)
        {
            CurrentSkill.DoneState(true, this);
        }
        else
        {
            SetState(State.Stagger);
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
                SetState(State.Roll);
            }
        }
    }

    public void BasicAbility()
    {
        if (!CanBasic) return;
        if (IsAttacking) return;
        if (!Equipment.IsWeaponEquipped) return;
        if (player.BasicIndex < 0) return;
        if (player.BasicIndex >= skills.basicAbilities.Length) return;
        if (CrowdControl.disarm.IsDisarmed) return;

        if (Input.BasicAbilityInput)
        {
            DestroyAllIndicators();
            state = State.Basic;
            skills.basicAbilities[player.BasicIndex].StartSkill(this);
        }
    }

    public void OffensiveAbility()
    {
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

        DestroyAllIndicators();
        Input.HasBufferedOffensiveInput = false;
        state = State.Offensive;
        skills.offensiveAbilities[player.OffensiveIndex].StartSkill(this);
    }

    public void MobilityAbility()
    {
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

        DestroyAllIndicators();
        Input.HasBufferedMobilityInput = false;
        state = State.Mobility;
        skills.mobilityAbilities[player.MobilityIndex].StartSkill(this);
    }

    public void DefensiveAbility()
    {
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

        DestroyAllIndicators();
        Input.HasBufferedDefensiveInput = false;
        state = State.Defensive;
        skills.defensiveAbilities[player.DefensiveIndex].StartSkill(this);
    }

    public void UtilityAbility()
    {
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

        DestroyAllIndicators();
        Input.HasBufferedUtilityInput = false;
        state = State.Ultility;
        skills.utilityAbilities[player.UtilityIndex].StartSkill(this);
    }

    public void UltimateAbility()
    {
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

        DestroyAllIndicators();
        Input.HasBufferedUltimateInput = false;
        state = State.Ultimate;
        skills.ultimateAbilities[player.UltimateIndex].StartSkill(this);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        ItemPickup item = collision.GetComponent<ItemPickup>();
        if (item != null)
        {
            if (Input.PickupInput && canPickup)
            {
                canPickup = false;
                item.PickUp(player);
                StartCoroutine(PickupCoolDown());
            }
        }
    }

    IEnumerator PickupCoolDown()
    {
        yield return new WaitForSeconds(.2f);
        canPickup = true;
    }

    [ServerRpc]
    public void RequestDisableColliderServerRpc(bool isEnabled)
    {
        Collider.enabled = isEnabled;
        player.SwordSprite.enabled = isEnabled;
        player.EyeSprite.enabled = isEnabled;
        player.HairSprite.enabled = isEnabled;
        player.AimerSprite.enabled = isEnabled;
        ApplyColliderStateClientRpc(isEnabled);
    }

    [ClientRpc]
    void ApplyColliderStateClientRpc(bool isEnabled)
    {
        Collider.enabled = isEnabled;
        player.SwordSprite.enabled = isEnabled;
        player.EyeSprite.enabled = isEnabled;
        player.HairSprite.enabled = isEnabled;
        player.AimerSprite.enabled = isEnabled;
    }

    [ServerRpc]
    public void RequestRespawnServerRpc()
    {
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

        SwordAnimator.SetFloat("Horizontal", direction.x);
        SwordAnimator.SetFloat("Vertical", direction.y);

        //EyesAnimator.SetFloat("Horizontal", direction.x);
        //EyesAnimator.SetFloat("Vertical", direction.y);

        //HairAnimator.SetFloat("Horizontal", direction.x);
        //HairAnimator.SetFloat("Vertical", direction.y);

        HeadAnimator.SetFloat("Horizontal", direction.x);
        HeadAnimator.SetFloat("Vertical", direction.y);

        ChestAnimator.SetFloat("Horizontal", direction.x);
        ChestAnimator.SetFloat("Vertical", direction.y);

        LegsAnimator.SetFloat("Horizontal", direction.x);
        LegsAnimator.SetFloat("Vertical", direction.y);
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

    #region Slide

    public void StartSlide(bool requireMoveInput)
    {
        if (!requireMoveInput || Input.MoveInput != Vector2.zero)
        {
            IsSliding = true;
        }
        else
        {
            PlayerRB.linearVelocity = Vector2.zero;
        }
    }

    public IEnumerator SlideDuration(Vector2 aimDirection, float slideForce, float slideDuration)
    {
        float elapsed = 0f;
        Vector2 startVelocity = aimDirection * slideForce;

        while (elapsed < slideDuration)
        {
            float t = elapsed / slideDuration;
            PlayerRB.linearVelocity = Vector2.Lerp(startVelocity, Vector2.zero, t);

            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        PlayerRB.linearVelocity = Vector2.zero;
        IsSliding = false;
    }

    #endregion
}