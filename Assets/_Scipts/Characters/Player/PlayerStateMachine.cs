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
    [SerializeField] PlayerHurtState playerHurtState;
    [SerializeField] PlayerDeathState playerDeathState;

    [Header("Animators")]
    public Animator SwordAnimator;
    public Animator BodyAnimator;
    public Animator HairAnimator;
    public Animator EyesAnimator;

    [Header("Components")]
    [HideInInspector] public SkillPanel skills;
    public PlayerInputHandler Input;
    public Collider2D Collider;
    public CrowdControl CrowdControl;
    public Buffs Buffs;
    public DeBuffs DeBuffs;
    public PlayerEquipment Equipment;
    public Rigidbody2D PlayerRB;
    public Transform Aimer;
    public Player player;

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

    public enum State
    {
        Spawn,
        Idle,
        Run,
        Roll,
        Hurt,
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

        BodyAnimator.SetFloat("Vertical", -1);
    }

    private void Update()
    {
        TestMethods();

        if (player.FirstPassiveIndex > -1 && player.FirstPassiveIndex <= skills.firstPassive.Length)
        {
            skills.firstPassive[player.FirstPassiveIndex].UpdateAbility(this);
        }

        if (player.SecondPassiveIndex > -1 && player.SecondPassiveIndex <= skills.secondPassive.Length)
        {
            skills.secondPassive[player.SecondPassiveIndex].UpdateAbility(this);
        }

        if (player.ThirdPassiveIndex > -1 && player.ThirdPassiveIndex <= skills.thirdPassive.Length)
        {
            skills.thirdPassive[player.ThirdPassiveIndex].UpdateAbility(this);
        }

        switch (state)
        {
            case State.Spawn: playerSpawnState.UpdateState(this); break;
            case State.Idle: playerIdleState.UpdateState(this); break;
            case State.Run: playerRunState.UpdateState(this); break;
            case State.Roll: playerRollState.UpdateState(this); break;
            case State.Hurt: playerHurtState.UpdateState(this); break;
            case State.Death: playerDeathState.UpdateState(this); break;
            case State.Basic: skills.basicAbilities[player.BasicIndex].UpdateAbility(this); break;
            case State.Offensive: skills.offensiveAbilities[player.OffensiveIndex].UpdateAbility(this); break;
            case State.Mobility: skills.mobilityAbilities[player.MobilityIndex].UpdateAbility(this); break;
            case State.Defensive: skills.defensiveAbilities[player.DefensiveIndex].UpdateAbility(this); break;
            case State.Ultility: skills.utilityAbilities[player.UtilityIndex].UpdateAbility(this); break;
            case State.Ultimate: skills.ultimateAbilities[player.UltimateIndex].UpdateAbility(this); break;
        }
    }

    private void FixedUpdate()
    {
        if (player.FirstPassiveIndex > -1 && player.FirstPassiveIndex <= skills.firstPassive.Length)
        {
            skills.firstPassive[player.FirstPassiveIndex].FixedUpdateAbility(this);
        }

        if (player.SecondPassiveIndex > -1 && player.SecondPassiveIndex <= skills.secondPassive.Length)
        {
            skills.secondPassive[player.SecondPassiveIndex].FixedUpdateAbility(this);
        }

        if (player.ThirdPassiveIndex > -1 && player.ThirdPassiveIndex <= skills.thirdPassive.Length)
        {
            skills.thirdPassive[player.ThirdPassiveIndex].FixedUpdateAbility(this);
        }

        switch (state)
        {
            case State.Spawn: playerSpawnState.FixedUpdateState(this); break;
            case State.Idle: playerIdleState.FixedUpdateState(this); break;
            case State.Run: playerRunState.FixedUpdateState(this); break;
            case State.Roll: playerRollState.FixedUpdateState(this); break;
            case State.Hurt: playerHurtState.FixedUpdateState(this); break;
            case State.Death: playerDeathState.FixedUpdateState(this); break;
            case State.Basic: skills.basicAbilities[player.BasicIndex].FixedUpdateAbility(this); break;
            case State.Offensive: skills.offensiveAbilities[player.OffensiveIndex].FixedUpdateAbility(this); break;
            case State.Mobility: skills.mobilityAbilities[player.MobilityIndex].FixedUpdateAbility(this); break;
            case State.Defensive: skills.defensiveAbilities[player.DefensiveIndex].FixedUpdateAbility(this); break;
            case State.Ultility: skills.utilityAbilities[player.UtilityIndex].FixedUpdateAbility(this); break;
            case State.Ultimate: skills.ultimateAbilities[player.UltimateIndex].FixedUpdateAbility(this); break;
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
            case State.Hurt: state = State.Hurt; playerHurtState.StartState(this); break;
            case State.Death: state = State.Death; playerDeathState.StartState(this); break;
        }
    }

    public void Roll()
    {
        if (CrowdControl.stun.IsStunned) return;
        if (!CanRoll) return;

        if (Input.RollInput)
        {
            if (player.Endurance.Value >= 50)
            {
                SetState(State.Roll);
            }
        }
    }

    public void BasicAbility()
    {
        if (!CanBasic) return;
        if (IsAttacking) return;
        if (!Equipment.IsWeaponEquipt) return;
        if (player.BasicIndex < 0) return;
        if (player.BasicIndex >= skills.basicAbilities.Length) return;
        if (CrowdControl.disarm.IsDisarmed) return;

        if (Input.BasicAbilityInput)
        {
            DestroyAllIndicators();
            state = State.Basic;
            skills.basicAbilities[player.BasicIndex].StartAbility(this);
        }
    }

    GameObject indicator;
    string indicatorType = null;

    public void OffensiveAbility()
    {
        if (!CanOffensive) return;
        if (IsAttacking) return;
        if (!Equipment.IsWeaponEquipt) return;
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
        skills.offensiveAbilities[player.OffensiveIndex].StartAbility(this);
    }

    public void MobilityAbility()
    {
        if (!CanMobility) return;
        if (IsAttacking) return;
        if (!Equipment.IsWeaponEquipt) return;
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
        skills.mobilityAbilities[player.MobilityIndex].StartAbility(this);
    }

    public void DefensiveAbility()
    {
        if (!CanDefensive) return;
        if (IsAttacking) return;
        if (!Equipment.IsWeaponEquipt) return;
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
        skills.defensiveAbilities[player.DefensiveIndex].StartAbility(this);
    }

    public void UtilityAbility()
    {
        if (!CanUtility) return;
        if (IsAttacking) return;
        if (!Equipment.IsWeaponEquipt) return;
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
        skills.utilityAbilities[player.UtilityIndex].StartAbility(this);
    }

    public void UltimateAbility()
    {
        if (!CanUltimate) return;
        if (IsAttacking) return;
        if (!Equipment.IsWeaponEquipt) return;
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
        skills.ultimateAbilities[player.UltimateIndex].StartAbility(this);
    }

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

    private void OnTriggerStay2D(Collider2D collision)
    {
        ItemPickup item = collision.GetComponent<ItemPickup>();
        if (item != null)
        {
            if (Input.PickupInput)
            {
                item.PickUp(player);
            }
        }
    }

    Coroutine CurrentAttack;

    public void HandlePotentialInterrupt()
    {
        if (!CrowdControl.IsInterrupted) return;
        if (player.CastBar.castBarFill.color != Color.black) return;

        if (IsServer)
        {
            player.CastBar.InterruptCastBar();
        }
        else
        {
            player.CastBar.InterruptServerRpc();
        }

        if (CurrentAttack != null)
        {
            StopCoroutine(CurrentAttack);
            CurrentAttack = null;
        }

        IsAttacking = false;
        return;
    }

    public IEnumerator CoolDownTime(SkillType type, float skillCoolDown)
    {
        yield return new WaitForSeconds(skillCoolDown);

        switch (type)
        {
            case SkillType.Basic: CanBasic = true; break;
            case SkillType.Offensive: CanOffensive = true; break;
            case SkillType.Mobility: CanMobility = true; break;
            case SkillType.Defensive: CanDefensive = true; break;
            case SkillType.Utility: CanUtility = true; break;
            case SkillType.Ultimate: CanUltimate = true; break;
        }
    }

    public void StartCast(float modifiedCastTime, float recoveryTime, PlayerAbility ability)
    {
        CurrentAttack = StartCoroutine(CastTime(modifiedCastTime, recoveryTime, ability));
    }

    IEnumerator CastTime(float modifiedCastTime, float recoveryTime, PlayerAbility ability)
    {
        yield return new WaitForSeconds(modifiedCastTime);

        if (!IsAttacking) yield break;
        if (player.IsDead) yield break;

        BodyAnimator.Play("Sword_Attack_I");
        SwordAnimator.Play("Sword_Attack_I");
        EyesAnimator.Play("Sword_Attack_I");
        HairAnimator.Play("Sword_Attack_I_" + player.hairIndex);

        StartCoroutine(ImpactTime(recoveryTime, ability));
    }

    IEnumerator ImpactTime(float recoveryTime, PlayerAbility ability)
    {
        yield return new WaitForSeconds(.1f);

        if (!IsAttacking) yield break;
        if (player.IsDead) yield break;

        ability.Impact(this);

        BodyAnimator.Play("Sword_Attack_R");
        SwordAnimator.Play("Sword_Attack_R");
        EyesAnimator.Play("Sword_Attack_R");
        HairAnimator.Play("Sword_Attack_R_" + player.hairIndex);

        // Start Recovery
        if (IsServer)
        {
            player.CastBar.StartRecovery(recoveryTime, player.CurrentAttackSpeed.Value);
        }
        else
        {
            player.CastBar.StartRecoveryServerRpc(recoveryTime, player.CurrentAttackSpeed.Value);
        }

        StartCoroutine(RecoveryTime(recoveryTime));
    }

    IEnumerator RecoveryTime(float modifiedRecoveryTime)
    {
        yield return new WaitForSeconds(modifiedRecoveryTime);

        if (!IsAttacking) yield break;
        if (player.IsDead) yield break;

        IsAttacking = false;
        if (state != State.Hurt)
        {
            SetState(State.Idle);
        }
    }

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
        HairAnimator.Play("Sword_Attack_C_" + player.hairIndex);
    }

    public void StartCast(float castTime)
    {
        if (IsServer)
        {
            player.CastBar.StartCast(castTime, player.CurrentAttackSpeed.Value);
        }
        else
        {
            player.CastBar.StartCastServerRpc(castTime, player.CurrentAttackSpeed.Value);
        }
    }

    void TestMethods()
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.F1))
        {
            Buffs.regeneration.Regeneration(HealType.Flat, 1,.5f,10);
        }

        if (UnityEngine.Input.GetKeyDown(KeyCode.F2))
        {
            CrowdControl.silence.StartSilence(4);
            //Buffs.might.StartMight(1, 10);
        }

        if (UnityEngine.Input.GetKeyDown(KeyCode.F3))
        {
            CrowdControl.immobilize.StartImmobilize(4);
            //Buffs.alacrity.StartAlacrity(1, 10);
        }

        if (UnityEngine.Input.GetKeyDown(KeyCode.F4))
        {
            CrowdControl.stun.StartStun(4);
            //Buffs.protection.StartProtection(1, 10);
        }

        if (UnityEngine.Input.GetKeyDown(KeyCode.F5))
        {
            CrowdControl.knockUp.StartKnockUp(.5f);
            //Buffs.swiftness.StartSwiftness(1, 10);
        }

        if (UnityEngine.Input.GetKeyDown(KeyCode.F6))
        {
            DeBuffs.slow.StartSlow(1, 10);
        }

        if (UnityEngine.Input.GetKeyDown(KeyCode.F7))
        {
            DeBuffs.weakness.StartWeakness(1, 10);
        }

        if (UnityEngine.Input.GetKeyDown(KeyCode.F8))
        {
            DeBuffs.impede.StartImpede(1, 10);
        }

        if (UnityEngine.Input.GetKeyDown(KeyCode.F9))
        {
            DeBuffs.vulnerability.StartVulnerability(1, 10);
        }

        if (UnityEngine.Input.GetKeyDown(KeyCode.F10))
        {
            DeBuffs.exhaust.StartExhaust(1, 10);
        }
    }

    public void Hurt()
    {
        if (player.IsDead) return;
        SetState(State.Hurt);
    }

    public void Death()
    {
        SetState(State.Death);
    }

    [ServerRpc]
    public void RequestDisableColliderServerRpc()
    {
        Collider.enabled = false;
        player.SwordSprite.enabled = false;
        player.EyeSprite.enabled = false;
        player.HairSprite.enabled = false;
        player.AimerSprite.enabled = false;
        ApplyColliderStateClientRpc(false);
    }

    [ServerRpc]
    public void RequestEnableColliderServerRpc()
    {
        Collider.enabled = true;
        player.SwordSprite.enabled = true;
        player.EyeSprite.enabled = true;
        player.HairSprite.enabled = true;
        player.AimerSprite.enabled = true;
        ApplyColliderStateClientRpc(true);
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
    public void RequestRespawnServerRpc(Vector3 position)
    {
        transform.position = position;
        player.GiveHeal(100, HealType.Percentage);
    }
}