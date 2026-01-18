using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ContextMenu : MonoBehaviour
{
    [SerializeField] PlayerEquipment equipment;
    [SerializeField] GameObject contextMenu;
    [SerializeField] InventorySlot inventorySlot;
    [SerializeField] ItemToolTip tooltip;

    [SerializeField] Button border_Button;

    [SerializeField] Button use_Button;
    [SerializeField] Button split_Button;
    [SerializeField] Button drop_Button;
    [SerializeField] Button sell_Button;

    private void OnEnable()
    {
        StartCoroutine(CheckSelection());
    }

    private void OnDisable()
    {
        EventSystem.current.SetSelectedGameObject(border_Button.gameObject);
        contextMenu.SetActive(false);
    }

    public void _UseButton()
    {
        inventorySlot.UseItem();
        contextMenu.SetActive(false);
    }

    public void _SplitButton()
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
        inventory.Save.SaveInventory(data.item, inventorySlot.slotIndex, remainingAmount);

        // Place split amount into empty slot
        inventory.AddItemToSlot(data.item, splitAmount, targetSlot);

        // Refresh UI
        inventory.inventoryUI.UpdateUI();

        EventSystem.current.SetSelectedGameObject(border_Button.gameObject);
        contextMenu.SetActive(false);
    }

    public void _DropButton()
    {
        InventorySlotData data = inventorySlot.slotData;
        Inventory inventory = inventorySlot.inventory;

        if (data == null) return;

        if (equipment.IsServer)
        {
            GameObject dropped = Instantiate(data.item.Prefab, inventory.Save.transform.position, Quaternion.identity);
            NetworkObject netObj = dropped.GetComponent<NetworkObject>();
            netObj.Spawn();
            dropped.GetComponent<ItemPickup>().Quantity.Value = data.quantity;
        }
        else
        {
            equipment.DropItemServerRPC(data.item.ITEM_ID, data.quantity,inventory.Save.transform.position);
        }

        inventory.RemoveItemBySlot(inventorySlot.slotIndex, data.quantity);
        EventSystem.current.SetSelectedGameObject(border_Button.gameObject);
        contextMenu.SetActive(false);
    }

    public void _SellButton()
    {
        InventorySlotData data = inventorySlot.slotData;
        Inventory inventory = inventorySlot.inventory;

        if (data == null) return;

        inventory.CoinCollected(data.item.SellValue);
        inventory.RemoveItemBySlot(inventorySlot.slotIndex, data.quantity);
        EventSystem.current.SetSelectedGameObject(border_Button.gameObject);
        contextMenu.SetActive(false);
    }

    private IEnumerator CheckSelection()
    {
        while (contextMenu.activeSelf)
        {
            GameObject current = EventSystem.current.currentSelectedGameObject;

            bool isValid =
                current == use_Button.gameObject ||
                current == split_Button.gameObject ||
                current == drop_Button.gameObject ||
                current == sell_Button.gameObject ||
                current == border_Button.gameObject;

            if (!isValid)
            {
                contextMenu.SetActive(false);
                yield break;
            }

            yield return null;
        }
    }
}
