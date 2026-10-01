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
        if (_inventory == null)
        {
            Debug.LogWarning("Consumable.Use called but inventory is null.");
            return;
        }

        var stats = _inventory.Stats;
        if (stats == null)
        {
            Debug.LogWarning("Consumable.Use: Inventory owner has no PlayerStats.");
            return;
        }

        switch (Action)
        {
            case ConsumableAction.HealthRegen:
                {
                    Buffs buffs = stats.GetComponent<Buffs>();
                    if (buffs != null && buffs.regeneration != null)
                    {
                        buffs.regeneration.StartBuff(Stacks, Duration);
                        Debug.Log($"Consumable: applied Health Regen ({Stacks} stacks, {Duration}s) to {stats.name}");
                    }
                    else
                    {
                        Debug.LogWarning("Consumable: no Buffs or Buff_Regeneration found on target.");
                    }
                    break;
                }

            case ConsumableAction.ManaRegen:
                {
                    Buffs buffs = stats.GetComponent<Buffs>();
                    if (buffs != null && buffs.replenishment != null)
                    {
                        buffs.replenishment.StartBuff(Stacks, Duration);
                        Debug.Log($"Consumable: applied Mana Replenishment ({Stacks} stacks, {Duration}s) to {stats.name}");
                    }
                    else
                    {
                        Debug.LogWarning("Consumable: no Buffs or Buff_Replenishment found on target.");
                    }
                    break;
                }

            case ConsumableAction.EnduranceRegen:
                {
                    Buffs buffs = stats.GetComponent<Buffs>();
                    if (buffs != null && buffs.resurgence != null)
                    {
                        buffs.resurgence.StartBuff(Stacks, Duration);
                        Debug.Log($"Consumable: applied Endurance Resurgence ({Stacks} stacks, {Duration}s) to {stats.name}");
                    }
                    else
                    {
                        Debug.LogWarning("Consumable: no Buffs or Buff_Resurgence found on target.");
                    }
                    break;
                }

            case ConsumableAction.Buff_Might:
                {
                    Buffs buffs = stats.GetComponent<Buffs>();
                    if (buffs != null && buffs.might != null)
                    {
                        buffs.might.StartMight(Stacks, Duration);
                        Debug.Log($"Consumable: applied Might ({Stacks} stacks, {Duration}s) to {stats.name}");
                    }
                    else
                    {
                        Debug.LogWarning("Consumable: no Buffs or Buff_Might found on target.");
                    }
                    break;
                }

            default:
                Debug.LogWarning("Consumable.Use: unknown action.");
                break;
        }

        // Remove one from inventory (safe removal by ID)
        _inventory.RemoveItemByID(ITEM_ID, 1);
    }
}
