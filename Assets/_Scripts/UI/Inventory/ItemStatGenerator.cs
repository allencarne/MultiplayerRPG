using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ItemStatGenerator : NetworkBehaviour
{
    public Item Item;
    public NetworkVariable<int> net_Quantity = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<ItemRarity> net_ItemRarity = new NetworkVariable<ItemRarity>(ItemRarity.Common, NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);
    public NetworkVariable<ItemQuality> net_ItemQuality = new NetworkVariable<ItemQuality>(ItemQuality.Normal, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkList<StatModifier> net_RolledModifiers = new NetworkList<StatModifier>();

    int[] LevelBreakpoints = { 1, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80 };

    public void RollStats()
    {
        if (!IsServer) return;
        if (Item == null) return;

        // If it's equipment, ensure modifiers are rolled.
        if (Item is Equipment)
        {
            // Create a temp slot representing the pickup current state
            InventorySlotData temp = new InventorySlotData(Item, net_Quantity.Value, net_ItemRarity.Value, net_ItemQuality.Value, GetRolledModifiers());

            // EnsureRolled will no-op if already rolled
            EnsureRolled(temp);

            // Apply rolled results back onto the pickup (these fields will be serialized if done before Spawn)
            net_ItemRarity.Value = temp.rarity;
            net_ItemQuality.Value = temp.quality;
            SetRolledModifiers(temp.modifiers);
        }
        else
        {
            // Ensure non-equipment items have defaults set
            if (net_ItemRarity.Value == 0) net_ItemRarity.Value = Item.ItemRarity;
            if (net_ItemQuality.Value == 0) net_ItemQuality.Value = Item.ItemQuality;
        }
    }

    void EnsureRolled(InventorySlotData slot)
    {
        // Ensure the InventorySlotData has been rolled. No-op if already rolled.

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
        // Equipment is the only Item subtype with a level requirement.
        Equipment equipment = slot.item as Equipment;
        if (equipment == null)
        {
            Debug.LogWarning($"ComputeBudget called on non-equipment item: {slot.item?.name}");
            return 1;
        }

        int baseOffset = GetBaseOffsetForLevel(equipment.LevelRequirement);
        int rarityOffset = (int)slot.rarity * 4;
        int qualityOffset = (int)slot.quality;

        return baseOffset + rarityOffset + qualityOffset + 1;
    }

    public void SetRolledModifiers(List<StatModifier> mods)
    {
        // Server-only: replace the synced modifier list.
        if (!IsServer) return;

        net_RolledModifiers.Clear();
        if (mods == null) return;
        foreach (StatModifier mod in mods) net_RolledModifiers.Add(mod);
    }

    public List<StatModifier> GetRolledModifiers()
    {
        // Safe to call on either server or client — just reads the synced list.

        List<StatModifier> mods = new List<StatModifier>(net_RolledModifiers.Count);
        foreach (StatModifier mod in net_RolledModifiers) mods.Add(mod);
        return mods;
    }

    public override void OnDestroy()
    {
        // NetworkList holds native memory that must be released
        net_RolledModifiers.Dispose();
        base.OnDestroy();
    }

    int GetMaxRarityIndexForLevel(int level)
    {
        // Returns the HIGHEST rarity index unlocked at the given level.
        // Because the enum order matches the unlock order, we can just return the enum value.

        if (level <= 15) return (int)ItemRarity.Uncommon;  // 1
        if (level <= 35) return (int)ItemRarity.Rare;       // 2
        if (level <= 55) return (int)ItemRarity.Epic;       // 3
        if (level <= 75) return (int)ItemRarity.Exotic;     // 4
        return (int)ItemRarity.Legendary;                   // 7 (level 80 endgame)
    }

    int GetBaseOffsetForLevel(int itemLevel)
    {
        // Sums up how many budget points were "used up" by every level bracket
        // BEFORE this one. This is the only genuinely cumulative part of the formula.

        int offset = 0;
        foreach (int breakpoint in LevelBreakpoints)
        {
            if (breakpoint >= itemLevel) break; // stop once we reach the item's own level

            int rarityCountAtThisLevel = GetMaxRarityIndexForLevel(breakpoint) + 1; // +1 because index is 0-based
            offset += rarityCountAtThisLevel * 4; // 4 quality steps per rarity
        }
        return offset;
    }

}
