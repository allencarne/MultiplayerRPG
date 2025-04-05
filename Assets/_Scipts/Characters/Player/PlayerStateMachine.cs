using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerStateMachine : NetworkBehaviour
{
    [Header("States")]
    [SerializeField] PlayerSpawnState playerSpawnState;
    [SerializeField] PlayerIdleState playerIdleState;
    [SerializeField] PlayerRunState playerRunState;
    [SerializeField] PlayerRollState playerRollState;
    [SerializeField] PlayerDeathState playerDeathState;

    [Header("Ability")]
    [SerializeField] PlayerAbility[] basicAbilities;
    [SerializeField] PlayerAbility[] offensiveAbilities;
    [SerializeField] PlayerAbility[] mobilityAbilities;
    [SerializeField] PlayerAbility[] defensiveAbilities;
    [SerializeField] PlayerAbility[] utilityAbilities;
    [SerializeField] PlayerAbility[] ultimateAbilities;

    [Header("Ability Indexes")]
    public int currentBasicIndex = -1;
    private int currentOffensiveIndex = -1;
    private int currentMobilityIndex = -1;
    private int currentDefensiveIndex = -1;
    private int currentUtilityIndex = -1;
    private int currentUltimateIndex = -1;

    [Header("Animators")]
    public Animator SwordAnimator;
    public Animator BodyAnimator;
    public Animator HairAnimator;
    public Animator EyesAnimator;

    [Header("Components")]
    public PlayerInputHandler InputHandler;
    public PlayerEquipment Equipment;
    public Rigidbody2D PlayerRB;
    public Transform Aimer;
    public Player player;

    [Header("Variables")]
    [HideInInspector] public Vector2 LastMoveDirection = Vector2.zero;
    [HideInInspector] public bool CanRoll = true;
    public bool isAttacking = false;
    public bool CanBasic = true;
    public bool canOffensive= true;
    public bool canMobility = true;
    public bool canDefensive = true;
    public bool canUtility = true;
    public bool canUltimate = true;

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

    private void Start()
    {
        playerSpawnState.StartState(this);
    }

    private void Update()
    {
        switch (state)
        {
            case State.Spawn: playerSpawnState.UpdateState(this); break;

            case State.Idle: playerIdleState.UpdateState(this); break;

            case State.Run: playerRunState.UpdateState(this); break;

            case State.Roll: playerRollState.UpdateState(this); break;

            case State.Death: playerDeathState.UpdateState(this); break;

            case State.Basic: basicAbilities[currentBasicIndex].UpdateAbility(this); break;

            case State.Offensive: offensiveAbilities[currentOffensiveIndex].UpdateAbility(this); break;

            case State.Mobility: mobilityAbilities[currentMobilityIndex].UpdateAbility(this); break;

            case State.Defensive: defensiveAbilities[currentDefensiveIndex].UpdateAbility(this); break;

            case State.Ultility: utilityAbilities[currentUtilityIndex].UpdateAbility(this); break;

            case State.Ultimate: ultimateAbilities[currentUltimateIndex].UpdateAbility(this); break;
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

            case State.Death: playerDeathState.FixedUpdateState(this); break;

            case State.Basic: basicAbilities[currentBasicIndex].FixedUpdateAbility(this); break;

            case State.Offensive: offensiveAbilities[currentOffensiveIndex].FixedUpdateAbility(this); break;

            case State.Mobility: mobilityAbilities[currentMobilityIndex].FixedUpdateAbility(this); break;

            case State.Defensive: defensiveAbilities[currentDefensiveIndex].FixedUpdateAbility(this); break;

            case State.Ultility: utilityAbilities[currentUtilityIndex].FixedUpdateAbility(this); break;

            case State.Ultimate: ultimateAbilities[currentUltimateIndex].FixedUpdateAbility(this); break;

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
            if (player.Endurance >= 50)
            {
                SetState(State.Roll);
            }
        }
    }

    public void BasicAbility(bool abilityInput)
    {
        if (!CanBasic) return;
        if (isAttacking) return;
        if (!Equipment.IsWeaponEquipt) return;

        if (abilityInput)
        {
            if (currentBasicIndex >= 0 && currentBasicIndex < basicAbilities.Length)
            {
                state = State.Basic;
                basicAbilities[currentBasicIndex].StartAbility(this);
            }
        }
    }

    public void OffensiveAbility(bool abilityInput)
    {
        if (!canOffensive) return;
        if (isAttacking) return;
        if (!Equipment.IsWeaponEquipt) return;

        if (abilityInput)
        {
            if (currentOffensiveIndex >= 0 && currentOffensiveIndex < offensiveAbilities.Length)
            {
                state = State.Offensive;
                offensiveAbilities[currentOffensiveIndex].StartAbility(this);
            }
        }
    }

    public void MobilityAbility(bool abilityInput)
    {
        if (!canMobility) return;
        if (isAttacking) return;
        if (!Equipment.IsWeaponEquipt) return;

        if (abilityInput)
        {
            if (currentMobilityIndex >= 0 && currentMobilityIndex < mobilityAbilities.Length)
            {
                state = State.Mobility;
                mobilityAbilities[currentMobilityIndex].StartAbility(this);
            }
        }
    }

    public void DefensiveAbility(bool abilityInput)
    {
        if (!canDefensive) return;
        if (isAttacking) return;
        if (!Equipment.IsWeaponEquipt) return;

        if (abilityInput)
        {
            if (currentDefensiveIndex >= 0 && currentDefensiveIndex < defensiveAbilities.Length)
            {
                state = State.Defensive;
                defensiveAbilities[currentDefensiveIndex].StartAbility(this);
            }
        }
    }

    public void UtilityAbility(bool abilityInput)
    {
        if (!canUtility) return;
        if (isAttacking) return;
        if (!Equipment.IsWeaponEquipt) return;

        if (abilityInput)
        {
            if (currentUtilityIndex >= 0 && currentUtilityIndex < utilityAbilities.Length)
            {
                state = State.Ultility;
                utilityAbilities[currentUtilityIndex].StartAbility(this);
            }
        }
    }

    public void UltimateAbility(bool abilityInput)
    {
        if (!canUltimate) return;
        if (isAttacking) return;
        if (!Equipment.IsWeaponEquipt) return;

        if (abilityInput)
        {
            if (currentUltimateIndex >= 0 && currentUltimateIndex < ultimateAbilities.Length)
            {
                state = State.Ultimate;
                ultimateAbilities[currentUltimateIndex].StartAbility(this);
            }
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
}