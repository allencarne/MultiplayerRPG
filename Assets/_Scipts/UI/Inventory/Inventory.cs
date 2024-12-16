using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    int inventorySlots = 30;
    public List<Item> items = new List<Item>();
    public InventorySlot[] slots;

    public void AddItem(Item newItem)
    {
        items.Add(newItem);
        Debug.Log($"Added {newItem} to inventory.");

        //Update UI
    }

    public void RemoveItem(Item itemName)
    {
        if (items.Contains(itemName))
        {
            items.Remove(itemName);
            Debug.Log($"Removed {itemName} from inventory.");
        }
        else
        {
            Debug.Log($"{itemName} is not in the inventory.");
        }
    }

    public void DisplayInventory()
    {
        Debug.Log("Inventory Contents:");
        foreach (Item item in items)
        {
            Debug.Log($"- {item}");
        }
    }
}
