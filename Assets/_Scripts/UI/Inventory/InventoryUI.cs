using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public TextMeshProUGUI CoinText;
    [SerializeField] Inventory inventory;
    [SerializeField] Transform itemsParent;
    InventorySlot[] iSlots;

    private void OnEnable()
    {
        inventory.Stats.PlayerLevel.OnValueChanged += OnPlayerLevelSynced;
    }

    private void OnDisable()
    {
        inventory.Stats.PlayerLevel.OnValueChanged -= OnPlayerLevelSynced;
    }

    private void Awake()
    {
        iSlots = itemsParent.GetComponentsInChildren<InventorySlot>();

        // Initialize each inventory slot with its index and reference to the inventory
        for (int i = 0; i < iSlots.Length; i++)
        {
            // Set the slot index for each inventory slot
            iSlots[i].slotIndex = i;

            // Set the reference to the inventory for each inventory slot
            iSlots[i].inventory = inventory;
        }
    }

    public void UpdateUI()
    {
        // Update the coin text with the current coin count and a coin icon
        CoinText.text = $"{inventory.Stats.Coins}<sprite index=0>";

        // Update each inventory slot with the corresponding item from the inventory
        for (int i = 0; i < iSlots.Length; i++)
        {

            // Check if the inventory has an item for this slot
            if (i < inventory.items.Length && inventory.items[i] != null)
            {
                // Add the item to the slot
                iSlots[i].AddItem(inventory.items[i]);
            }
            else
            {
                // Clear the slot if it's empty
                iSlots[i].ClearSlot();
            }
        }

        // Invoke the OnInventoryChanged event to notify any listeners that the inventory has changed
        inventory.OnInventoryChanged?.Invoke();
    }

    void OnPlayerLevelSynced(int oldValue, int newValue)
    {
        UpdateUI();
    }
}
