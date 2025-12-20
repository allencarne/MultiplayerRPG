using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AttributeUI : MonoBehaviour
{
    [SerializeField] PlayerStats stats;

    [SerializeField] TextMeshProUGUI playerName;
    [SerializeField] TextMeshProUGUI playerClass;
    [SerializeField] Sprite[] classIcons;
    [SerializeField] Image classIcon;
    [SerializeField] TextMeshProUGUI playerLevel;
    [SerializeField] TextMeshProUGUI attributePoints;

    [SerializeField] TextMeshProUGUI totalHealth;
    [SerializeField] TextMeshProUGUI totalDamage;
    [SerializeField] TextMeshProUGUI totalAttackSpeed;
    [SerializeField] TextMeshProUGUI totalCDR;
    [SerializeField] TextMeshProUGUI totalSpeed;
    [SerializeField] TextMeshProUGUI enduranceRecharge;
    [SerializeField] TextMeshProUGUI totalArmor;

    private void OnEnable()
    {
        InvokeRepeating("UpdateUI", 0, 1);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    void UpdateUI()
    {
        // Character Stats
        playerName.text = stats.net_playerName.Value.ToString();
        playerClass.text = "Class: " + stats.playerClass.ToString();
        GetClassIcon();
        playerLevel.text = "LvL: " + stats.PlayerLevel.Value.ToString();
        attributePoints.text = "Attribute Points: " + stats.AttributePoints.Value.ToString();

        // Health
        totalHealth.text = StringBuildFloat(
            stats.net_TotalHP.Value,
            stats.net_BaseHP.Value,
            stats.GetModifierFloat(StatType.Health, ModSource.Equipment),
            stats.GetModifierFloat(StatType.Health, ModSource.Buff),
            stats.GetModifierFloat(StatType.Health, ModSource.Debuff));

        // Damage
        totalDamage.text = StringBuildFloat(
            stats.TotalDamage,
            stats.BaseDamage,
            stats.GetModifierFloat(StatType.Damage, ModSource.Equipment),
            stats.GetModifierFloat(StatType.Damage, ModSource.Buff),
            stats.GetModifierFloat(StatType.Damage, ModSource.Debuff));

        // Attack Speed
        totalAttackSpeed.text = StringBuildFloat(
            stats.TotalAS,
            stats.BaseAS,
            stats.GetModifierFloat(StatType.AttackSpeed, ModSource.Equipment),
            stats.GetModifierFloat(StatType.AttackSpeed, ModSource.Buff),
            stats.GetModifierFloat(StatType.AttackSpeed, ModSource.Debuff));

        // Cooldown Reduction (CDR)
        totalCDR.text = StringBuildFloat(
            stats.TotalCDR,
            stats.BaseCDR,
            stats.GetModifierFloat(StatType.CoolDown, ModSource.Equipment),
            stats.GetModifierFloat(StatType.CoolDown, ModSource.Buff),
            stats.GetModifierFloat(StatType.CoolDown, ModSource.Debuff));

        // Speed
        totalSpeed.text = StringBuildFloat(
            stats.TotalSpeed,
            stats.BaseSpeed,
            stats.GetModifierFloat(StatType.Speed, ModSource.Equipment),
            stats.GetModifierFloat(StatType.Speed, ModSource.Buff),
            stats.GetModifierFloat(StatType.Speed, ModSource.Debuff));

        // Endurance
        enduranceRecharge.text = stats.EnduranceRechargeRate.Value.ToString();

        // Armor
        totalArmor.text = stats.BaseArmor.ToString();
    }

    string StringBuildFloat(float total, float value, float equipment, float buff, float debuff)
    {
        float totalMods = equipment + buff + debuff;

        if (totalMods == 0)
        {
            return total.ToString();
        }
        else
        {
            List<string> modStrings = new List<string>();

            if (equipment != 0)
                modStrings.Add($"<color=#33C4FF>{equipment:+0;-0}</color>");

            if (buff != 0)
                modStrings.Add($"<color=#33FF33>{buff:+0;-0}</color>");

            if (debuff != 0)
                modStrings.Add($"<color=#FF3333>{debuff:+0;-0}</color>");

            return $"{total} ({value} {string.Join(" ", modStrings)})";
        }
    }

    void GetClassIcon()
    {
        switch (stats.playerClass)
        {
            case PlayerStats.PlayerClass.Beginner:
                classIcon.sprite = classIcons[0];
                break;
            case PlayerStats.PlayerClass.Warrior:
                classIcon.sprite = classIcons[1];
                break;
            case PlayerStats.PlayerClass.Magician:
                classIcon.sprite = classIcons[2];
                break;
            case PlayerStats.PlayerClass.Archer:
                classIcon.sprite = classIcons[3];
                break;
            case PlayerStats.PlayerClass.Rogue:
                classIcon.sprite = classIcons[4];
                break;
        }
    }
}
