using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Item/ItemStatRules")]
public class ItemStatRules : ScriptableObject
{
    int[] LevelBreakpoints = { 1, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80 };

    [Header("Roll Weighting")]
    [Range(0.01f, 0.99f)]
    public float rarityDecayFactor;
    [Range(0.01f, 0.99f)]
    public float qualityDecayFactor;

    [Header("Stat Line Roll Weighting")]
    [Range(0.1f, 0.9f)]
    public float statLineDecayMin;
    [Range(0.1f, 0.9f)]
    public float statLineDecayMax;

    [Header("Primary Stat Weighting")]
    [Tooltip("Percentage of the total budget allocated to the primary stat based on the number of stat lines.")]
    [Range(0f, 1f)]
    public float primaryShare1Line;
    [Range(0f, 1f)]
    public float primaryShare2Lines;
    [Range(0f, 1f)]
    public float primaryShare3Lines;
    [Range(0f, 1f)]
    public float primaryShare4Lines;

    [Header("Percent Stat Scaling")]
    [Tooltip("Fractional percent added per budget point")]
    public float percentPerBudgetPoint;

    [Header("Rate Stat Scaling")]
    [Range(0.01f, 1f)]
    public float rateStatFlatScale;

    readonly StatType[] AllRollableStats = { StatType.Damage, StatType.Armor, StatType.Health, StatType.AttackSpeed, StatType.CoolDown, StatType.Speed, StatType.Vamp, StatType.Mana, StatType.ManaRegen};

    public void RollStats(InventorySlotData slot, float rarityBoost = 0f)
    {
        // Safety Check
        if (slot == null) return;

        // If modifiers already exist, this item has already been rolled
        if (slot.modifiers != null && slot.modifiers.Count > 0) return;

        // Convert the item to Equipment
        Equipment equipment = slot.item as Equipment;

        // If it isn't equipment, there's nothing to roll
        if (equipment == null) return;

        // Randomly determine the rarity based on level
        slot.rarity = RollRarity(equipment.LevelRequirement, rarityBoost);

        // If no quality exist yet, use the item's default quality
        if (slot.quality == 0) slot.quality = slot.item.ItemQuality;

        // Calculate the total stat budget for this item
        int budget = GetBudget(slot);

        // Create the item's rolled modifiers
        float decay = Random.Range(statLineDecayMin, statLineDecayMax);
        slot.modifiers = RollModifiers(equipment, budget, decay, randomizeSecondaryOrder: true);
    }

    public InventorySlotData BuildItemData(Item item)
    {
        // Gets the Mods directly from the item if it's Equipment, otherwise just returns the item with default rarity and quality
        if (item is Equipment equipment)
        {
            List<StatModifier> mods = new List<StatModifier>(equipment.modifiers);
            return new InventorySlotData(item, 1, equipment.ItemRarity, equipment.ItemQuality, mods);
        }

        return new InventorySlotData(item, 1, item.ItemRarity, item.ItemQuality);
    }

    int GetBudget(InventorySlotData slot)
    {
        // Convert the item into Equipment
        Equipment equipment = slot.item as Equipment;

        // Prevent invalid items from continuing
        if (equipment == null) return 1;

        // Calculate how much budget existst from all previous level tiers
        int baseOffset = GetBaseOffsetForLevel(equipment.LevelRequirement);

        // Each rarity increases the budget by 4
        int rarityOffset = (int)slot.rarity * 4;

        // Each quality increases the budget by 1
        int qualityOffset = (int)slot.quality;

        // Final budget
        return baseOffset + rarityOffset + qualityOffset + 1;
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

    int GetStatLineCountForLevel(int level)
    {
        // How many stat lines an item gets, based on it's level
        if (level <= 15) return 1;
        if (level <= 35) return 2;
        if (level <= 55) return 3;
        return 4; // 60-80
    }

    StatType GetPrimaryStatType(EquipmentType type)
    {
        // Determine the primary stat based on the equipment slot
        switch (type)
        {
            // Defensive equipment
            case EquipmentType.Head:
            case EquipmentType.Chest:
            case EquipmentType.Legs:
            case EquipmentType.Neck:
            case EquipmentType.Shoulder:
                return StatType.Armor;

            // Offensive equipment
            case EquipmentType.Finger:
            case EquipmentType.Weapon:
            case EquipmentType.Back:
                return StatType.Damage;

            // Fallback in case a new equpment type hasn't been assigned
            default:
                Debug.LogWarning($"No primary stat mapping for {type}, defaulting to Damage.");
                return StatType.Damage;
        }
    }

    // Builds the pool of eligible (StatType, ModType) combos for the *secondary* lines.
    List<(StatType stat, ModType modType)> BuildSecondaryPool(StatType primaryType)
    {
        // pool will contain every possible (StatType, ModType) combination that can be rolled for secondary lines
        List<(StatType, ModType)> pool = new List<(StatType, ModType)>();

        // Walk through every rollable stat type
        foreach (StatType stat in AllRollableStats)
        {
            // Determine if this stat can ever be a percent line
            bool neverPercent = (stat == StatType.Health || stat == StatType.Armor);

            // Every stat can have a Flat line, except the primary's Flat (already used)
            if (stat != primaryType)
            {
                pool.Add((stat, ModType.Flat));
            }

            // Percent line, if this stat allows one at all
            if (!neverPercent)
            {
                pool.Add((stat, ModType.Percent));
            }
        }

        // Remove the primary stat's percent line from the pool, if it exists
        return pool;
    }

    // Fisher-Yates shuffle algorithm to randomize the order of a list in place
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

    // Splits a total budget into a number of parts, with each part receiving less than the previous one based on a decay factor
    int[] SplitBudget(int budget, int lineCount, float decay)
    {
        // If there's only one stat line, it receive the entire budget
        if (lineCount <= 1) return new int[] { budget };

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
            // Give this stat line it's proportional share of the total budget
            amounts[i] = Mathf.FloorToInt((weights[i] / totalWeight) * budget);

            // Keep track of the total assigned so far
            assigned += amounts[i];
        }

        // Determine how many points were lost due to rounding
        int remainder = budget - assigned;

        // Start redisctibuting from the largest stat line
        int index = 0;

        // Continue until every budget stat point has been assigned
        while (remainder > 0)
        {
            // Give one extra point to the current stat line
            amounts[index]++;

            // One less point remains to distribue
            remainder--;

            // Move to the next stat line, looping back to the start if necessary
            index = (index + 1) % lineCount;
        }

        // Return the completed budget split
        return amounts;
    }

    // Rolls the stat modifiers for a given piece of equipment, based on its budget and decay factor
    List<StatModifier> RollModifiers(Equipment equipment, int budget, float decay, bool randomizeSecondaryOrder)
    {
        // Determine how many stat lines this item should have based on its level
        int lineCount = GetStatLineCountForLevel(equipment.LevelRequirement);

        // Determine the item's primary stat based on its equipment slot
        StatType primaryType = GetPrimaryStatType(equipment.equipmentType);

        // Determine what percentage of the total budget should be allocated to the primary stat based on the number of stat lines
        float primaryShare = GetPrimaryBudgetShare(lineCount);

        // Calculate how much of the total budget should be allocated
        int primaryAmount = Mathf.Clamp(Mathf.RoundToInt(budget * primaryShare), 1, budget);

        // Create the primary stat modifier and add it to the list
        List<StatModifier> modifiers = new List<StatModifier> { CreateModifierFromBudget(primaryType, ModType.Flat, primaryAmount) };

        // Calculate how much budget remains for the secondary stat lines
        int remainingBudget = budget - primaryAmount;

        // Determine how many secondary stat lines should be rolled
        int secondaryLineCount = Mathf.Max(0, lineCount - 1);

        // If there are secondary lines to roll and budget remaining, roll them
        if (secondaryLineCount > 0 && remainingBudget > 0)
        {
            // Build a pool of eligible (StatType, ModType) combinations for the secondary lines
            List<(StatType stat, ModType modType)> pool = BuildSecondaryPool(primaryType);

            // Randomize the order of the pool if requested
            if (randomizeSecondaryOrder) Shuffle(pool);

            // Safety clamp — pool always has more entries than max line count, but just in case
            int actualSecondaryCount = Mathf.Min(secondaryLineCount, pool.Count);

            // Split the remaining budget across the secondary stat lines, with each line receiving less than the previous one based on the decay factor
            int[] amounts = SplitBudget(remainingBudget, actualSecondaryCount, decay);

            // Create each secondary stat modifier and add it to the list
            for (int i = 0; i < actualSecondaryCount; i++)
            {
                // Get the (StatType, ModType) combination for this line
                var combo = pool[i];

                // Create the modifier and add it to the list
                modifiers.Add(CreateModifierFromBudget(combo.stat, combo.modType, amounts[i]));
            }
        }

        return modifiers;
    }

    StatModifier CreateModifierFromBudget(StatType stat, ModType modType, int points)
    {
        if (points <= 0) points = 1;

        return new StatModifier
        {
            statType = stat,
            value = points * GetValuePerPoint(stat, modType),
            source = ModSource.Equipment,
            modType = modType
        };
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

    ItemRarity RollRarity(int itemLevel, float rarityBoost = 0f)
    {
        int maxRarityIndex = GetMaxRarityIndexForLevel(itemLevel);

        // Nudge the decay up, but keep it inside the valid range
        float decay = Mathf.Clamp(rarityDecayFactor + rarityBoost, 0.01f, 0.99f);

        int rolledIndex = WeightedRandomIndex(maxRarityIndex + 1, decay);
        return (ItemRarity)rolledIndex;
    }

    public int CompareBudget(InventorySlotData item, InventorySlotData equipped)
    {
        int itemBudget = GetBudget(item);
        int equippedBudget = GetBudget(equipped);

        return itemBudget.CompareTo(equippedBudget);
    }

    float GetPrimaryBudgetShare(int lineCount)
    {
        switch (lineCount)
        {
            case 1: return primaryShare1Line;
            case 2: return primaryShare2Lines;
            case 3: return primaryShare3Lines;
            case 4: return primaryShare4Lines;
            default: return primaryShare4Lines;
        }
    }

    public float GetValuePerPoint(StatType stat, ModType modType)
    {
        if (modType == ModType.Percent)
        {
            return percentPerBudgetPoint;
        }

        bool isRateStat = (stat == StatType.AttackSpeed || stat == StatType.CoolDown || stat == StatType.Speed || stat == StatType.Vamp);
        return isRateStat ? rateStatFlatScale : 1f;
    }
}
