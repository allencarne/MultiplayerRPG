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
        playerInput.onControlsChanged += OnControlsChanged;
    }

    public override void OnNetworkDespawn()
    {
        playerInput.onControlsChanged -= OnControlsChanged;
    }

    private void Start()
    {
        pickupText.enabled = false;
    }

    private void Update()
    {
        if (!player.IsLocalPlayer) return;

        RefreshTarget();

        if (currentTarget != null && input.PickupInput && canPickup)
        {
            canPickup = false;
            currentTarget.PickUp(player);
            StartCoroutine(PickupCooldown());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!player.IsLocalPlayer) return;
        if (!collision.CompareTag("Item")) return;

        ItemPickup item = collision.GetComponent<ItemPickup>();
        if (item == null || itemsInRange.Contains(item)) return;

        itemsInRange.Add(item);
        item.Despawned += HandleItemDespawned;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!player.IsLocalPlayer) return;
        if (!collision.CompareTag("Item")) return;

        ItemPickup item = collision.GetComponent<ItemPickup>();
        if (item == null) return;

        RemoveFromRange(item);
    }

    void HandleItemDespawned(ItemPickup item)
    {
        RemoveFromRange(item);
    }

    void RemoveFromRange(ItemPickup item)
    {
        item.Despawned -= HandleItemDespawned;
        itemsInRange.Remove(item);
        if (currentTarget == item) SetTarget(null);
    }

    void RefreshTarget()
    {
        ItemPickup closest = null;
        float closestDist = float.MaxValue;

        foreach (ItemPickup item in itemsInRange)
        {
            float dist = (item.transform.position - transform.position).sqrMagnitude;
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = item;
            }
        }

        if (closest != currentTarget) SetTarget(closest);
    }

    void SetTarget(ItemPickup target)
    {
        currentTarget = target;

        if (currentTarget == null)
        {
            pickupText.enabled = false;
            player.HideToolTip();
        }
        else
        {
            player.ShowToolTip(currentTarget.GetSlotData());
            UpdatePickupText(currentTarget.Item.name);
        }
    }

    void UpdatePickupText(string itemName)
    {
        pickupText.enabled = true;

        string controlScheme = playerInput.currentControlScheme;
        int bindingIndex = GetBindingIndexForCurrentScheme(controlScheme);
        string bindName = pickupAction.action.GetBindingDisplayString(bindingIndex);

        pickupText.text = $"Press <color=#00FF00>{bindName}</color> to pick up <color=#00FF00><b>{itemName}</b></color>";
    }

    void OnControlsChanged(PlayerInput input)
    {
        if (currentTarget != null) UpdatePickupText(currentTarget.Item.name);
    }

    int GetBindingIndexForCurrentScheme(string scheme)
    {
        if (string.IsNullOrEmpty(scheme)) return 0;

        for (int i = 0; i < pickupAction.action.bindings.Count; i++)
        {
            InputBinding binding = pickupAction.action.bindings[i];
            if (binding.groups.Contains(scheme)) return i;
        }

        return 0;
    }

    IEnumerator PickupCooldown()
    {
        yield return new WaitForSeconds(.2f);
        canPickup = true;
    }
}
