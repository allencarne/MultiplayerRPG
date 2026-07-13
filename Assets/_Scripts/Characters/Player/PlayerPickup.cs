using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPickup : NetworkBehaviour
{
    // References
    [SerializeField] Player player;

    // Values
    bool canPickup = true;

    // UI
    [SerializeField] TextMeshProUGUI pickupText;

    // Input
    [SerializeField] PlayerInputHandler input;
    [SerializeField] PlayerInput playerInput;
    [SerializeField] InputActionReference pickupAction;

    // Pickup
    ItemPickup currentTarget;
    readonly List<ItemPickup> itemsInRange = new List<ItemPickup>();

    public override void OnNetworkSpawn()
    {
        // Subscribe so pickupText updates if the player switches input devices mid-game
        playerInput.onControlsChanged += OnControlsChanged;
    }

    public override void OnNetworkDespawn()
    {
        // Unsubscribe to avoid calling into a destroyed object
        playerInput.onControlsChanged -= OnControlsChanged;
    }

    private void Start()
    {
        // Start with no pickup prompt showing until we're actually near an item
        pickupText.enabled = false;
    }

    private void Update()
    {
        // Only the local player needs to run pickup logic
        if (!player.IsLocalPlayer) return;

        // Recalculate which item (if any) is the closest one in range
        RefreshTarget();

        // If we have a target, the pickup button was pressed, and we're not on cooldown
        if (currentTarget != null && input.PickupInput && canPickup)
        {
            // Prevent picking up again until the cooldown coroutine finishes
            canPickup = false;

            // Attempt to pick up the current target
            currentTarget.PickUp(player);

            // Start the cooldown timer
            StartCoroutine(PickupCooldown());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Only the local player should be tracking nearby items
        if (!player.IsLocalPlayer) return;

        // Ignore anything that isn't tagged as an item
        if (!collision.CompareTag("Item")) return;

        // Get the ItemPickup component from the collider we entered
        ItemPickup item = collision.GetComponent<ItemPickup>();

        // Return if this isn't a valid item, or if we're already tracking it
        if (item == null || itemsInRange.Contains(item)) return;

        // Add this item to the list of items currently in range
        itemsInRange.Add(item);

        // Listen for this item despawning while it's still in range, so we can
        // clean it up even if we never get an OnTriggerExit2D for it
        item.Despawned += HandleItemDespawned;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Only the local player should be tracking nearby items
        if (!player.IsLocalPlayer) return;

        // Ignore anything that isn't tagged as an item
        if (!collision.CompareTag("Item")) return;

        // Get the ItemPickup component from the collider we exited
        ItemPickup item = collision.GetComponent<ItemPickup>();

        // Return if this isn't a valid item
        if (item == null) return;

        // Stop tracking this item since it's no longer in range
        RemoveFromRange(item);
    }

    void HandleItemDespawned(ItemPickup item)
    {
        RemoveFromRange(item);
    }

    void RemoveFromRange(ItemPickup item)
    {
        // Stop listening for this item's despawn now that we're done tracking it
        item.Despawned -= HandleItemDespawned;

        // Remove it from the in-range list
        itemsInRange.Remove(item);

        // If the item we just removed was our current target, clear the target
        if (currentTarget == item) SetTarget(null);
    }

    void RefreshTarget()
    {
        // Track the closest item found so far
        ItemPickup closest = null;

        // Start with an impossibly large distance so the first item always beats it
        float closestDist = float.MaxValue;

        // Check every item currently in range
        foreach (ItemPickup item in itemsInRange)
        {
            // Calculate Distance
            float dist = (item.transform.position - transform.position).sqrMagnitude;

            // If this item is closer than anything we've found so far, it becomes the new closest
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = item;
            }
        }

        // Only update the target (and UI) if the closest item actually changed
        if (closest != currentTarget) SetTarget(closest);
    }

    void SetTarget(ItemPickup target)
    {
        // Assign the new target
        currentTarget = target;

        if (currentTarget == null)
        {
            // No target - hide the pickup prompt and tooltip
            pickupText.enabled = false;
            player.HideToolTip();
        }
        else
        {
            // We have a target - show its tooltip and update the pickup prompt text
            player.ShowToolTip(currentTarget.GetSlotData());
            UpdatePickupText(currentTarget.Item.name);
        }
    }

    void UpdatePickupText(string itemName)
    {
        // Make sure the prompt is visible now that we have something to show
        pickupText.enabled = true;

        // Get the player's current control scheme (keyboard, gamepad, etc.)
        string controlScheme = playerInput.currentControlScheme;

        // Find which binding index corresponds to that control scheme
        int bindingIndex = GetBindingIndexForCurrentScheme(controlScheme);

        // Get a human-readable name for that binding (e.g. "E" or "Gamepad South")
        string bindName = pickupAction.action.GetBindingDisplayString(bindingIndex);

        // Build the final prompt text, highlighting the button and item name in green
        pickupText.text = $"Press <color=#00FF00>{bindName}</color> to pick up <color=#00FF00><b>{itemName}</b></color>";
    }

    void OnControlsChanged(PlayerInput input)
    {
        if (currentTarget != null) UpdatePickupText(currentTarget.Item.name);
    }

    int GetBindingIndexForCurrentScheme(string scheme)
    {
        // If there's no active control scheme yet, fall back to the first binding
        if (string.IsNullOrEmpty(scheme)) return 0;

        // Search through every binding on the pickup action
        for (int i = 0; i < pickupAction.action.bindings.Count; i++)
        {
            InputBinding binding = pickupAction.action.bindings[i];

            // Return the index of the binding that belongs to the current control scheme
            if (binding.groups.Contains(scheme)) return i;
        }

        // Fallback if no matching binding was found
        return 0;
    }

    IEnumerator PickupCooldown()
    {
        yield return new WaitForSeconds(.2f);
        canPickup = true;
    }
}
