using UnityEngine;
using TMPro;
using System.Text;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemToolTip : MonoBehaviour, ISelectHandler, IDeselectHandler, ISubmitHandler, IPointerClickHandler, ICancelHandler
{
    [SerializeField] GameObject tooltip;
    [SerializeField] GameObject contextMenu;

    [SerializeField] InventorySlot inventorySlot;
    [SerializeField] EquipmentSlot equipment;

    [SerializeField] ItemRarityInfo riarityInfo;
    [SerializeField] GameObject gemSlotsImage;

    [SerializeField] Image itemIcon;
    [SerializeField] Image textBox;

    [SerializeField] TextMeshProUGUI itemName_Text;
    [SerializeField] TextMeshProUGUI itemInfo_Text;

    [SerializeField] GameObject button_Use;
    [SerializeField] GameObject button_Move;
    [SerializeField] GameObject button_Split;
    [SerializeField] GameObject button_Drop;

    public void UpdateCurrencyInfo(Currency currency, Item item)
    {
        // Hide gem slots for currency
        gemSlotsImage.SetActive(false);

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
        gemSlotsImage.SetActive(false);

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
        gemSlotsImage.SetActive(true);

        // Sprite
        itemIcon.sprite = equipment.Icon;

        // Name
        itemName_Text.text = FormatNameWithRarity(item.name.Replace("(Clone)", "").Trim(), item.ItemRarity);

        // Build text info
        StringBuilder sb = new StringBuilder();
        sb.AppendLine();

        // Loop through each stat modifier
        foreach (var mod in equipment.modifiers)
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
        gemSlotsImage.SetActive(true);

        // Sprite
        itemIcon.sprite = weapon.Icon;

        // Name
        itemName_Text.text = FormatNameWithRarity(item.name.Replace("(Clone)", "").Trim(), item.ItemRarity);

        // Build text info
        StringBuilder sb = new StringBuilder();
        sb.AppendLine();

        // Loop through each stat modifier
        foreach (var mod in weapon.modifiers)
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

        var tempColor = textBox.color;
        tempColor.a = .90f;
        textBox.color = tempColor;

        // Convert the Color to a hex string
        string colorHex = UnityEngine.ColorUtility.ToHtmlStringRGB(color);

        // Format the name with the appropriate color using rich text
        return $"<color=#{colorHex}><b>{name}</b></color>";
    }

    public void OnSelect(BaseEventData eventData)
    {
        Item item = GetCurrentItem();
        if (item == null) return;

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

        if (contextMenu == null) return;
        if (!contextMenu.activeSelf)
        {
            tooltip.SetActive(true);
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        tooltip.SetActive(false);
    }

    private Item GetCurrentItem()
    {
        if (inventorySlot != null && inventorySlot.slotData != null)
            return inventorySlot.slotData.item;

        if (equipment != null && equipment.Item != null)
            return equipment.Item;

        return null;
    }

    private void OnDisable()
    {
        tooltip.SetActive(false);
    }

    public void OnSubmit(BaseEventData eventData)
    {
        tooltip.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Left and Right Click Disables Tooltip
        tooltip.SetActive(false);

        // Equipment doesn't have an Inventory Slot
        if (inventorySlot == null) return;

        // Left Click
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (inventorySlot.slotData == null)
            {
                contextMenu.SetActive(false);
            }
        }

        // Right Click
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (contextMenu == null || inventorySlot.slotData == null) return;

            if (contextMenu.activeSelf)
            {
                contextMenu.SetActive(false);
            }
            else
            {
                UpdateContextMenuButtons(inventorySlot.slotData.item);
                contextMenu.SetActive(true);
            }
        }
    }

    private void UpdateContextMenuButtons(Item item)
    {
        if (item == null || contextMenu == null) return;

        // Use is only available for Consumable, Equipment, Weapon
        bool canUse = item is Consumable || item is Equipment || item is Weapon;
        button_Use.SetActive(canUse);

        // Split is available for stackable Collectables and Consumables
        bool canSplit = item.IsStackable && !(item is Equipment);
        button_Split.SetActive(canSplit);

        // Move and Drop are always available
        button_Move.SetActive(true);
        button_Drop.SetActive(true);
    }

    public void OnCancel(BaseEventData eventData)
    {
        tooltip.SetActive(false);
        if (inventorySlot == null) return;

        if (contextMenu.activeSelf)
        {
            contextMenu.SetActive(false);
        }
        else
        {
            UpdateContextMenuButtons(inventorySlot.slotData.item);
            contextMenu.SetActive(true);
        }
    }
}