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
