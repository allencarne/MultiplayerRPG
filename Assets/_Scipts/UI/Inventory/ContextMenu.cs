using Unity.Netcode;
using UnityEngine;

public class ContextMenu : MonoBehaviour
{
    [SerializeField] GameObject contextMenu;
    [SerializeField] InventorySlot inventorySlot;
    [SerializeField] ItemToolTip tooltip;

    public void UseButton()
    {
        inventorySlot.UseItem();
        contextMenu.SetActive(false);
    }

    public void MoveButton()
    {

    }

    public void SplitButton()
    {
        InventorySlotData data = inventorySlot.slotData;
        Inventory inventory = inventorySlot.inventory;

        if (data == null || data.quantity < 2 || !data.item.IsStackable)
        {
            Debug.Log("Cannot split stack.");
            contextMenu.SetActive(false);
            return;
        }

        // Try to find an empty slot
        int targetSlot = System.Array.FindIndex(inventory.items, x => x == null);
        if (targetSlot == -1)
        {
            Debug.Log("No empty slot to split into.");
            contextMenu.SetActive(false);
            return;
        }

        // Calculate split amount (round down)
        int splitAmount = data.quantity / 2;
        int remainingAmount = data.quantity - splitAmount;

        // Update current slot with remaining amount
        data.quantity = remainingAmount;
        inventory.items[inventorySlot.slotIndex] = data;
        inventory.initialize.SaveInventory(data.item, inventorySlot.slotIndex, remainingAmount);

        // Place split amount into empty slot
        inventory.AddItemToSlot(data.item, splitAmount, targetSlot);

        // Refresh UI
        inventory.inventoryUI.UpdateUI();

        contextMenu.SetActive(false);
    }

    public void DropButton()
    {
        GameObject item = Instantiate(inventorySlot.slotData.item.Prefab, inventorySlot.inventory.initialize.transform.position, Quaternion.identity);
        NetworkObject netItem = item.GetComponent<NetworkObject>();
        netItem.Spawn();

        item.GetComponent<ItemPickup>().Quantity = inventorySlot.slotData.quantity;

        inventorySlot.ClearSlot();

        contextMenu.SetActive(false);
    }

    private void OnDisable()
    {
        contextMenu.SetActive(false);
    }
}
