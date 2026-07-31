using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class UpgradeUI : MonoBehaviour
{
    [SerializeField] Image image_background;
    [SerializeField] Image image_icon;

    [SerializeField] TextMeshProUGUI text_description;

    public void AssignIcon(InventorySlotData slotData)
    {
        image_icon.sprite = slotData.item.Icon;
        image_background.color = slotData.item.GetRarityColor(slotData.rarity);
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

        string hex = ColorUtility.ToHtmlStringRGB(color);

        return $"<color=#{hex}><b>{quality}</b></color>";
    }
}
