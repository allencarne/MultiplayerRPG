using UnityEngine;
using TMPro;

public class ItemToolTip : MonoBehaviour
{
    [SerializeField] ItemPickup itemPickup;

    [SerializeField] GameObject gemSlotsImage;
    [SerializeField] TextMeshProUGUI itemInfo_Text;


    private void Start()
    {
        if (itemPickup.Item != null)
        {
            if (itemPickup.Item.IsCurrency)
            {
                UpdateCurrencyInfo();
            }

            if (itemPickup.Item is Equipment equipment)
            {
                UpdateEquipmentInfo(equipment);
            }

            if (itemPickup.Item is Weapon weapon)
            {
                UpdateWeaponInfo(weapon);
            }
        }
    }

    public void UpdateCurrencyInfo()
    {
        // Gem Slots
        gemSlotsImage.SetActive(false);

        // Info
        itemInfo_Text.text = itemPickup.Item.Prefab.name;
        // Next Line
        itemInfo_Text.text = itemPickup.Item.Description;
    }

    public void UpdateEquipmentInfo(Equipment equipment)
    {
        // Gem Slots
        gemSlotsImage.SetActive(true);

        // Info
        itemInfo_Text.text = equipment.Prefab.name;

        // Next Line
        itemInfo_Text.text = equipment.healthModifier.ToString(); // +1 Health Hide if 0
        itemInfo_Text.text = equipment.damageModifier.ToString(); // +1 Damage Hide if 0

        // Next Line
        itemInfo_Text.text = equipment.ItemRarity.ToString();
        itemInfo_Text.text = equipment.equipmentType.ToString();
        itemInfo_Text.text = "Level Requirement: " + equipment.LevelRequirement.ToString();
        itemInfo_Text.text = "Class Requirement: " + equipment.ClassRequirement.ToString();
        itemInfo_Text.text = "Sell Value: " + equipment.SellValue.ToString();
    }

    public void UpdateWeaponInfo(Weapon weapon)
    {
        // Gem Slots
        gemSlotsImage.SetActive(true);

        // Info
        itemInfo_Text.text = weapon.Prefab.name;
        // Next Line
        itemInfo_Text.text = weapon.healthModifier.ToString(); // +1 Health Hide if 0
        itemInfo_Text.text = weapon.damageModifier.ToString(); // +1 Damage Hide if 0

        // Next Line
        itemInfo_Text.text = weapon.ItemRarity.ToString();
        itemInfo_Text.text = weapon.weaponType.ToString();
        itemInfo_Text.text = "Level Requirement: " + weapon.LevelRequirement.ToString();
        itemInfo_Text.text = "Class Requirement: " + weapon.ClassRequirement.ToString();
        itemInfo_Text.text = "Sell Value: " + weapon.SellValue.ToString();
    }
}
