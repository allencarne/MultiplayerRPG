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
    public bool OffensiveAbilityInput { get; private set; }
    public bool MobilityAbilityInput { get; private set; }
    public bool DefensiveAbilityInput { get; private set; }
    public bool UtilityAbilityInput { get; private set; }
    public bool UltimateAbilityInput { get; private set; }
    public Vector2 MousePosition { get; private set; }
    public bool PickupInput { get; private set; }
    public bool InteractInput { get; private set; }

    LayerMask ignoredLayers;

    public UnityEvent OnPauseUI;
    public UnityEvent OnInventoryUI;
    public UnityEvent OnStatsUI;
    public UnityEvent OnAbilityUI;
    public UnityEvent OnMapUI;

    private void Awake()
    {
        // Initialize ignoredLayers
        ignoredLayers = 1 << LayerMask.NameToLayer("IgnoredUI");
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
        if (!isMouseOverUI(ignoredLayers))
        {
            BasicAbilityInput = context.ReadValueAsButton();
        }
        else
        {
            BasicAbilityInput = false;
        }
    }

    public void OnOffensiveAbility(InputAction.CallbackContext context)
    {
        if (!isMouseOverUI(ignoredLayers))
        {
            OffensiveAbilityInput = context.ReadValueAsButton();
        }
        else
        {
            OffensiveAbilityInput = false;
        }
    }

    public void OnMobilityAbility(InputAction.CallbackContext context)
    {
        if (!isMouseOverUI(ignoredLayers))
        {
            MobilityAbilityInput = context.ReadValueAsButton();
        }
        else
        {
            MobilityAbilityInput = false;
        }
    }

    public void OnDefensiveAbility(InputAction.CallbackContext context)
    {

        if (!isMouseOverUI(ignoredLayers))
        {
            DefensiveAbilityInput = context.ReadValueAsButton();
        }
        else
        {
            DefensiveAbilityInput = false;
        }
    }

    public void OnUtilityAbility(InputAction.CallbackContext context)
    {

        if (!isMouseOverUI(ignoredLayers))
        {
            UtilityAbilityInput = context.ReadValueAsButton();
        }
        else
        {
            UtilityAbilityInput = false;
        }
    }

    public void OnUltimateAbility(InputAction.CallbackContext context)
    {

        if (!isMouseOverUI(ignoredLayers))
        {
            UltimateAbilityInput = context.ReadValueAsButton();
        }
        else
        {
            UltimateAbilityInput = false;
        }
    }

    public void OnMousePos(InputAction.CallbackContext context)
    {
        MousePosition = Camera.main.ScreenToWorldPoint(context.ReadValue<Vector2>());
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

    public void OnInventoryInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OnInventoryUI.Invoke();
        }
    }

    public void OnStatsInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OnStatsUI.Invoke();
        }
    }

    public void OnAbilityInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OnAbilityUI.Invoke();
        }
    }

    public void OnMapInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OnMapUI.Invoke();
        }
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OnPauseUI.Invoke();
        }
    }

    bool isMouseOverUI(LayerMask ignoredLayers)
    {
        // Create a new PointerEventData
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        // Create a list to store the results of the raycast
        List<RaycastResult> results = new List<RaycastResult>();

        // Perform the raycast
        EventSystem.current.RaycastAll(eventData, results);

        // Check if any of the hit objects are not on the ignored layers
        foreach (RaycastResult result in results)
        {
            if ((ignoredLayers & (1 << result.gameObject.layer)) == 0)
            {
                // If the hit object is not on the ignored layers, return true
                return true;
            }
        }

        // If none of the hit objects are not on the ignored layers, return false
        return false;
    }
}