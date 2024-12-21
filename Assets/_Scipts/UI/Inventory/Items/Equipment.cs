using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "ScriptableObjects/Inventory/Equipment")]
public class Equipment : Item
{
    [Header("Index")]
    public int itemIndex; // Used to determine what Equipment to place on the character

    [Header("Equipment")]
    public EquipmentType equipmentType;

    [Header("Modifiers")]
    public int healthModifier;
    public int damageModifier;

    public override void Use(Inventory _inventory, EquipmentManager _equipmentManager)
    {
        _equipmentManager.Equip(this);

        RemoveFromInventory(_inventory);
    }
}

public enum EquipmentType
{
    Head,
    Chest,
    Legs,
    Finger,
    Neck,
    Weapon,
    Shoulder,
    Back
}
