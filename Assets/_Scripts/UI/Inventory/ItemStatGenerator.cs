using System.Collections.Generic;
using Unity.Netcode;

public class ItemStatGenerator : NetworkBehaviour
{
    public Item Item;
    public NetworkVariable<int> net_Quantity = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<ItemRarity> net_ItemRarity = new NetworkVariable<ItemRarity>(ItemRarity.Common, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<ItemQuality> net_ItemQuality = new NetworkVariable<ItemQuality>(ItemQuality.Normal, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkList<StatModifier> net_RolledModifiers = new NetworkList<StatModifier>();

    public override void OnDestroy()
    {
        // Release the NetworkList when this object is destroyed
        net_RolledModifiers.Dispose();

        // Let NetworkBehaviour clean itself up
        base.OnDestroy();
    }

    public void RollStats()
    {
        // Only the server is allowed to generate item stats
        if (!IsServer) return;

        // Don't continue if there isn't an item assigned
        if (Item == null) return;

        // Equipment is randomly rolled
        if (Item is Equipment)
        {
            // Create a temporary item slot using the item's current data
            // This lets us modify everything in one place before writing it back
            InventorySlotData temp = new InventorySlotData(Item, net_Quantity.Value, net_ItemRarity.Value, net_ItemQuality.Value, GetRolledModifiers());

            // Hand the actual rolling off to the shared rules asset
            Item.ItemStatRules.RollStats(temp);

            // Copy the generated values back into the network variables
            net_ItemRarity.Value = temp.rarity;
            net_ItemQuality.Value = temp.quality;

            // Copy the generated modifiers into the NetworkList
            SetRolledModifiers(temp.modifiers);
        }
        else
        {
            // Non-equipment items don't roll random stats

            // If no rarity has been assigned yet, use the item's default rarity
            if (net_ItemRarity.Value == 0) net_ItemRarity.Value = Item.ItemRarity;

            // If no quality has been assigned yet, use the item's default quality
            if (net_ItemQuality.Value == 0) net_ItemQuality.Value = Item.ItemQuality;
        }
    }

    public void SetRolledModifiers(List<StatModifier> mods)
    {
        // Only the server can modify NetworkLists
        if (!IsServer) return;

        // Remove any previous modifiers
        net_RolledModifiers.Clear();

        // Nothing to copy
        if (mods == null) return;

        // Copy each modifier into the NetworkList
        foreach (StatModifier mod in mods) net_RolledModifiers.Add(mod);
    }

    public List<StatModifier> GetRolledModifiers()
    {
        // Create a normal List with the same capacity
        List<StatModifier> mods = new List<StatModifier>(net_RolledModifiers.Count);

        // Copy every modifier from the NetworkList
        foreach (StatModifier mod in net_RolledModifiers) mods.Add(mod);

        // Return a standard List instead of the NetworkList
        return mods;
    }
}
