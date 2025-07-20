using UnityEngine;
using TMPro;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class ItemPickup : NetworkBehaviour
{
    public Item Item;
    [SerializeField] GameObject toolTip;
    [SerializeField] TextMeshProUGUI pickupText;
    [SerializeField] InputActionReference pickupAction;
    bool _hasBeenPickedUp = false;
    PlayerInput playerInput;

    public void PickUp(Player player)
    {
        if (_hasBeenPickedUp) return;
        _hasBeenPickedUp = true;

        if (Item is Currency)
        {
            player.CoinCollected(Item.Quantity);

            if (IsServer)
            {
                NetworkObject.Despawn(true);
            }
            else
            {
                DespawnServerRPC();
            }

            return;
        }

        // Add Item to Inventory if we have enough space
        bool wasPickedUp = player.PlayerInventory.AddItem(Item);

        // Destroy item if it was collected
        if (wasPickedUp)
        {
            if (IsServer)
            {
                NetworkObject.Despawn(true);
            }
            else
            {
                DespawnServerRPC();
            }
        }
        else
        {
            _hasBeenPickedUp = false;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void DespawnServerRPC()
    {
        NetworkObject.Despawn(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        Player player = collision.GetComponent<Player>();
        if (!player || !player.IsLocalPlayer) return;

        // Store reference to PlayerInput
        playerInput = player.GetComponent<PlayerInput>();
        if (playerInput == null) return;

        // Show tooltip
        toolTip.SetActive(true);
        UpdatePickupText();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        Player player = collision.GetComponent<Player>();
        if (!player || !player.IsLocalPlayer) return;

        toolTip.SetActive(false);
        pickupText.text = "";
    }

    private void UpdatePickupText()
    {
        if (playerInput == null || pickupAction == null)
        {
            pickupText.text = "Press Interact";
            return;
        }

        string controlScheme = playerInput.currentControlScheme;
        int bindingIndex = GetBindingIndexForCurrentScheme(controlScheme);

        string bindName = pickupAction.action.GetBindingDisplayString(bindingIndex);
        pickupText.text = $"Press <color=#00FF00>{bindName}</color> to pick up";
    }

    private int GetBindingIndexForCurrentScheme(string scheme)
    {
        for (int i = 0; i < pickupAction.action.bindings.Count; i++)
        {
            var binding = pickupAction.action.bindings[i];
            if (binding.groups.Contains(scheme))
            {
                return i;
            }
        }

        return 0; // Fallback
    }
}
