using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<Item> items = new List<Item>();

    public void AddItem(Item itemName)
    {
        items.Add(itemName);
        Debug.Log($"Added {itemName} to inventory.");
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
