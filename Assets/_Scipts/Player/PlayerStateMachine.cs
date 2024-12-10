using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateMachine : MonoBehaviour
{
    PlayerState state;

    [Header("Components")]
    //[SerializeField] Animator swordAnimator; public Animator SwordAnimator => swordAnimator;
    [SerializeField] Animator bodyAnimator; public Animator BodyAnimator => bodyAnimator;
    [SerializeField] Animator hairAnimator; public Animator HairAnimator => hairAnimator;
    [SerializeField] Animator eyesAnimator; public Animator EyeAnimator => eyesAnimator;
    //[SerializeField] Animator headAnimator; public Animator HeadAnimator => headAnimator;
    //[SerializeField] Animator chestAnimator; public Animator ChestAnimator => chestAnimator;
    //[SerializeField] Animator legsAnimator; public Animator LegsAnimator => legsAnimator;
    [SerializeField] Rigidbody2D rb; public Rigidbody2D Rigidbody => rb;
    //[SerializeField] Transform aimer; public Transform Aimer => aimer;

    [Header("Scripts")]
    [SerializeField] PlayerInputHandler inputHandler; public PlayerInputHandler InputHandler => inputHandler;
    [SerializeField] Player player; public Player Player => player;

    // Variables
    public Vector2 LastMoveDirection = Vector2.zero;

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

    void Update()
    {
        state.Update();
    }

    public void OnPlayerDeath()
    {
        //SetState(new PlayerDeathState(this));
    }

    private void FixedUpdate()
    {
        state.FixedUpdate();
    }

    public void SetState(PlayerState newState) => state = newState;

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
}