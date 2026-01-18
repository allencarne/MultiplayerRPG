using UnityEngine;
using TMPro;
using Unity.Netcode;
using UnityEngine.InputSystem;
using System.Collections;

public class ItemPickup : NetworkBehaviour
{
    [SerializeField] GameObject toolTip;
    [SerializeField] TextMeshProUGUI pickupText;
    [SerializeField] TextMeshProUGUI quantityText;

    [SerializeField] InputActionReference pickupAction;
    [SerializeField] Animator animator;
    PlayerInput playerInput;

    public Item Item;
    public NetworkVariable<int> Quantity = new NetworkVariable<int>(1,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);
    bool _hasBeenPickedUp = false;

    public override void OnNetworkSpawn()
    {
        Quantity.OnValueChanged += OnQuantityChanged;
        UpdateQuantityUI(Quantity.Value);
    }

    public override void OnNetworkDespawn()
    {
        Quantity.OnValueChanged -= OnQuantityChanged;
    }

    public void PickUp(Player player)
    {
        if (_hasBeenPickedUp) return;
        _hasBeenPickedUp = true;

        // Add Item to Inventory if we have enough space
        bool wasPickedUp = player.PlayerInventory.AddItem(Item, Quantity.Value);

        // Destroy item if it was collected
        if (wasPickedUp)
        {
            PlayPickupEffect();
        }
        else
        {
            _hasBeenPickedUp = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        Player player = collision.GetComponent<Player>();
        if (!player || !player.IsLocalPlayer) return;

        playerInput = player.GetComponent<PlayerInput>();
        if (playerInput == null) return;

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
            InputBinding binding = pickupAction.action.bindings[i];
            if (binding.groups.Contains(scheme))
            {
                return i;
            }
        }

        return 0; // Fallback
    }

    private void PlayPickupEffect()
    {
        toolTip.SetActive(false);
        animator.Play("Anim_Item_Pickup");

        Collider2D collider = GetComponent<Collider2D>();
        if (collider) collider.enabled = false;

        if (!IsServer) PlayPickupAnimationServerRpc();

        StartCoroutine(Delay());
    }

    [ServerRpc(RequireOwnership = false)]
    void PlayPickupAnimationServerRpc()
    {
        animator.Play("Anim_Item_Pickup");
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(.6f);

        gameObject.SetActive(false);

        if (IsServer)
        {
            NetworkObject.Despawn(true);
        }
        else
        {
            DespawnServerRPC();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void DespawnServerRPC()
    {
        NetworkObject.Despawn(true);
    }

    private void OnQuantityChanged(int previousValue, int newValue)
    {
        UpdateQuantityUI(newValue);
    }

    private void UpdateQuantityUI(int quantity)
    {
        if (quantity > 1)
        {
            quantityText.text = quantity.ToString();
        }
        else
        {
            quantityText.text = "";
        }
    }
}
