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

    public Item Item;
    public NetworkVariable<int> Quantity = new NetworkVariable<int>(1,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);
    public ItemRarity ItemRarity;
    public ItemQuality ItemQuality;
    public List<StatModifier> RolledModifiers = new List<StatModifier>();
    bool _hasBeenPickedUp = false;


    public override void OnNetworkSpawn()
    {
        Quantity.OnValueChanged += OnQuantityChanged;
        UpdateQuantityUI(Quantity.Value);

        // Server should generate stats once for new spawned loot.
        if (IsServer)
        {
            // If an Equipment item and we haven't already rolled modifiers, roll now
            if (Item is Equipment)
            {
                // Only roll if no modifiers exist (prevents re-rolls when item is re-created from saved data)
                if (RolledModifiers == null || RolledModifiers.Count == 0)
                {
                    InventorySlotData temp = new InventorySlotData(Item, Quantity.Value, ItemRarity, ItemQuality, RolledModifiers);
                    generator.EnsureRolled(temp);

                    // apply back to this pickup's networked fields so they replicate
                    ItemRarity = temp.rarity;
                    ItemQuality = temp.quality;
                    RolledModifiers = temp.modifiers;
                }
            }
            else
            {
                ItemRarity = Item.ItemRarity;
                ItemQuality = Item.ItemQuality;
            }
        }
    }

    public override void OnNetworkDespawn()
    {
        Quantity.OnValueChanged -= OnQuantityChanged;
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
        bool wasPickedUp = player.PlayerInventory.AddItem(Item, Quantity.Value, ItemRarity, ItemQuality, RolledModifiers);

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

        player.ShowToolTip(new InventorySlotData(Item, Quantity.Value, ItemRarity, ItemQuality, RolledModifiers));

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
            pickupText.text = $"Press Interact to pick up {Item.name}";
            return;
        }

        string controlScheme = playerInput.currentControlScheme;
        int bindingIndex = GetBindingIndexForCurrentScheme(controlScheme);

        string bindName = pickupAction.action.GetBindingDisplayString(bindingIndex);
        pickupText.text = $"Press <color=#00FF00>{bindName}</color> to pick up <color=#00FF00><b>{Item.name}</b></color>";
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
