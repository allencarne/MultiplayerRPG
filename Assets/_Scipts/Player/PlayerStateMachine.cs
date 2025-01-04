using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateMachine : MonoBehaviour
{
    PlayerState state;

    [Header("Components")]
    [SerializeField] Animator swordAnimator; public Animator SwordAnimator => swordAnimator;
    [SerializeField] Animator bodyAnimator; public Animator BodyAnimator => bodyAnimator;
    [SerializeField] Animator hairAnimator; public Animator HairAnimator => hairAnimator;
    [SerializeField] Animator eyesAnimator; public Animator EyeAnimator => eyesAnimator;
    [SerializeField] Rigidbody2D rb; public Rigidbody2D Rigidbody => rb;

    [SerializeField] PlayerAbilities abilities; public PlayerAbilities Abilities => abilities;

    [Header("Scripts")]
    [SerializeField] PlayerInputHandler inputHandler; public PlayerInputHandler InputHandler => inputHandler;
    [SerializeField] Player player; public Player Player => player;

    // Variables
    public Vector2 LastMoveDirection = Vector2.zero;
    [HideInInspector] public bool canRoll = true;
    [HideInInspector] public bool isAttacking = false;

    [Header("Basic Ability")]
    [HideInInspector] public bool CanBasic = true;

    [Header("Offensive Ability")]
    [HideInInspector] public bool canOffensiveAbility = true;

    [Header("Mobility Ability")]
    [HideInInspector] public bool canMobilityAbility = true;

    private void Awake()
    {
        SetState(new PlayerSpawnState(this));
    }

    private void Update()
    {
        state.Update();
    }

    private void FixedUpdate()
    {
        state.FixedUpdate();
    }

    public void SetState(PlayerState newState)
    {
        state = newState;
        state.Start();
    }

    public void Roll(bool rollInput)
    {
        if (rollInput && canRoll)
        {
            if (player.Endurance >= 50)
            {
                SetState(new PlayerRollState(this));
            }
        }
    }

    public void BasicAbility(bool abilityInput)
    {
        if (abilityInput && CanBasic)
        {
            if (abilities.basicAbility != null)
            {
                SetState(new PlayerBasicState(this));
            }
        }
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

    public void OnPlayerDeath()
    {
        //SetState(new PlayerDeathState(this));
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
            if (inputHandler.PickupInput)
            {
                item.PickUp(player);
            }
        }
    }
}