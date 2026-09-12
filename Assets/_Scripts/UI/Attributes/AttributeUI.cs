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

        // Health (health remains flat-only display)
        totalHealth.text = SimpleStringBuild(
            stats.net_TotalHP.Value,
            stats.net_BaseHP.Value,
            stats.GetModifier(StatType.Health, ModSource.Equipment),
            stats.GetModifier(StatType.Health, ModSource.Buff),
            stats.GetModifier(StatType.Health, ModSource.Debuff));

        // Damage (support flat + percent)
        totalDamage.text = ComplexStringBuild(
            stats.TotalDamage,
            stats.net_BaseDamage.Value,
            stats.GetModifier(StatType.Damage, ModSource.Equipment),
            stats.GetPercentModifier(StatType.Damage, ModSource.Equipment),
            stats.GetModifier(StatType.Damage, ModSource.Buff),
            stats.GetPercentModifier(StatType.Damage, ModSource.Buff),
            stats.GetModifier(StatType.Damage, ModSource.Debuff),
            stats.GetPercentModifier(StatType.Damage, ModSource.Debuff));

        // Attack Speed
        totalAttackSpeed.text = ComplexStringBuild(
            stats.TotalAS,
            stats.net_BaseAS.Value,
            stats.GetModifier(StatType.AttackSpeed, ModSource.Equipment),
            stats.GetPercentModifier(StatType.AttackSpeed, ModSource.Equipment),
            stats.GetModifier(StatType.AttackSpeed, ModSource.Buff),
            stats.GetPercentModifier(StatType.AttackSpeed, ModSource.Buff),
            stats.GetModifier(StatType.AttackSpeed, ModSource.Debuff),
            stats.GetPercentModifier(StatType.AttackSpeed, ModSource.Debuff));

        // Cooldown Reduction (CDR)
        totalCDR.text = ComplexStringBuild(
            stats.TotalCDR,
            stats.net_BaseCDR.Value,
            stats.GetModifier(StatType.CoolDown, ModSource.Equipment),
            stats.GetPercentModifier(StatType.CoolDown, ModSource.Equipment),
            stats.GetModifier(StatType.CoolDown, ModSource.Buff),
            stats.GetPercentModifier(StatType.CoolDown, ModSource.Buff),
            stats.GetModifier(StatType.CoolDown, ModSource.Debuff),
            stats.GetPercentModifier(StatType.CoolDown, ModSource.Debuff));

        // Speed
        totalSpeed.text = ComplexStringBuild(
            stats.TotalSpeed,
            stats.net_BaseSpeed.Value,
            stats.GetModifier(StatType.Speed, ModSource.Equipment),
            stats.GetPercentModifier(StatType.Speed, ModSource.Equipment),
            stats.GetModifier(StatType.Speed, ModSource.Buff),
            stats.GetPercentModifier(StatType.Speed, ModSource.Buff),
            stats.GetModifier(StatType.Speed, ModSource.Debuff),
            stats.GetPercentModifier(StatType.Speed, ModSource.Debuff));

        // Endurance
        enduranceRecharge.text = stats.EnduranceRechargeRate.Value.ToString();

        // Armor (flat)
        totalArmor.text = stats.net_BaseArmor.Value.ToString();
    }

    string SimpleStringBuild(float total, float value, float equipment, float buff, float debuff)
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
                modStrings.Add($"<color=#33C4FF>{FormatModifierFlat(equipment)}</color>");
            }

            if (buff != 0)
            {
                modStrings.Add($"<color=#33FF33>{FormatModifierFlat(buff)}</color>");
            }

            if (debuff != 0)
            {
                modStrings.Add($"<color=#FF3333>{FormatModifierFlat(debuff)}</color>");
            }

            return $"{FormatValue(total)} ({FormatValue(value)} {string.Join(" ", modStrings)})";
        }
    }

    // For stats that can have flat + percent modifiers
    string ComplexStringBuild(float total, float value,
        float equipmentFlat, float equipmentPct,
        float buffFlat, float buffPct,
        float debuffFlat, float debuffPct)
    {
        bool hasMods = equipmentFlat != 0 || equipmentPct != 0 || buffFlat != 0 || buffPct != 0 || debuffFlat != 0 || debuffPct != 0;

        if (!hasMods)
        {
            return FormatValue(total);
        }
        else
        {
            List<string> modStrings = new List<string>();

            if (equipmentFlat != 0) modStrings.Add($"<color=#33C4FF>{FormatModifierFlat(equipmentFlat)}</color>");
            if (equipmentPct != 0) modStrings.Add($"<color=#33C4FF>{FormatModifierPercent(equipmentPct)}</color>");

            if (buffFlat != 0) modStrings.Add($"<color=#33FF33>{FormatModifierFlat(buffFlat)}</color>");
            if (buffPct != 0) modStrings.Add($"<color=#33FF33>{FormatModifierPercent(buffPct)}</color>");

            if (debuffFlat != 0) modStrings.Add($"<color=#FF3333>{FormatModifierFlat(debuffFlat)}</color>");
            if (debuffPct != 0) modStrings.Add($"<color=#FF3333>{FormatModifierPercent(debuffPct)}</color>");

            return $"{FormatValue(total)} ({FormatValue(value)} {string.Join(" ", modStrings)})";
        }
    }

    string FormatModifierFlat(float modifier)
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

    string FormatModifierPercent(float fractional)
    {
        float pct = fractional * 100f;
        if (pct % 1 == 0)
        {
            return $"{pct:+0;-0}%";
        }
        else
        {
            return $"{pct:+0.0;-0.0}%";
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
            case PlayerStats.PlayerClass.Beginner: classIcon.sprite = classIcons[0]; break;
            case PlayerStats.PlayerClass.Warrior: classIcon.sprite = classIcons[1]; break;
            case PlayerStats.PlayerClass.Magician: classIcon.sprite = classIcons[2]; break;
            case PlayerStats.PlayerClass.Archer: classIcon.sprite = classIcons[3]; break;
            case PlayerStats.PlayerClass.Rogue: classIcon.sprite = classIcons[4]; break;
        }
    }
}
