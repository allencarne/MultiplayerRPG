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

    [Header("Animators")]
    public Animator SwordAnimator;
    public Animator BodyAnimator;
    public Animator HairAnimator;
    public Animator EyesAnimator;

    [Header("Components")]
    public PlayerInputHandler InputHandler;
    public PlayerAbilities Abilities;
    public PlayerEquipment Equipment;
    public Rigidbody2D PlayerRB;
    public Transform Aimer;
    public Player player;

    [Header("Variables")]
    [HideInInspector] public Vector2 LastMoveDirection = Vector2.zero;
    [HideInInspector] public bool canRoll = true;
    [HideInInspector] public bool isAttacking = false;
    [HideInInspector] public bool CanBasic = true;
    [HideInInspector] public bool canOffensiveAbility = true;
    [HideInInspector] public bool canMobilityAbility = true;

    public enum State
    {
        Spawn,
        Idle,
        Run,
        Roll,
        Death,
        Basic,
        Offensive,
        Defensive,
        Mobility,
        Ultility,
        Ultimate,
    }

    public State state = State.Spawn;

    private void Awake()
    {
        //SetState(new PlayerSpawnState(this));
    }

    private void Start()
    {
        playerSpawnState.StartState(this);
    }

    private void Update()
    {
        //state.Update();

        switch (state)
        {
            case State.Spawn: playerSpawnState.UpdateState(this); break;

            case State.Idle: playerIdleState.UpdateState(this); break;

            case State.Run: playerRunState.UpdateState(this); break;

            case State.Roll: playerRollState.UpdateState(this); break;

            case State.Death: playerDeathState.UpdateState(this); break;

        }
    }

    private void FixedUpdate()
    {
        //state.FixedUpdate();

        switch (state)
        {
            case State.Spawn: playerSpawnState.FixedUpdateState(this); break;

            case State.Idle: playerIdleState.FixedUpdateState(this); break;

            case State.Run: playerRunState.FixedUpdateState(this); break;

            case State.Roll: playerRollState.FixedUpdateState(this); break;

            case State.Death: playerDeathState.FixedUpdateState(this); break;

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
        if (rollInput && canRoll)
        {
            if (player.Endurance >= 50)
            {
                SetState(State.Roll);
            }
        }
    }

    public void BasicAbility(bool abilityInput)
    {
        if (abilityInput && CanBasic && Equipment.IsWeaponEquipt && Abilities.basicAbilityReference != null)
        {
            if (Abilities.basicAbility != null)
            {
                //SetState(new PlayerBasicState(this));
            }
        }
    }

    [ServerRpc]
    public void AttackServerRpc(Vector3 spawnPosition, Quaternion spawnRotation)
    {
        var instance = Instantiate(player.AttackPrefab, spawnPosition, spawnRotation);
        var instanceNetworkObject = instance.GetComponent<NetworkObject>();

        Physics2D.IgnoreCollision(instance.GetComponent<Collider2D>(), gameObject.GetComponent<Collider2D>());
        instanceNetworkObject.Spawn();
    }


    public void OffensiveAbility(bool abilityInput)
    {
        if (abilityInput)
        {
            //SetState(new PlayerOffensiveState(this));
        }
    }

    public void MobilityAbility(bool abilityInput)
    {
        if (abilityInput)
        {
            //SetState(new PlayerMobilityState(this));
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