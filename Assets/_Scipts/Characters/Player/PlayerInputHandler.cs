using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool BasicAbilityInput { get; private set; }
    public bool IsOffensiveHeld { get; private set; }
    public bool IsOffensiveReleased;
    public float OffensiveBufferTime = .2f;
    public float OffensiveTimer = 0f;
    public bool HasBufferedOffensiveInput = false;
    public bool IsMobilityHeld { get; private set; }
    public bool IsMobilityReleased;
    public float MobilityBufferTime = .2f;
    public float MobilityTimer = 0f;
    public bool HasBufferedMobilityInput = false;
    public bool IsDefensiveHeld { get; private set; }
    public bool IsDefensiveReleased;
    public float DefensiveBufferTime = .2f;
    public float DefensiveTimer = 0f;
    public bool HasBufferedDefensiveInput = false;
    public bool IsUtilityHeld { get; private set; }
    public bool IsUtilityReleased;
    public float UtilityBufferTime = .2f;
    public float UtilityTimer = 0f;
    public bool HasBufferedUtilityInput = false;
    public bool IsUltimateHeld { get; private set; }
    public bool IsUltimateReleased;
    public float UltimateBufferTime = .2f;
    public float UltimateTimer = 0f;
    public bool HasBufferedUltimateInput = false;

    public Vector2 MousePosition { get; private set; }
    public bool PickupInput { get; private set; }
    public bool InteractInput { get; private set; }
    public bool RollInput { get; private set; }
    public Vector2 ZoomInput { get; private set; }

    [HideInInspector] public Camera cameraInstance;
    public UnityEvent OnCharacterUIInput;
    public UnityEvent OnJournalUIInput;
    public UnityEvent OnSocialUIInput;
    public UnityEvent OnMapUIInput;
    public UnityEvent OnSettingsUIInput;
    public event UnityAction<Vector2> ZoomPerformed;

    private void Update()
    {
        BufferOffensive();
        BufferMobility();
        BufferDefensive();
        BufferUtility();
        BufferUltimate();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();

        if (context.canceled)
        {
            MoveInput = Vector2.zero;
        }
    }
    
    public void OnLook(InputAction.CallbackContext context)
    {
        LookInput = context.ReadValue<Vector2>();

        if (context.canceled)
        {
            LookInput = Vector2.zero;
        }
    }

    public void OnBasicAbility(InputAction.CallbackContext context)
    {
        BasicAbilityInput = context.ReadValueAsButton();

        if (context.canceled)
        {
            BasicAbilityInput = false;
        }
    }

    public void OnOffensiveAbility(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            IsOffensiveHeld = true;
            IsOffensiveReleased = false;
        }
        else if (context.canceled)
        {
            IsOffensiveHeld = false;
            IsOffensiveReleased = true;
        }
    }

    public void OnMobilityAbility(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            IsMobilityHeld = true;
            IsMobilityReleased = false;
        }
        else if (context.canceled)
        {
            IsMobilityHeld = false;
            IsMobilityReleased = true;
        }
    }

    public void OnDefensiveAbility(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            IsDefensiveHeld = true;
            IsDefensiveReleased = false;
        }
        else if (context.canceled)
        {
            IsDefensiveHeld = false;
            IsDefensiveReleased = true;
        }
    }

    public void OnUtilityAbility(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            IsUtilityHeld = true;
            IsUtilityReleased = false;
        }
        else if (context.canceled)
        {
            IsUtilityHeld = false;
            IsUtilityReleased = true;
        }
    }

    public void OnUltimateAbility(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            IsUltimateHeld = true;
            IsUltimateReleased = false;
        }
        else if (context.canceled)
        {
            IsUltimateHeld = false;
            IsUltimateReleased = true;
        }
    }

    public void OnMousePos(InputAction.CallbackContext context)
    {
        if (cameraInstance == null) return;

        MousePosition = cameraInstance.ScreenToWorldPoint(context.ReadValue<Vector2>());
    }

    public void OnPickup(InputAction.CallbackContext context)
    {
        PickupInput = context.ReadValueAsButton();

        if (context.canceled)
        {
            PickupInput = false;
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        InteractInput = context.ReadValueAsButton();

        if (context.canceled)
        {
            InteractInput = false;
        }
    }

    public void OnCharacterInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OnCharacterUIInput.Invoke();
        }
    }

    public void OnJournalInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OnJournalUIInput.Invoke();
        }
    }

    public void OnSocialInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OnSocialUIInput.Invoke();
        }
    }

    public void OnMapInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OnMapUIInput.Invoke();
        }
    }

    public void OnSettingsInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OnSettingsUIInput.Invoke();
        }
    }

    public void OnRoll(InputAction.CallbackContext context)
    {
        RollInput = context.ReadValueAsButton();

        if (context.canceled)
        {
            RollInput = false;
        }
    }

    public void OnZoom(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // Read the Vector2 value from the scroll input
            ZoomInput = context.ReadValue<Vector2>();

            ZoomPerformed?.Invoke(ZoomInput);
        }
    }

    void BufferOffensive()
    {
        if (IsOffensiveReleased)
        {
            IsOffensiveReleased = false;
            HasBufferedOffensiveInput = true;
            OffensiveTimer = OffensiveBufferTime;
        }
        if (HasBufferedOffensiveInput)
        {
            OffensiveTimer -= Time.deltaTime;
            if (OffensiveTimer <= 0f)
            {
                HasBufferedOffensiveInput = false;
            }
        }
    }

    void BufferMobility()
    {
        if (IsMobilityReleased)
        {
            IsMobilityReleased = false;
            HasBufferedMobilityInput = true;
            MobilityTimer = MobilityBufferTime;
        }
        if (HasBufferedMobilityInput)
        {
            MobilityTimer -= Time.deltaTime;
            if (MobilityTimer <= 0f)
            {
                HasBufferedMobilityInput = false;
            }
        }
    }

    void BufferDefensive()
    {
        if (IsDefensiveReleased)
        {
            IsDefensiveReleased = false;
            HasBufferedDefensiveInput = true;
            DefensiveTimer = DefensiveBufferTime;
        }
        if (HasBufferedDefensiveInput)
        {
            DefensiveTimer -= Time.deltaTime;
            if (DefensiveTimer <= 0f)
            {
                HasBufferedDefensiveInput = false;
            }
        }
    }

    void BufferUtility()
    {
        if (IsUtilityReleased)
        {
            IsUtilityReleased = false;
            HasBufferedUtilityInput = true;
            UtilityTimer = UtilityBufferTime;
        }

        if (HasBufferedUtilityInput)
        {
            UtilityTimer -= Time.deltaTime;
            if (UtilityTimer <= 0f)
            {
                HasBufferedUtilityInput = false;
            }
        }
    }

    void BufferUltimate()
    {
        if (IsUltimateReleased)
        {
            IsUltimateReleased = false;
            HasBufferedUltimateInput = true;
            UltimateTimer = UltimateBufferTime;
        }

        if (HasBufferedUltimateInput)
        {
            UltimateTimer -= Time.deltaTime;
            if (UltimateTimer <= 0f)
            {
                HasBufferedUltimateInput = false;
            }
        }
    }
}