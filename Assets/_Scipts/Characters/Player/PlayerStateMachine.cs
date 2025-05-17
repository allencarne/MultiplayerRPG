using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerStateMachine : NetworkBehaviour
{
    [Header("States")]
    [SerializeField] PlayerSpawnState playerSpawnState;
    [SerializeField] PlayerIdleState playerIdleState;
    [SerializeField] PlayerRunState playerRunState;
    [SerializeField] PlayerRollState playerRollState;
    [SerializeField] PlayerDeathState playerDeathState;

    [Header("Animators")]
    public Animator SwordAnimator;
    public Animator BodyAnimator;
    public Animator HairAnimator;
    public Animator EyesAnimator;

    [Header("Components")]
    [HideInInspector] public SkillPanel skills;
    public PlayerInputHandler InputHandler;
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
    public bool CanOffensive= true;
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
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Buffs.Haste(1, 10);
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            DeBuffs.Slow(1, 10);
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            Buffs.Might(1, 10);
        }

        if (Input.GetKeyDown(KeyCode.F4))
        {
            DeBuffs.Weakness(1, 10);
        }

        if (Input.GetKeyDown(KeyCode.F5))
        {
            Buffs.Alacrity(1, 10);
        }

        if (Input.GetKeyDown(KeyCode.F6))
        {
            DeBuffs.Impede(1, 10);
        }

        if (Input.GetKeyDown(KeyCode.F7))
        {
            Buffs.Protection(1, 10);
        }

        if (Input.GetKeyDown(KeyCode.F8))
        {
            DeBuffs.Vulnerability(1, 10);
        }

        if (Input.GetKeyDown(KeyCode.F9))
        {
            Buffs.Swiftness(1, 10);
        }

        if (Input.GetKeyDown(KeyCode.F10))
        {
            DeBuffs.Exhaust(1, 10);
        }

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
        if (CrowdControl.IsImmobilized) return;

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

            case State.Death: state = State.Death; playerDeathState.StartState(this); break;
        }
    }

    public void Roll(bool rollInput)
    {
        if (rollInput && CanRoll)
        {
            if (player.Endurance.Value >= 50)
            {
                SetState(State.Roll);
            }
        }
    }

    public void BasicAbility(bool abilityInput)
    {
        if (!CanBasic) return;
        if (IsAttacking) return;
        if (!Equipment.IsWeaponEquipt) return;
        if (player.BasicIndex < 0) return;
        if (player.BasicIndex >= skills.basicAbilities.Length) return;

        if (abilityInput)
        {
            state = State.Basic;
            skills.basicAbilities[player.BasicIndex].StartAbility(this);
        }
    }
    
    public void OffensiveAbility()
    {
        if (!CanOffensive || 
            IsAttacking || 
            !Equipment.IsWeaponEquipt ||
            player.OffensiveIndex < 0 || 
            player.OffensiveIndex >= skills.offensiveAbilities.Length)
        {
            // Even if we can't use the ability, clear the input release so it's not buffered
            InputHandler.IsOffensiveReleased = false;
            return;
        }

        if (InputHandler.IsOffensiveHeld)
        {
            Debug.Log("Held");
        }
        else
        {
            Debug.Log("Not Held");
        }

        if (InputHandler.IsOffensiveReleased)
        {
            state = State.Offensive;
            skills.offensiveAbilities[player.OffensiveIndex].StartAbility(this);
            InputHandler.IsOffensiveReleased = false;
        }
    }

    public void MobilityAbility(bool abilityInput)
    {
        if (!CanMobility) return;
        if (IsAttacking) return;
        if (!Equipment.IsWeaponEquipt) return;
        if (player.MobilityIndex < 0) return;
        if (player.MobilityIndex >= skills.mobilityAbilities.Length) return;

        if (abilityInput)
        {
            state = State.Mobility;
            skills.mobilityAbilities[player.MobilityIndex].StartAbility(this);
        }
    }

    public void DefensiveAbility(bool abilityInput)
    {
        if (!CanDefensive) return;
        if (IsAttacking) return;
        if (!Equipment.IsWeaponEquipt) return;
        if (player.DefensiveIndex < 0) return;
        if (player.DefensiveIndex >= skills.defensiveAbilities.Length) return;

        if (abilityInput)
        {
            state = State.Defensive;
            skills.defensiveAbilities[player.DefensiveIndex].StartAbility(this);
        }
    }

    public void UtilityAbility(bool abilityInput)
    {
        if (!CanUtility) return;
        if (IsAttacking) return;
        if (!Equipment.IsWeaponEquipt) return;
        if (player.UtilityIndex < 0) return;
        if (player.UtilityIndex >= skills.utilityAbilities.Length) return;

        if (abilityInput)
        {
            state = State.Ultility;
            skills.utilityAbilities[player.UtilityIndex].StartAbility(this);
        }
    }

    public void UltimateAbility(bool abilityInput)
    {
        if (!CanUltimate) return;
        if (IsAttacking) return;
        if (!Equipment.IsWeaponEquipt) return;
        if (player.UltimateIndex < 0) return;
        if (player.UltimateIndex >= skills.ultimateAbilities.Length) return;

        if (abilityInput)
        {
            state = State.Ultimate;
            skills.ultimateAbilities[player.UltimateIndex].StartAbility(this);
        }
    }
    
    // This Code allows the Last Input direction to be animated
    public Vector2 SnapDirection(Vector2 direction)
    {
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
            if (InputHandler.PickupInput)
            {
                item.PickUp(player);
            }
        }
    }

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

        IsAttacking = false;
        SetState(State.Idle);
        return;
    }

    public IEnumerator CoolDownTime(SkillType type, float skillCoolDown)
    {
        float modifiedCooldown = skillCoolDown / player.CurrentCDR.Value;

        yield return new WaitForSeconds(modifiedCooldown);

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

    public IEnumerator CastTime(float modifiedCastTime, float recoveryTime, PlayerAbility ability)
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
        SetState(State.Idle);
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

    public void StartSlide()
    {
        if (InputHandler.MoveInput != Vector2.zero)
        {
            IsSliding = true;
        }
        else
        {
            PlayerRB.linearVelocity = Vector2.zero;
        }
    }
}