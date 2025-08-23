using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Scriptable Objects/Item")]
public class Item : ScriptableObject
{
    [Header("Item")]
    public GameObject Prefab;
    public Sprite Icon;

    [Header("Stats")]
    public ItemRarity ItemRarity;

    public int SellValue;
    public string Description;
    [Range(0, 100)] public int DropChance;
    public bool IsStackable;
    //public int Quantity;

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
