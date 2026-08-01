using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    [SerializeField] UpgradeSlot slot;

    [SerializeField] Image image_background;
    [SerializeField] Image image_icon;
    [SerializeField] Image image_collectableIcon;

    [SerializeField] TextMeshProUGUI text_name;
    [SerializeField] TextMeshProUGUI text_description;
    [SerializeField] TextMeshProUGUI text_available;
    [SerializeField] TextMeshProUGUI text_coinCost;
    [SerializeField] TextMeshProUGUI text_collectableCost;

    [SerializeField] TextMeshProUGUI[] text_statLines;

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

    public void CloseUpgradeUI()
    {
        image_icon.sprite = null;
        text_name.text = "Drop Equuipment to Upgrade";
        text_name.color = Color.grey;
        image_background.color = Color.white;
        text_description.text = "";
        text_available.text = "Upgrades Available: 0";

        foreach (TextMeshProUGUI statLine in text_statLines)
        {
            statLine.text = "";
            statLine.transform.parent.gameObject.SetActive(false);
        }

        slot.ClearSlot();
    }

    public void IncreaseStat(int index)
    {

    }

    public void DecreaseStat(int index)
    {

    }
}
