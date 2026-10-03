using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Item/Item Type/Consumable")]
public class Consumable : Item
{
    public enum ConsumableAction
    {
        HealthRegen,
        ManaRegen,
        EnduranceRegen,
        Buff_Might
    }

    [Header("Consumable Slot")]
    public ConsumableSlotType SlotType = ConsumableSlotType.Potion;

    [Header("Consumable Behaviour")]
    public ConsumableAction Action = ConsumableAction.HealthRegen;

    [Tooltip("Stacks applied for stackable buffs (e.g. regen or might)")]
    public int Stacks = 1;

    [Tooltip("Duration in seconds for duration-based buffs. Use negative duration for fixed stacks where supported.")]
    public float Duration = 5f;

    [Tooltip("Flat amount for instant restores (mana / endurance)")]
    public float Amount = 0f;

    public override void Use(Inventory _inventory, EquipmentManager _equipmentManager, InventorySlotData slotData)
    {
        _equipmentManager.ConsumableSlots.Equip(slotData);
    }

    public bool Apply(PlayerStats stats)
    {
        Buffs buffs = stats.GetComponent<Buffs>();
        if (buffs == null)
        {
            Debug.LogWarning("Consumable.Apply: no Buffs found on target.");
            return false;
        }

        switch (Action)
        {
            case ConsumableAction.HealthRegen:
                if (buffs.regeneration == null) return false;
                buffs.regeneration.StartBuff(Stacks, Duration);
                return true;

            case ConsumableAction.ManaRegen:
                if (buffs.replenishment == null) return false;
                buffs.replenishment.StartBuff(Stacks, Duration);
                return true;

            case ConsumableAction.EnduranceRegen:
                if (buffs.resurgence == null) return false;
                buffs.resurgence.StartBuff(Stacks, Duration);
                return true;

            case ConsumableAction.Buff_Might:
                if (buffs.might == null) return false;
                buffs.might.StartMight(Stacks, Duration);
                return true;
        }

        return false;
    }
}

public enum ConsumableSlotType
{
    Potion,
    Food
}
