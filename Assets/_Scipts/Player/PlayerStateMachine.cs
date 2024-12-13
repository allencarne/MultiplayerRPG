using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateMachine : MonoBehaviour
{
    PlayerState state;

    [Header("Components")]
    [SerializeField] Animator bodyAnimator; public Animator BodyAnimator => bodyAnimator;
    [SerializeField] Animator hairAnimator; public Animator HairAnimator => hairAnimator;
    [SerializeField] Animator eyesAnimator; public Animator EyeAnimator => eyesAnimator;
    [SerializeField] Rigidbody2D rb; public Rigidbody2D Rigidbody => rb;

    [Header("Scripts")]
    [SerializeField] PlayerInputHandler inputHandler; public PlayerInputHandler InputHandler => inputHandler;
    [SerializeField] Player player; public Player Player => player;

    // Variables
    public Vector2 LastMoveDirection = Vector2.zero;
    [HideInInspector] public bool canRoll = true;

    [Header("Basic Ability")]
    [HideInInspector] public bool CanBasicAbility = true;
    [HideInInspector] public Quaternion AbilityDir;
    [HideInInspector] public bool hasAttacked = false;

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
            if (player.endurance >= 50)
            {
                SetState(new PlayerRollState(this));
            }
        }
    }

    public void BasicAbility(bool abilityInput)
    {
        if (abilityInput)
        {
            //SetState(new PlayerBasicState(this));
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


    public Vector2 SnapDirection(Vector2 direction)
    {
        // This Code allows the Last Input direction to be animated
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            direction.y = 0;
            direction.x = Mathf.Sign(direction.x);
        }
        else
        {
            direction.x = 0;
            direction.y = Mathf.Sign(direction.y);
        }

        return direction;
    }
}