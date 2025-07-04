using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "ScriptableObjects/Inventory/Item")]
public class Item : ScriptableObject
{
    [Header("Item")]
    public GameObject Prefab;
    public Sprite Icon;

    [Header("Stats")]
    public int PurchaseValue; // What Venders will sell this item for
    public int SellValue; // What you can sell to vendors for
    public string Description;
    public ItemRarity ItemRarity;
    [Range(0, 100)]
    public int DropChance;

    [Header("Currency")]
    public bool IsStackable;
    public int Quantity; // Amount of this item. Example: Drop 10 coins at once

    public virtual void Use(Inventory _inventory, EquipmentManager _equipmentManager)
    {

    }

    public void RemoveFromInventory(Inventory _inventory)
    {
        _inventory.RemoveItem(this);
    }
}

public enum ItemRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Exotic,
    Mythic,
    Legendary
}
