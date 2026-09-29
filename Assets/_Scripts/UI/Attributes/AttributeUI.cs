using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AttributeUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] PlayerStats stats;

    [Header("References")]
    [SerializeField] Sprite[] spr_ClassIcons;

    [Header("Character Stats")]
    [SerializeField] TextMeshProUGUI txt_PlayerName;
    [SerializeField] TextMeshProUGUI txt_PlayerLevel;
    [SerializeField] TextMeshProUGUI txt_PlayerClass;
    [SerializeField] Image img_classIcon;
    [SerializeField] TextMeshProUGUI txt_PlayerAP;

    [Header("Combat Stats")]
    [SerializeField] TextMeshProUGUI txt_TotalHealth;
    [SerializeField] TextMeshProUGUI txt_TotalHealthRegen;
    [SerializeField] TextMeshProUGUI txt_TotalRecharge;
    [SerializeField] TextMeshProUGUI txt_TotalDexterity;
    [SerializeField] TextMeshProUGUI txt_TotalEndurance;
    [SerializeField] TextMeshProUGUI txt_totalEnduranceRegen;
    [SerializeField] TextMeshProUGUI txt_TotalPrecision;
    [SerializeField] TextMeshProUGUI txt_TotalFerocity;
    [SerializeField] TextMeshProUGUI txt_TotalPower;
    [SerializeField] TextMeshProUGUI txt_TotalVamp;
    [SerializeField] TextMeshProUGUI txt_TotalArmor;
    [SerializeField] TextMeshProUGUI txt_TotalLethality;
    [SerializeField] TextMeshProUGUI txt_TotalMana;
    [SerializeField] TextMeshProUGUI txt_TotalManaRegen;
    [SerializeField] TextMeshProUGUI txt_TotalSpeed;

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
        txt_PlayerName.text = $"<color=#FFFFFF>{stats.net_playerName.Value.ToString()}</color>";
        txt_PlayerLevel.text = $"LvL: <color=#FFFFFF>{stats.PlayerLevel.Value}</color>";
        txt_PlayerClass.text = $"Class: <color=#FFFFFF>{stats.playerClass.ToString()}</color>";
        GetClassIcon();
        txt_PlayerAP.text = $"Attribute Points: <color=#FFFFFF>{stats.AttributePoints.Value}</color>";

        // Health
        txt_TotalHealth.text = SimpleStringBuild(
            stats.net_TotalHealth.Value,
            stats.net_BaseHealth.Value,
            stats.GetModifier(StatType.Health, ModSource.Equipment),
            stats.GetModifier(StatType.Health, ModSource.Buff),
            stats.GetModifier(StatType.Health, ModSource.Debuff));

        // HealthRegen
        txt_TotalHealthRegen.text = ComplexStringBuild(
            stats.TotalHealthRegen,
            stats.net_BaseHealthRegen.Value,
            stats.GetModifier(StatType.HealthRegen, ModSource.Equipment),
            stats.GetPercentModifier(StatType.HealthRegen, ModSource.Equipment),
            stats.GetModifier(StatType.HealthRegen, ModSource.Buff),
            stats.GetPercentModifier(StatType.HealthRegen, ModSource.Buff),
            stats.GetModifier(StatType.HealthRegen, ModSource.Debuff),
            stats.GetPercentModifier(StatType.HealthRegen, ModSource.Debuff));

        // Recharge
        txt_TotalRecharge.text = ComplexStringBuild(
            stats.TotalCDR,
            stats.net_BaseCDR.Value,
            stats.GetModifier(StatType.Recharge, ModSource.Equipment),
            stats.GetPercentModifier(StatType.Recharge, ModSource.Equipment),
            stats.GetModifier(StatType.Recharge, ModSource.Buff),
            stats.GetPercentModifier(StatType.Recharge, ModSource.Buff),
            stats.GetModifier(StatType.Recharge, ModSource.Debuff),
            stats.GetPercentModifier(StatType.Recharge, ModSource.Debuff));

        // Dexterity
        txt_TotalDexterity.text = ComplexStringBuild(
            stats.TotalAS,
            stats.net_BaseAS.Value,
            stats.GetModifier(StatType.Dexterity, ModSource.Equipment),
            stats.GetPercentModifier(StatType.Dexterity, ModSource.Equipment),
            stats.GetModifier(StatType.Dexterity, ModSource.Buff),
            stats.GetPercentModifier(StatType.Dexterity, ModSource.Buff),
            stats.GetModifier(StatType.Dexterity, ModSource.Debuff),
            stats.GetPercentModifier(StatType.Dexterity, ModSource.Debuff));

        // Endurance
        txt_TotalEndurance.text = SimpleStringBuild(
            stats.TotalEndurance,
            stats.net_BaseEndurance.Value,
            stats.GetModifier(StatType.Endurance, ModSource.Equipment),
            stats.GetModifier(StatType.Endurance, ModSource.Buff),
            stats.GetModifier(StatType.Endurance, ModSource.Debuff));

        // Endurance Regen
        txt_totalEnduranceRegen.text = ComplexStringBuild(
            stats.TotalEnduranceRegen,
            stats.net_BaseEnduranceRegen.Value,
            stats.GetModifier(StatType.EnduranceRegen, ModSource.Equipment),
            stats.GetPercentModifier(StatType.EnduranceRegen, ModSource.Equipment),
            stats.GetModifier(StatType.EnduranceRegen, ModSource.Buff),
            stats.GetPercentModifier(StatType.EnduranceRegen, ModSource.Buff),
            stats.GetModifier(StatType.EnduranceRegen, ModSource.Debuff),
            stats.GetPercentModifier(StatType.EnduranceRegen, ModSource.Debuff));

        // Precision
        txt_TotalPrecision.text = ComplexStringBuild(
            stats.TotalPrecision,
            stats.net_BasePrecision.Value,
            stats.GetModifier(StatType.Precision, ModSource.Equipment),
            stats.GetPercentModifier(StatType.Precision, ModSource.Equipment),
            stats.GetModifier(StatType.Precision, ModSource.Buff),
            stats.GetPercentModifier(StatType.Precision, ModSource.Buff),
            stats.GetModifier(StatType.Precision, ModSource.Debuff),
            stats.GetPercentModifier(StatType.Precision, ModSource.Debuff));

        // Ferocity
        txt_TotalFerocity.text = ComplexStringBuild(
            stats.TotalFerocity,
            stats.net_BaseFerocity.Value,
            stats.GetModifier(StatType.Ferocity, ModSource.Equipment),
            stats.GetPercentModifier(StatType.Ferocity, ModSource.Equipment),
            stats.GetModifier(StatType.Ferocity, ModSource.Buff),
            stats.GetPercentModifier(StatType.Ferocity, ModSource.Buff),
            stats.GetModifier(StatType.Ferocity, ModSource.Debuff),
            stats.GetPercentModifier(StatType.Ferocity, ModSource.Debuff));

        // Power
        txt_TotalPower.text = ComplexStringBuild(
            stats.TotalDamage,
            stats.net_BaseDamage.Value,
            stats.GetModifier(StatType.Power, ModSource.Equipment),
            stats.GetPercentModifier(StatType.Power, ModSource.Equipment),
            stats.GetModifier(StatType.Power, ModSource.Buff),
            stats.GetPercentModifier(StatType.Power, ModSource.Buff),
            stats.GetModifier(StatType.Power, ModSource.Debuff),
            stats.GetPercentModifier(StatType.Power, ModSource.Debuff));

        // Vamp
        txt_TotalVamp.text = ComplexStringBuild(
            stats.TotalVamp,
            stats.net_BaseVamp.Value,
            stats.GetModifier(StatType.Vamp, ModSource.Equipment),
            stats.GetPercentModifier(StatType.Vamp, ModSource.Equipment),
            stats.GetModifier(StatType.Vamp, ModSource.Buff),
            stats.GetPercentModifier(StatType.Vamp, ModSource.Buff),
            stats.GetModifier(StatType.Vamp, ModSource.Debuff),
            stats.GetPercentModifier(StatType.Vamp, ModSource.Debuff));

        // Armor
        txt_TotalArmor.text = SimpleStringBuild(
            stats.TotalArmor,
            stats.net_BaseArmor.Value,
            stats.GetModifier(StatType.Armor, ModSource.Equipment),
            stats.GetModifier(StatType.Armor, ModSource.Buff),
            stats.GetModifier(StatType.Armor, ModSource.Debuff));

        // Lethality
        txt_TotalLethality.text = ComplexStringBuild(
            stats.TotalLethality,
            stats.net_BaseLethality.Value,
            stats.GetModifier(StatType.Lethality, ModSource.Equipment),
            stats.GetPercentModifier(StatType.Lethality, ModSource.Equipment),
            stats.GetModifier(StatType.Lethality, ModSource.Buff),
            stats.GetPercentModifier(StatType.Lethality, ModSource.Buff),
            stats.GetModifier(StatType.Lethality, ModSource.Debuff),
            stats.GetPercentModifier(StatType.Lethality, ModSource.Debuff));

        // Mana
        txt_TotalMana.text = ComplexStringBuild(
            stats.TotalMana,
            stats.net_BaseMana.Value,
            stats.GetModifier(StatType.Mana, ModSource.Equipment),
            stats.GetPercentModifier(StatType.Mana, ModSource.Equipment),
            stats.GetModifier(StatType.Mana, ModSource.Buff),
            stats.GetPercentModifier(StatType.Mana, ModSource.Buff),
            stats.GetModifier(StatType.Mana, ModSource.Debuff),
            stats.GetPercentModifier(StatType.Mana, ModSource.Debuff));

        // Mana Regen
        txt_TotalManaRegen.text = ComplexStringBuild(
            stats.TotalManaRegen,
            stats.net_BaseManaRegen.Value,
            stats.GetModifier(StatType.ManaRegen, ModSource.Equipment),
            stats.GetPercentModifier(StatType.ManaRegen, ModSource.Equipment),
            stats.GetModifier(StatType.ManaRegen, ModSource.Buff),
            stats.GetPercentModifier(StatType.ManaRegen, ModSource.Buff),
            stats.GetModifier(StatType.ManaRegen, ModSource.Debuff),
            stats.GetPercentModifier(StatType.ManaRegen, ModSource.Debuff));

        // Speed
        txt_TotalSpeed.text = ComplexStringBuild(
            stats.TotalSpeed,
            stats.net_BaseSpeed.Value,
            stats.GetModifier(StatType.Speed, ModSource.Equipment),
            stats.GetPercentModifier(StatType.Speed, ModSource.Equipment),
            stats.GetModifier(StatType.Speed, ModSource.Buff),
            stats.GetPercentModifier(StatType.Speed, ModSource.Buff),
            stats.GetModifier(StatType.Speed, ModSource.Debuff),
            stats.GetPercentModifier(StatType.Speed, ModSource.Debuff));
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

            return $"{FormatValue(total)} (<color=#AAAAAA>{FormatValue(value)}</color> {string.Join(" ", modStrings)})";
        }
    }

    string ComplexStringBuild(float total, float value, float equipmentFlat, float equipmentPct, float buffFlat, float buffPct, float debuffFlat, float debuffPct)
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

            return $"{FormatValue(total)} (<color=#AAAAAA>{FormatValue(value)}</color> {string.Join(" ", modStrings)})";
        }
    }

    string FormatModifierFlat(float modifier)
    {
        return $"{modifier:+0.##;-0.##}";
    }

    string FormatValue(float val)
    {
        return $"{val:0.##}";
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

    void GetClassIcon()
    {
        switch (stats.playerClass)
        {
            case PlayerStats.PlayerClass.Beginner: img_classIcon.sprite = spr_ClassIcons[0]; break;
            case PlayerStats.PlayerClass.Warrior: img_classIcon.sprite = spr_ClassIcons[1]; break;
            case PlayerStats.PlayerClass.Magician: img_classIcon.sprite = spr_ClassIcons[2]; break;
            case PlayerStats.PlayerClass.Archer: img_classIcon.sprite = spr_ClassIcons[3]; break;
            case PlayerStats.PlayerClass.Rogue: img_classIcon.sprite = spr_ClassIcons[4]; break;
        }
    }
}
