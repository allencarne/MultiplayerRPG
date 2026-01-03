using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VendorItemToolTip : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] ItemRarityInfo riarityInfo;
    //[SerializeField] GameObject gemSlotsImage;

    Image itemIcon;
    Image textBox;
    TextMeshProUGUI itemName_Text;
    TextMeshProUGUI itemInfo_Text;

    [SerializeField] GameObject ToolTipPrefab;
    GameObject tooltip;
    Item item;

    public void Init(Item _item)
    {
        item = _item;
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (item == null) return;

        Canvas canvas = GetComponentInParent<Canvas>();
        tooltip = Instantiate(ToolTipPrefab, canvas.transform);
        RectTransform tooltipRect = tooltip.GetComponent<RectTransform>();
        tooltipRect.position = transform.position + new Vector3(0, 1f, 0);

        Transform nameTransform = tooltip.transform.Find("TextBox/Text - Name");
        if (nameTransform != null)
        {
            itemName_Text = nameTransform.GetComponent<TextMeshProUGUI>();
        }

        Transform infoTransform = tooltip.transform.Find("TextBox/Text - Description");
        if (infoTransform != null)
        {
            itemInfo_Text = infoTransform.GetComponent<TextMeshProUGUI>();
        }

        Transform iconTransform = tooltip.transform.Find("TextBox/Icon");
        if (iconTransform != null)
        {
            itemIcon = iconTransform.GetComponent<Image>();
        }

        Transform textboxTransform = tooltip.transform.Find("TextBox");
        if (textboxTransform != null)
        {
            textBox = textboxTransform.GetComponent<Image>();
        }

        switch (item)
        {
            case Currency currency:
                UpdateCurrencyInfo(currency, item);
                break;

            case Collectable collectable:
                UpdateCollectableInfo(collectable, item);
                break;

            case Weapon weapon:
                UpdateWeaponInfo(weapon, item);
                break;

            case Equipment equip:
                UpdateEquipmentInfo(equip, item);
                break;
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        Destroy(tooltip);
    }

    public void UpdateCurrencyInfo(Currency currency, Item item)
    {
        // Hide gem slots for currency
        //gemSlotsImage.SetActive(false);

        // Sprite
        itemIcon.sprite = currency.Icon;

        // Name
        itemName_Text.text = FormatNameWithRarity(item.name.Replace("(Clone)", "").Trim(), item.ItemRarity);

        // Description
        itemInfo_Text.text = item.Description;
    }

    public void UpdateCollectableInfo(Collectable collectable, Item item)
    {
        // Hide gem slots for currency
        //gemSlotsImage.SetActive(false);

        // Sprite
        itemIcon.sprite = collectable.Icon;

        // Name
        itemName_Text.text = FormatNameWithRarity(item.name.Replace("(Clone)", "").Trim(), item.ItemRarity);

        // Description
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(item.Description);
        sb.AppendLine($"{collectable.SellValue}<sprite index=0>");

        // Update text
        itemInfo_Text.text = sb.ToString();
    }

    public void UpdateEquipmentInfo(Equipment equipment, Item item)
    {
        // Show gem slots for equipment
        //gemSlotsImage.SetActive(true);

        // Sprite
        itemIcon.sprite = equipment.Icon;

        // Name
        itemName_Text.text = FormatNameWithRarity(item.name.Replace("(Clone)", "").Trim(), item.ItemRarity);

        // Build text info
        StringBuilder sb = new StringBuilder();
        sb.AppendLine();

        // Loop through each stat modifier
        foreach (StatModifier mod in equipment.modifiers)
        {
            sb.AppendLine($"+{mod.value} {mod.statType}");
        }

        sb.AppendLine();

        // Add additional info
        sb.AppendLine(FormatNameWithRarity(equipment.ItemRarity.ToString(), equipment.ItemRarity));
        sb.AppendLine($"{equipment.equipmentType}");
        sb.AppendLine($"Required Level: {equipment.LevelRequirement}");
        sb.AppendLine($"Required Class: {equipment.ClassRequirement}");
        sb.AppendLine($"{equipment.SellValue}<sprite index=0>");

        // Update text
        itemInfo_Text.text = sb.ToString();
    }

    public void UpdateWeaponInfo(Weapon weapon, Item item)
    {
        // Show gem slots for weapons
        //gemSlotsImage.SetActive(true);

        // Sprite
        itemIcon.sprite = weapon.Icon;

        // Name
        itemName_Text.text = FormatNameWithRarity(item.name.Replace("(Clone)", "").Trim(), item.ItemRarity);

        // Build text info
        StringBuilder sb = new StringBuilder();
        sb.AppendLine();

        // Loop through each stat modifier
        foreach (StatModifier mod in weapon.modifiers)
        {
            sb.AppendLine($"+{mod.value} {mod.statType}");
        }

        sb.AppendLine();

        // Add additional info
        sb.AppendLine(FormatNameWithRarity(weapon.ItemRarity.ToString(), weapon.ItemRarity));
        sb.AppendLine($"{weapon.weaponType}");
        sb.AppendLine($"Required Level: {weapon.LevelRequirement}");
        sb.AppendLine($"Required Class: {weapon.ClassRequirement}");
        sb.AppendLine($"{weapon.SellValue}<sprite index=0>");

        // Update text
        itemInfo_Text.text = sb.ToString();
    }

    private string FormatNameWithRarity(string name, ItemRarity rarity)
    {
        // Retrieve the appropriate color from the ItemRarityInfo
        Color color = rarity switch
        {
            ItemRarity.Common => riarityInfo.CommonColor,
            ItemRarity.Uncommon => riarityInfo.UnCommonColor,
            ItemRarity.Rare => riarityInfo.RareColor,
            ItemRarity.Epic => riarityInfo.EpicColor,
            ItemRarity.Exotic => riarityInfo.ExoticColor,
            ItemRarity.Mythic => riarityInfo.MythicColor,
            ItemRarity.Legendary => riarityInfo.LegendaryColor,
            _ => Color.white // Default to white
        };

        // Assign Box Color
        textBox.color = color;

        Color tempColor = textBox.color;
        tempColor.a = .50f;
        textBox.color = tempColor;

        // Convert the Color to a hex string
        string colorHex = UnityEngine.ColorUtility.ToHtmlStringRGB(color);

        // Format the name with the appropriate color using rich text
        return $"<color=#{colorHex}><b>{name}</b></color>";
    }
}
