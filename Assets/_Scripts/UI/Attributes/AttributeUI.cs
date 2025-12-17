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
        totalHealth.text = StringBuildFloat(stats.ModifiedMaxHealth, stats.MaxHealth.Value, stats.GetModifierFloat(StatType.Health));

        // Damage
        totalDamage.text = StringBuildInt(stats.ModifiedDamage, stats.Damage, stats.GetModifierInt(StatType.Damage));

        // Attack Speed
        totalAttackSpeed.text = stats.AttackSpeed.ToString("F2");

        // Cooldown Reduction (CDR)
        totalCDR.text = stats.CoolDownReduction.ToString("F2");

        // Speed
        totalSpeed.text = stats.Speed.ToString("F2");

        // Endurance
        enduranceRecharge.text = stats.EnduranceRechargeRate.Value.ToString();

        // Armor
        totalArmor.text = stats.Armor.ToString();
    }

    string StringBuildFloat(float total, float value, float mods)
    {
        if (mods == 0)
        {
            return total.ToString();
        }
        else
        {
            return $"{total} ({value} + <color=#33C4FF>{mods}</color>)";
        }
    }

    string StringBuildInt(int total, int value, int mods)
    {
        if (mods == 0)
        {
            return total.ToString();
        }
        else
        {
            return $"{total} ({value} + <color=#33C4FF>{mods}</color>)";
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
