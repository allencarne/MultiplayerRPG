using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Scriptable Objects/Item/Equipment")]
public class Equipment : Item
{
    [Header("Index")]
    public int AnimationIndex; // Used to determine what Equipment (Not Weapon) to place on the character

    [Header("Equipment")]
    public int LevelRequirement;
    public EquipmentType equipmentType;
    public ClassRequirement ClassRequirement;

    [Header("Modifiers")]
    public List<StatModifier> modifiers = new List<StatModifier>();

    public override void Use(Inventory _inventory, EquipmentManager _equipmentManager, InventorySlotData slotData)
    {
        _equipmentManager.Equip(slotData);
        RemoveFromInventory(_inventory);
    }

    public bool IsUnderLevelRequirement(PlayerStats stats)
    {
        return stats.PlayerLevel.Value < LevelRequirement;
    }

    public bool IsWrongClass(PlayerStats stats)
    {
        if (ClassRequirement == ClassRequirement.None) return false;

        return stats.playerClass.ToString() != ClassRequirement.ToString();
    }

    public bool CanPlayerUse(PlayerStats stats)
    {
        return !IsUnderLevelRequirement(stats) && !IsWrongClass(stats);
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