using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "ScriptableObjects/Inventory/Item")]
public class Item : ScriptableObject
{
    [Header("Item")]
    public GameObject prefab;

    [Header("Stats")]
    new public string name; // Maybe Remove this by getting prefab name
    public Sprite icon;
    public int cost; // What Venders will sell this item for
    public int sellValue; // What you can sell to vendors for

    [Header("Bools")]
    public bool isCurrency;

    [Header("Stack")]
    public bool isStackable;
    public int quantity; // Amount of this item. Example: Drop 10 coins at once

    public virtual void Use(Inventory _inventory, EquipmentManager _equipmentManager)
    {

    }

    public void RemoveFromInventory(Inventory _inventory)
    {
        _inventory.RemoveItem(this);
    }
}
