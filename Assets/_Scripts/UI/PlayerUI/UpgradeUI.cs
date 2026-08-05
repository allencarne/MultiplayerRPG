using System;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    // References
    [SerializeField] UpgradeSlot slot;
    [SerializeField] PlayerStats playerStats;
    [SerializeField] Inventory inventory;

    // Slot
    [SerializeField] Image image_background;
    [SerializeField] Image image_icon;

    // Currency
    [SerializeField] Image image_coinIcon;
    [SerializeField] Image image_collectableIcon;

    // Text
    [SerializeField] TextMeshProUGUI text_name;
    [SerializeField] TextMeshProUGUI text_description;
    [SerializeField] TextMeshProUGUI text_available;
    [SerializeField] TextMeshProUGUI text_coinCost;
    [SerializeField] TextMeshProUGUI text_collectableCost;
    [SerializeField] TextMeshProUGUI[] text_statLines;

    // Buttons
    [SerializeField] Button[] button_statPlus;
    [SerializeField] Button[] button_statMinus;
    [SerializeField] Button button_apply;

    // Stat
    int[] statToAdd = new int[4];
    [HideInInspector] public int pendingCoinTotal;
    [HideInInspector] public int pendingCollectableTotal;

    public void AssignIconUI(InventorySlotData slotData)
    {
        image_icon.sprite = slotData.item.Icon;
        image_background.color = slotData.item.GetRarityColor(slotData.rarity);
    }

    public void AssignNameUI(InventorySlotData slotData)
    {
        text_name.text = FormatNameWithRarity(slotData);
    }

    public void AssignStatsUI(InventorySlotData slotData)
    {
        for (int i = 0; i < text_statLines.Length; i++)
        {
            if (i < slotData.modifiers.Count)
            {
                StatModifier modifier = slotData.modifiers[i];
                text_statLines[i].transform.parent.gameObject.SetActive(true);
                text_statLines[i].text = $"{modifier.statType}: {modifier.value}";
            }
        }
    }

    public void AssignDataUI(InventorySlotData slotData)
    {
        text_description.text = FormatDescription(slotData);
    }

    public void AssignButtonsUI(InventorySlotData slotData)
    {
        // Hide Minus buttons - so we cannot decrease points before adding them
        for (int i = 0; i < button_statMinus.Length; i++)
        {
            button_statMinus[i].gameObject.SetActive(false);
        }

        // Hide Apply Button
        button_apply.gameObject.SetActive(false);

        // Hide Plus Buttons - Enable as needed
        for (int i = 0; i < button_statPlus.Length; i++)
        {
            if (i < slot.upgradeSlotData.modifiers.Count)
            {
                button_statPlus[i].gameObject.SetActive(true);
            }
        }
    }

    bool HasEnoughCoins()
    {
        return playerStats.Coins >= pendingCoinTotal;
    }

    bool HasEnoughCollectables()
    {
        int collectableAmount = inventory.GetItemQuantity(slot.currentCollectable.ITEM_ID);
        return collectableAmount >= pendingCollectableTotal;
    }

    int GetTotalStagedSteps()
    {
        int total = 0;

        // Loop through every stat to add
        for (int i = 0; i < statToAdd.Length; i++)
        {
            // add to the total
            total += statToAdd[i];
        }

        // return the total stats to add
        return total;
    }

    public void RefreshUpgradeButtons()
    {
        // Nothing to refresh if no item has been dropped
        if (slot.upgradeSlotData == null) return;

        // Cast Item as Equipment
        Equipment equipment = (Equipment)slot.upgradeSlotData.item;

        // Figure out how many upgrades have been staged
        int stagedSteps = GetTotalStagedSteps();

        // Determine what quality the item WOULD become if applied.
        int simulatedQuality = (int)slot.upgradeSlotData.quality + stagedSteps;

        // Calculate the TOTAL pending cost
        CalculatePendingCosts();

        if (stagedSteps == 0)
        {
            image_coinIcon.gameObject.SetActive(false);
            image_collectableIcon.gameObject.SetActive(false);

            text_coinCost.text = "";
            text_collectableCost.text = "";

            button_apply.gameObject.SetActive(false);

            // Check if we reached max quality
            bool reachedMaxQuality = simulatedQuality >= Enum.GetNames(typeof(ItemQuality)).Length - 1;

            // Show Plus buttons for every stat line.
            for (int i = 0; i < button_statPlus.Length; i++)
            {
                bool hasModifier = i < slot.upgradeSlotData.modifiers.Count;
                button_statPlus[i].gameObject.SetActive(hasModifier && !reachedMaxQuality);

                // No staged upgrades means NO minus buttons.
                button_statMinus[i].gameObject.SetActive(false);
            }

            return;
        }

        // Show the accumulated cost.
        image_coinIcon.gameObject.SetActive(true);
        image_collectableIcon.gameObject.SetActive(true);
        text_coinCost.text = $"Coins: {pendingCoinTotal}";
        text_collectableCost.text = $"{slot.currentCollectable.name}: {pendingCollectableTotal}";

        // Determine affordability.
        bool canAfford = HasEnoughCoins() && HasEnoughCollectables();
        text_coinCost.color = HasEnoughCoins() ? Color.white : Color.red;
        text_collectableCost.color = HasEnoughCollectables() ? Color.white : Color.red;

        // Only visible if the player can afford
        button_apply.gameObject.SetActive(canAfford);

        // Check if we reached max quality
        bool maxQualityReached = simulatedQuality >= Enum.GetNames(typeof(ItemQuality)).Length - 1;

        // Handle Plus Buttons
        for (int i = 0; i < button_statPlus.Length; i++)
        {
            // hasModifier checks if the current index has a corresponding modifier in the upgradeSlotData. If it does, the plus button can be shown based on affordability and max quality.
            bool hasModifier = i < slot.upgradeSlotData.modifiers.Count;

            // show/hide the plus button based on affordability and max quality
            button_statPlus[i].gameObject.SetActive(hasModifier && canAfford && !maxQualityReached);
        }

        // Handle Minus Buttons
        for (int i = 0; i < button_statMinus.Length; i++)
        {
            button_statMinus[i].gameObject.SetActive(statToAdd[i] > 0);
        }
    }

    void CalculatePendingCosts()
    {
        // Reset the totals before recalculating.
        pendingCoinTotal = 0;
        pendingCollectableTotal = 0;

        Equipment equipment = (Equipment)slot.upgradeSlotData.item;

        // Start from the item's CURRENT quality.
        ItemQuality quality = slot.upgradeSlotData.quality;

        // How many upgrades has the player staged?
        int stagedSteps = GetTotalStagedSteps();

        // Calculate the price of EVERY staged upgrade.
        for (int i = 0; i < stagedSteps; i++)
        {
            slot.CalculateCost(equipment.LevelRequirement, quality);

            pendingCoinTotal += slot.coinCost;
            pendingCollectableTotal += slot.collectableCost;

            // Move to the next quality so the following loop calculates
            // the next upgrade's price.
            quality++;
        }

        // The collectable used is based on the FINAL staged quality.
        slot.CalculateCost(equipment.LevelRequirement, quality);
    }

    string FormatDescription(InventorySlotData slotData)
    {
        StringBuilder sb = new();
        sb.AppendLine(FormatNameWithRarity(slotData));
        sb.AppendLine(FormatNameWithQuality(slotData.quality));
        return sb.ToString();
    }

    string FormatNameWithRarity(InventorySlotData slotData)
    {
        // Get Rarity Color
        Color color = slotData.item.GetRarityColor(slotData.rarity);

        // Convert the Color to a hex string
        string colorHex = ColorUtility.ToHtmlStringRGB(color);

        // Format the name with the appropriate color using rich text
        return $"<color=#{colorHex}><b>{slotData.item.name}</b></color>";
    }

    string FormatNameWithQuality(ItemQuality quality)
    {
        Color color = quality switch
        {
            ItemQuality.Normal => Color.white,
            ItemQuality.Good => new Color32(120, 200, 120, 255),
            ItemQuality.Great => new Color32(100, 170, 255, 255),
            ItemQuality.Excellent => new Color32(255, 215, 100, 255),
            _ => Color.white
        };

        // Convert the Color to a hex string
        string hex = ColorUtility.ToHtmlStringRGB(color);

        // Format the name with the appropriate color using rich text
        return $"<color=#{hex}><b>{quality}</b></color>";
    }

    public void IncreaseStat(int index)
    {
        int simulatedQuality = (int)slot.upgradeSlotData.quality + GetTotalStagedSteps();
        if (simulatedQuality >= Enum.GetNames(typeof(ItemQuality)).Length - 1) return;

        // Increase stat by 1
        statToAdd[index] += 1;

        // Update UI
        StatModifier modifier = slot.upgradeSlotData.modifiers[index];
        text_statLines[index].text = $"{modifier.statType}: {modifier.value + statToAdd[index]}";

        // Button
        button_statMinus[index].gameObject.SetActive(true);

        RefreshUpgradeButtons();
    }

    public void DecreaseStat(int index)
    {
        // Decrease stat by 1
        statToAdd[index] -= 1;

        // Update UI
        StatModifier modifier = slot.upgradeSlotData.modifiers[index];
        text_statLines[index].text = $"{modifier.statType}: {modifier.value + statToAdd[index]}";

        // Button
        if (statToAdd[index] <= 0)
        {
            button_statMinus[index].gameObject.SetActive(false);
        }

        RefreshUpgradeButtons();
    }

    public void ApplyButton()
    {
        // Spend Resources
        inventory.CoinSpent(pendingCoinTotal);
        inventory.RemoveItemByID(slot.currentCollectable.ITEM_ID, pendingCollectableTotal);

        // Increase Quality
        slot.upgradeSlotData.quality = (ItemQuality)((int)slot.upgradeSlotData.quality + GetTotalStagedSteps());

        // Apply Stat Points
        for (int i = 0; i < statToAdd.Length; i++)
        {
            if (i < slot.upgradeSlotData.modifiers.Count)
            {
                StatModifier modifier = slot.upgradeSlotData.modifiers[i];
                modifier.value += statToAdd[i];
                slot.upgradeSlotData.modifiers[i] = modifier;
            }
        }

        // Clear Staged Stats
        ResetUpgradeState();

        // Update UI
        slot.UpdateUI();
    }

    public void ResetUpgradeState()
    {
        Array.Clear(statToAdd, 0, statToAdd.Length);
        pendingCoinTotal = 0;
        pendingCollectableTotal = 0;
    }

    public void CloseUpgradeUI()
    {
        slot.ReturnItemAndClear();

        image_icon.sprite = null;
        text_name.text = "Drop Equuipment to Upgrade";
        text_name.color = Color.grey;
        image_background.color = Color.white;
        text_description.text = "";
        text_available.text = "";

        image_coinIcon.gameObject.SetActive(false);
        image_collectableIcon.gameObject.SetActive(false);

        text_coinCost.text = "";
        text_coinCost.color = Color.white;

        text_collectableCost.text = "";
        text_collectableCost.color = Color.white;

        ResetUpgradeState();

        foreach (TextMeshProUGUI statLine in text_statLines)
        {
            statLine.text = "";
            statLine.transform.parent.gameObject.SetActive(false);
        }

        slot.ClearSlot();
    }
}
