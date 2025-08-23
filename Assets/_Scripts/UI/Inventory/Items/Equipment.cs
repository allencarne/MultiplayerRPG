using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Scriptable Objects/Item/Equipment")]
public class Equipment : Item
{
    [Header("Index")]
    public int itemIndex; // Used to determine what Equipment (Not Weapon) to place on the character

    [Header("Stats")]
    public int PurchaseValue;

    [Header("Equipment")]
    public int LevelRequirement;
    public EquipmentType equipmentType;
    public ClassRequirement ClassRequirement;

    [Header("Modifiers")]
    public List<StatModifier> modifiers = new List<StatModifier>();

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

public enum ClassRequirement
{
    None,
    Beginner,
    Warrior,
    Magician,
    Archer,
    Rogue
}