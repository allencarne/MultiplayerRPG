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
        totalHealth.text = StringBuild(
            stats.net_TotalHP.Value,
            stats.net_BaseHP.Value,
            stats.GetModifier(StatType.Health, ModSource.Equipment),
            stats.GetModifier(StatType.Health, ModSource.Buff),
            stats.GetModifier(StatType.Health, ModSource.Debuff));

        // Damage
        totalDamage.text = StringBuild(
            stats.TotalDamage,
            stats.BaseDamage,
            stats.GetModifier(StatType.Damage, ModSource.Equipment),
            stats.GetModifier(StatType.Damage, ModSource.Buff),
            stats.GetModifier(StatType.Damage, ModSource.Debuff));

        // Attack Speed
        totalAttackSpeed.text = StringBuild(
            stats.TotalAS,
            stats.BaseAS,
            stats.GetModifier(StatType.AttackSpeed, ModSource.Equipment),
            stats.GetModifier(StatType.AttackSpeed, ModSource.Buff),
            stats.GetModifier(StatType.AttackSpeed, ModSource.Debuff));

        // Cooldown Reduction (CDR)
        totalCDR.text = StringBuild(
            stats.TotalCDR,
            stats.BaseCDR,
            stats.GetModifier(StatType.CoolDown, ModSource.Equipment),
            stats.GetModifier(StatType.CoolDown, ModSource.Buff),
            stats.GetModifier(StatType.CoolDown, ModSource.Debuff));

        // Speed
        totalSpeed.text = StringBuild(
            stats.TotalSpeed,
            stats.BaseSpeed,
            stats.GetModifier(StatType.Speed, ModSource.Equipment),
            stats.GetModifier(StatType.Speed, ModSource.Buff),
            stats.GetModifier(StatType.Speed, ModSource.Debuff));

        // Endurance
        enduranceRecharge.text = stats.EnduranceRechargeRate.Value.ToString();

        // Armor
        totalArmor.text = stats.BaseArmor.ToString();
    }

    string StringBuild(float total, float value, float equipment, float buff, float debuff)
    {
        float totalMods = equipment + buff + debuff;

        if (totalMods == 0)
        {
            return FormatValue(total);
        }
        else
        {
            List<string> modStrings = new List<string>();

            if (equipment != 0)
            {
                modStrings.Add($"<color=#33C4FF>{FormatModifier(equipment)}</color>");
            }

            if (buff != 0)
            {
                modStrings.Add($"<color=#33FF33>{FormatModifier(buff)}</color>");
            }

            if (debuff != 0)
            {
                modStrings.Add($"<color=#FF3333>{FormatModifier(debuff)}</color>");
            }

            return $"{FormatValue(total)} ({FormatValue(value)} {string.Join(" ", modStrings)})";
        }
    }

    string FormatModifier(float modifier)
    {
        if (modifier % 1 == 0)
        {
            return $"{modifier:+0;-0}";
        }
        else
        {
            return $"{modifier:+0.0;-0.0}";
        }
    }

    string FormatValue(float val)
    {
        if (val % 1 == 0)
        {
            return $"{val:0}";
        }
        else
        {
            return $"{val:0.0}";
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
