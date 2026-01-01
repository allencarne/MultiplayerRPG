using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public TextMeshProUGUI CoinText;
    [SerializeField] Inventory inventory;
    [SerializeField] Transform itemsParent;
    InventorySlot[] iSlots;

    private void Awake()
    {
        iSlots = itemsParent.GetComponentsInChildren<InventorySlot>();

        for (int i = 0; i < iSlots.Length; i++)
        {
            iSlots[i].slotIndex = i;
            iSlots[i].inventory = inventory;
        }
    }

    public void UpdateUI()
    {
        CoinText.text = $"{inventory.Stats.Coins}<sprite index=0>";

        // Loop through all inventory slots
        for (int i = 0; i < iSlots.Length; i++)
        {
            // If there's an item in the corresponding slot
            if (i < inventory.items.Length && inventory.items[i] != null)
            {
                // Add the item to the slot
                iSlots[i].AddItem(inventory.items[i]?.item, inventory.items[i]?.quantity ?? 0);
            }
            else
            {
                // Clear the slot if it's empty
                iSlots[i].ClearSlot();
            }
        }
    }
}
