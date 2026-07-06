using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemPickup : NetworkBehaviour
{
    [HideInInspector] public SpawnItem Manager;
    [HideInInspector] public Transform SpawnPoint;
    [SerializeField] ItemStatGenerator generator;

    [SerializeField] TextMeshProUGUI pickupText;
    [SerializeField] TextMeshProUGUI quantityText;

    [SerializeField] InputActionReference pickupAction;
    [SerializeField] Animator animator;
    PlayerInput playerInput;

    bool _hasBeenPickedUp = false;


    public override void OnNetworkSpawn()
    {
        generator.net_Quantity.OnValueChanged += OnQuantityChanged;
        UpdateQuantityUI(generator.net_Quantity.Value);
    }

    public override void OnNetworkDespawn()
    {
        generator.net_Quantity.OnValueChanged -= OnQuantityChanged;
    }

    private void Start()
    {
        if (IsServer && Manager == null)
        {
            StartCoroutine(DespawnDelay(180));
        }
    }

    public void PickUp(Player player)
    {
        if (_hasBeenPickedUp) return;
        _hasBeenPickedUp = true;

        // Add Item to Inventory if we have enough space
        InventorySlotData item = new InventorySlotData(generator.Item, generator.net_Quantity.Value, generator.net_ItemRarity.Value, generator.net_ItemQuality.Value, generator.GetRolledModifiers());

        bool wasPickedUp = player.PlayerInventory.AddItem(item);

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

        player.ShowToolTip(new InventorySlotData(generator.Item, generator.net_Quantity.Value, generator.net_ItemRarity.Value, generator.net_ItemQuality.Value, generator.GetRolledModifiers()));

        UpdatePickupText();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        Player player = collision.GetComponent<Player>();
        if (!player || !player.IsLocalPlayer) return;

        player.HideToolTip();
        pickupText.text = "";
    }

    private void UpdatePickupText()
    {
        if (playerInput == null || pickupAction == null)
        {
            pickupText.text = $"Press Interact to pick up {generator.Item.name}";
            return;
        }

        string controlScheme = playerInput.currentControlScheme;
        int bindingIndex = GetBindingIndexForCurrentScheme(controlScheme);

        string bindName = pickupAction.action.GetBindingDisplayString(bindingIndex);
        pickupText.text = $"Press <color=#00FF00>{bindName}</color> to pick up <color=#00FF00><b>{generator.Item.name}</b></color>";
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
        //player.HideToolTip();
        animator.Play("Anim_Item_Pickup");

        Collider2D collider = GetComponent<Collider2D>();
        if (collider) collider.enabled = false;

        if (!IsServer) PlayPickupAnimationServerRpc();

        StartCoroutine(DespawnDelay(.6f));
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    void PlayPickupAnimationServerRpc()
    {
        animator.Play("Anim_Item_Pickup");
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

    IEnumerator DespawnDelay(float time)
    {
        yield return new WaitForSeconds(time);

        if (IsServer)
        {
            if (Manager != null) Manager.AddPosition(SpawnPoint);
            NetworkObject.Despawn(true);
        }
        else
        {
            DespawnServerRPC();
        }
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    void DespawnServerRPC()
    {
        if (Manager != null) Manager.AddPosition(SpawnPoint);
        NetworkObject.Despawn(true);
    }
}
