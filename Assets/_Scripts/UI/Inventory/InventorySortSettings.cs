using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InventorySortSettings", menuName = "Scriptable Objects/InventorySortSettings")]
public class InventorySortSettings : ScriptableObject
{
    [Serializable]
    public class CategoryPriority
    {
        //public ItemCategory category;
        public int priority;
    }

    [Serializable]
    public class EquipmentPriority
    {
        public EquipmentType type;
        public int priority;
    }

    [Header("Category Order")]
    public List<CategoryPriority> CategoryPriorities = new();

    [Header("Equipment Order")]
    public List<EquipmentPriority> EquipmentPriorities = new();

    public int GetCategoryPriority(Item item)
    {
        foreach (var p in CategoryPriorities)
        {
            /*
            if (p.category == item.Category)
                return p.priority;
            */
        }

        return int.MaxValue;
    }

    public int GetEquipmentPriority(Equipment equipment)
    {
        foreach (var p in EquipmentPriorities)
        {
            if (p.type == equipment.equipmentType)
                return p.priority;
        }

        return int.MaxValue;
    }
}
