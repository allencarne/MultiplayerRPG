using System.Text;
using TMPro;
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
    int[] statToAdd = new int[3];

    public void AssignIcon(InventorySlotData slotData)
    {
        image_icon.sprite = slotData.item.Icon;
        image_background.color = slotData.item.GetRarityColor(slotData.rarity);
    }

    public void AssignName(InventorySlotData slotData)
    {
        text_name.text = FormatNameWithRarity(slotData);
    }

    public void AssignStats(InventorySlotData slotData)
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

    public void AssignData(InventorySlotData slotData)
    {
        text_description.text = FormatDescription(slotData);
    }

    public void AssignCost(InventorySlotData slotData)
    {
        // Cast Item as Equipment
        Equipment equipment = (Equipment)slotData.item;

        // Assign the Coin and Collectable Cost
        slot.CalculateCost(equipment.LevelRequirement, equipment.ItemQuality);

        // Set Coin Cost Text
        text_coinCost.text = $"Coins: {slot.coinCost}";

        // Set Collectable Cost Text
        text_collectableCost.text = $"Collectable: {slot.collectableCost}";

        image_coinIcon.gameObject.SetActive(true);
        image_collectableIcon.gameObject.SetActive(true);

        CalculateAvaliableUpgrades(slotData);
    }

    public void AssignButtons(InventorySlotData slotData)
    {
        // Hide Minus Buttons - so we cannot decrease points before adding them
        for (int i = 0; i < button_statMinus.Length; i++)
        {
            button_statMinus[i].gameObject.SetActive(false);
        }

        // Hide Apply Button
        button_apply.gameObject.SetActive(false);
    }

    void CalculateAvaliableUpgrades(InventorySlotData slotData)
    {
        int avaliableUpgrades = 0;
        bool enoughCoins = false;
        bool enouchCollectables = false;

        // Check if we have enough coins
        if (playerStats.Coins < slot.coinCost)
        {
            // not enough coins
            text_coinCost.color = Color.red;
            enoughCoins = false;
        }
        else
        {
            // enough coins
            text_coinCost.color = Color.white;
            enoughCoins = true;
        }

        // Check if we have enough collectable
        int collectableAmount = inventory.GetItemQuantity(slot.currentCollectable.ITEM_ID);
        if (collectableAmount < slot.collectableCost)
        {
            // not enough collectable
            text_collectableCost.color = Color.red;
            enouchCollectables = false;
        }
        else
        {
            // enough collectable
            text_collectableCost.color = Color.white;
            enouchCollectables = true;

        }

        if (enoughCoins && enouchCollectables)
        {
            // Enable Plus Button for avaliable stats
        }

        text_available.text = $"Upgrades Available: {avaliableUpgrades}";
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
        // Increase stat by 1
        statToAdd[index] += 1;

        // Update UI
        StatModifier modifier = slot.upgradeSlotData.modifiers[index];
        text_statLines[index].text = $"{modifier.statType}: {modifier.value + statToAdd[index]}";

        // Enable Apply Button
        button_apply.gameObject.SetActive(true);

        // Enable Minue Button
        button_statMinus[index].gameObject.SetActive(true);

        // Hide Plus button if we cannot upgrade any further
    }

    public void DecreaseStat(int index)
    {
        // Decrease stat by 1
        statToAdd[index] -= 1;

        // Update UI
        StatModifier modifier = slot.upgradeSlotData.modifiers[index];
        text_statLines[index].text = $"{modifier.statType}: {modifier.value + statToAdd[index]}";

        // If we decreaes enough - disable the plus
    }

    public void ApplyButton()
    {
        // Spend Resources - worry about later

        // Increase Quality - worry about later

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

        // Return Item and reset the panel
        slot.ReturnItemAndClear();
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

        foreach (TextMeshProUGUI statLine in text_statLines)
        {
            statLine.text = "";
            statLine.transform.parent.gameObject.SetActive(false);
        }

        slot.ClearSlot();
    }
}
