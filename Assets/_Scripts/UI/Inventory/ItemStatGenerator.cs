using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ItemStatGenerator : NetworkBehaviour
{
    public Item Item;
    public NetworkVariable<int> net_Quantity = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<ItemRarity> net_ItemRarity = new NetworkVariable<ItemRarity>(ItemRarity.Common, NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);
    public NetworkVariable<ItemQuality> net_ItemQuality = new NetworkVariable<ItemQuality>(ItemQuality.Normal, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public List<StatModifier> RolledModifiers = new List<StatModifier>();

    public void RollStats()
    {
        if (!IsServer) return;
        if (Item == null) return;

        // If it's equipment, ensure modifiers are rolled.
        if (Item is Equipment)
        {
            // Create a temp slot representing the pickup current state
            InventorySlotData temp = new InventorySlotData(Item, net_Quantity.Value, net_ItemRarity.Value, net_ItemQuality.Value, RolledModifiers);

            // EnsureRolled will no-op if already rolled
            EnsureRolled(temp);

            // Apply rolled results back onto the pickup (these fields will be serialized if done before Spawn)
            net_ItemRarity.Value = temp.rarity;
            net_ItemQuality.Value = temp.quality;
            RolledModifiers = temp.modifiers;
        }
        else
        {
            // Ensure non-equipment items have defaults set
            if (net_ItemRarity.Value == 0) net_ItemRarity.Value = Item.ItemRarity;
            if (net_ItemQuality.Value == 0) net_ItemQuality.Value = Item.ItemQuality;
        }
    }


    // Ensure the InventorySlotData has been rolled. No-op if already rolled.
    public void EnsureRolled(InventorySlotData slot)
    {
        if (slot == null) return;
        if (slot.modifiers != null && slot.modifiers.Count > 0) return; // already rolled

        // Ensure rarity/quality defaults from template
        if (slot.rarity == 0) slot.rarity = slot.item.ItemRarity;
        if (slot.quality == 0) slot.quality = slot.item.ItemQuality;

        // Compute budget (replace with your formula)
        int budget = ComputeBudget(slot);

        // Store budget so it persists / can be saved
        // (You may prefer a seed instead for deterministic re-rolls)
        // slot.Budget = budget; // add Budget to InventorySlotData if desired

        // Simple placeholder roll - replace with your mod selection logic
        slot.modifiers = new List<StatModifier>
        {
            new StatModifier { statType = StatType.Damage, value = Mathf.Max(1, budget / 2), source = ModSource.Equipment }
        };

        Debug.Log($"ItemStatGenerator: Rolled {slot.item.name} budget={budget} mods={slot.modifiers.Count}");
    }

    int ComputeBudget(InventorySlotData slot)
    {
        // Placeholder: use your level/rarity/quality table. Example starting point:
        // map rarity/quality to offsets and compose with item level requirement.
        // Return an deterministically derived integer budget.
        int baseValue = 1;
        baseValue += (int)slot.rarity * 4;
        baseValue += (int)slot.quality;
        return baseValue;
    }
}
