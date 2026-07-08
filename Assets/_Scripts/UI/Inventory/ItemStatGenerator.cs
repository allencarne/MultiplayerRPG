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

    [Header("Roll Weighting")]
    [Range(0.01f, 0.99f)]
    float rarityDecayFactor = 0.35f;
    [Range(0.01f, 0.99f)]
    float qualityDecayFactor = 0.5f;

    [Header("Stat Line Roll Weighting")]
    [Range(0.1f, 0.9f)]
    float statLineDecayMin = 0.35f;
    [Range(0.1f, 0.9f)]
    float statLineDecayMax = 0.75f;

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

            // If the item hasn't already been rolled, generate it's stats
            EnsureRolled(temp);

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

    void EnsureRolled(InventorySlotData slot)
    {
        // Safety check
        if (slot == null) return;

        // If modifiers already exist, this item has already been rolled
        if (slot.modifiers != null && slot.modifiers.Count > 0) return;

        // Convert the item into Equipment
        Equipment equipment = slot.item as Equipment;

        // If it isn't equipment, there's nothing to roll
        if (equipment == null) return;

        // Randomly determine the rarity based on item level
        slot.rarity = RollRarity(equipment.LevelRequirement);

        // If no quality exists yet, use the item's default quality
        if (slot.quality == 0) slot.quality = slot.item.ItemQuality;

        // Calculate the total stat budget for this item
        int budget = ComputeBudget(slot);

        // Create the item's rolled modifiers.
        slot.modifiers = RollModifiers(equipment, budget);

        // Log the generated result for debugging
        //Debug.Log($"ItemStatGenerator: Rolled {slot.item.name} budget={budget} mods={slot.modifiers.Count}");
    }

    int ComputeBudget(InventorySlotData slot)
    {
        // Convert the item into Equipment
        Equipment equipment = slot.item as Equipment;

        // Prevent invalid items from continuing
        if (equipment == null)
        {
            Debug.LogWarning($"ComputeBudget called on non-equipment item: {slot.item?.name}");
            return 1;
        }

        // Calculate how much budget exists from all previous level tiers
        int baseOffset = GetBaseOffsetForLevel(equipment.LevelRequirement);

        // Each rarity increases the budget by 4
        int rarityOffset = (int)slot.rarity * 4;

        // Each quality increases the budget by 1
        int qualityOffset = (int)slot.quality;

        // Final budget
        return baseOffset + rarityOffset + qualityOffset + 1;
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

    int GetMaxRarityIndexForLevel(int level)
    {
        // Determine the highest rarity that can appear at this level
        if (level <= 15) return (int)ItemRarity.Uncommon;
        if (level <= 35) return (int)ItemRarity.Rare;
        if (level <= 55) return (int)ItemRarity.Epic;
        if (level <= 75) return (int)ItemRarity.Exotic;
        return (int)ItemRarity.Legendary;
    }

    int GetStatLineCountForLevel(int itemLevel)
    {
        // How many stat lines an item gets, based purely on its level
        if (itemLevel <= 15) return 1;
        if (itemLevel <= 35) return 2;
        if (itemLevel <= 55) return 3;
        return 4; // 60-80
    }

    StatType GetPrimaryStatType(EquipmentType type)
    {
        // Determine the primary stat based on the equipment slot.
        switch (type)
        {
            // Defensive equipment primarily grants Health.
            case EquipmentType.Head:
            case EquipmentType.Chest:
            case EquipmentType.Legs:
            case EquipmentType.Neck:
            case EquipmentType.Shoulder:
                return StatType.Health;

            // Offensive equipment primarily grants Damage.
            case EquipmentType.Finger:
            case EquipmentType.Weapon:
            case EquipmentType.Back:
                return StatType.Damage;

            // Fallback in case a new equipment type hasn't been assigned yet.
            default:
                Debug.LogWarning($"No primary stat mapping for {type}, defaulting to Damage.");
                return StatType.Damage;
        }
    }

    void Shuffle<T>(List<T> list)
    {
        // Walk backward through the list
        for (int i = list.Count - 1; i > 0; i--)
        {
            // Pick a random element from the unshuffled portion
            int j = Random.Range(0, i + 1);

            // Swap the current element with the randomly selected one
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    int[] SplitBudget(int budget, int lineCount)
    {
        // If there's only one stat line, it receives the entire budget
        if (lineCount <= 1) return new int[] { budget };

        // Randomly choose a decay value so each item distributes stats a little differently
        float decay = Random.Range(statLineDecayMin, statLineDecayMax);

        // Store the weight assigned to each stat line
        float[] weights = new float[lineCount];

        // Running total of all weights
        float totalWeight = 0f;

        // Calculate each weight
        for (int i = 0; i < lineCount; i++)
        {
            // Each successive stat line receives less weight than the previous one
            weights[i] = Mathf.Pow(decay, i);

            // Add this weight into the total
            totalWeight += weights[i];
        }

        // Store the final budget assigned to each stat line
        int[] amounts = new int[lineCount];

        // Keep track of how many budget points have been assigned
        int assigned = 0;

        // Convert each weight into an integer amount
        for (int i = 0; i < lineCount; i++)
        {
            // Give this stat line its proportional share of the total budget
            amounts[i] = Mathf.FloorToInt((weights[i] / totalWeight) * budget);

            // Keep track of the total assigned so far
            assigned += amounts[i];
        }

        // Determine how many points were lost due to rounding
        int remainder = budget - assigned;

        // Start redistributing from the largest stat line
        int index = 0;

        // Continue until every budget point has been assigned
        while (remainder > 0)
        {
            // Give one extra point to the current stat line
            amounts[index]++;

            // One less point remains to distribute
            remainder--;

            // Move to the next stat line, looping back to the start if necessary
            index = (index + 1) % lineCount;
        }

        // Return the completed budget split
        return amounts;
    }

    List<StatModifier> RollModifiers(Equipment equipment, int budget)
    {
        // Determine how many stat lines this item should have based on its level
        int lineCount = GetStatLineCountForLevel(equipment.LevelRequirement);

        // Determine the item's primary stat based on its equipment slot
        StatType primaryType = GetPrimaryStatType(equipment.equipmentType);

        // Create a pool containing every possible stat type
        List<StatType> remainingPool = new List<StatType> { StatType.Damage, StatType.Health, StatType.AttackSpeed, StatType.CoolDown };

        // Remove the primary stat so it can't be selected twice
        remainingPool.Remove(primaryType);

        // Randomize the remaining stat types
        Shuffle(remainingPool);

        // Start the chosen stat list with the guaranteed primary stat
        List<StatType> chosenTypes = new List<StatType> { primaryType };

        // Fill the remaining stat lines using the shuffled pool
        for (int i = 0; i < lineCount - 1 && i < remainingPool.Count; i++)
        {
            // Add one random secondary stat
            chosenTypes.Add(remainingPool[i]);
        }

        // Divide the total budget across all chosen stat lines
        // The first entry (primary stat) will always receive the largest share
        int[] amounts = SplitBudget(budget, chosenTypes.Count);

        // Create the final list of stat modifiers.
        List<StatModifier> modifiers = new List<StatModifier>();

        // Pair each stat type with its allocated budget
        for (int i = 0; i < chosenTypes.Count; i++)
        {
            modifiers.Add(new StatModifier
            {
                // Which stat this modifier affects
                statType = chosenTypes[i],

                // How many points this stat receives
                value = amounts[i],

                // Mark this modifier as coming from equipment
                source = ModSource.Equipment
            });
        }

        // Return the completed modifier list
        return modifiers;
    }

    int GetBaseOffsetForLevel(int itemLevel)
    {
        // Start with no accumulated budget
        int offset = 0;

        // Walk through every level breakpoint
        foreach (int breakpoint in LevelBreakpoints)
        {
            // Stop once we've reached the current item's level
            if (breakpoint >= itemLevel) break;

            // Determine how many rarities exist at this level
            int rarityCountAtThisLevel = GetMaxRarityIndexForLevel(breakpoint) + 1;

            // Each rarity has four quality levels
            // Add every possible budget from this level tier
            offset += rarityCountAtThisLevel * 4;
        }
        return offset;
    }

    int WeightedRandomIndex(int optionCount, float decayFactor)
    {
        // If there's only one option, always return it
        if (optionCount <= 1) return 0;

        // Store each option's weight
        float[] weights = new float[optionCount];

        // Sum of all weights
        float totalWeight = 0f;

        // Calculate weights
        for (int i = 0; i < optionCount; i++)
        {
            // Each option becomes less likely than the previous
            weights[i] = Mathf.Pow(decayFactor, i);

            // Keep track of the total weight
            totalWeight += weights[i];
        }

        // Roll somewhere within the total weight
        float roll = Random.Range(0f, totalWeight);

        // Running total while searching
        float cumulative = 0f;

        // Find which weight the roll landed inside
        for (int i = 0; i < optionCount; i++)
        {
            cumulative += weights[i];
            if (roll <= cumulative) return i;
        }

        // Fallback in case of floating point rounding
        return optionCount - 1;
    }

    ItemRarity RollRarity(int itemLevel)
    {
        // Determine the highest rarity this level can roll
        int maxRarityIndex = GetMaxRarityIndexForLevel(itemLevel);

        // Randomly choose one using weighted odds
        int rolledIndex = WeightedRandomIndex(maxRarityIndex + 1, rarityDecayFactor);

        // Convert the integer back into the ItemRarity enum
        return (ItemRarity)rolledIndex;
    }

}
