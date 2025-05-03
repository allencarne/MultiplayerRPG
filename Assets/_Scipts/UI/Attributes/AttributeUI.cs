using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AttributeUI : MonoBehaviour
{
    [SerializeField] Player player;

    [SerializeField] TextMeshProUGUI playerName;
    [SerializeField] TextMeshProUGUI playerClass;
    [SerializeField] Sprite[] classIcons;
    [SerializeField] Image classIcon;
    [SerializeField] TextMeshProUGUI playerLevel;
    [SerializeField] TextMeshProUGUI attributePoints;

    [SerializeField] TextMeshProUGUI baseHealth;
    [SerializeField] TextMeshProUGUI bonusHealth;
    [SerializeField] TextMeshProUGUI totalHealth;

    [SerializeField] TextMeshProUGUI baseDamage;
    [SerializeField] TextMeshProUGUI bonusDamage;
    [SerializeField] TextMeshProUGUI totalDamage;

    [SerializeField] TextMeshProUGUI baseAttackSpeed;
    [SerializeField] TextMeshProUGUI bonusAttackSpeed;
    [SerializeField] TextMeshProUGUI totalAttackSpeed;

    [SerializeField] TextMeshProUGUI baseCDR;
    [SerializeField] TextMeshProUGUI bonusCDR;
    [SerializeField] TextMeshProUGUI totalCDR;

    [SerializeField] TextMeshProUGUI baseSpeed;
    [SerializeField] TextMeshProUGUI bonusSpeed;
    [SerializeField] TextMeshProUGUI totalSpeed;

    [SerializeField] TextMeshProUGUI baseEndurance;
    [SerializeField] TextMeshProUGUI bonusEndurance;
    [SerializeField] TextMeshProUGUI totalEndurance;

    [SerializeField] TextMeshProUGUI baseArmor;
    [SerializeField] TextMeshProUGUI bonusArmor;
    [SerializeField] TextMeshProUGUI totalArmor;

    private void Start()
    {
        InvokeRepeating("UpdateUI", 0,1);
    }

    void UpdateUI()
    {
        // Character Stats
        playerName.text = player.PlayerName;
        playerClass.text = "Class: " + player.playerClass.ToString();
        GetClassIcon();
        playerLevel.text = "LvL: " + player.PlayerLevel.ToString();
        attributePoints.text = "Avaliable Points: " + player.AttributePoints.ToString();

        // Health
        baseHealth.text = player.Health.Value.ToString();
        bonusHealth.text = (player.MaxHealth.Value - player.Health.Value).ToString();
        totalHealth.text = player.MaxHealth.Value.ToString();

        // Damage
        baseDamage.text = player.BaseDamage.Value.ToString();
        bonusDamage.text = (player.CurrentDamage.Value - player.BaseDamage.Value).ToString();
        totalDamage.text = player.CurrentDamage.Value.ToString();

        // Attack Speed
        baseAttackSpeed.text = player.BaseAttackSpeed.Value.ToString("F2");
        bonusAttackSpeed.text = (player.CurrentAttackSpeed.Value - player.BaseAttackSpeed.Value).ToString("F2");
        totalAttackSpeed.text = player.CurrentAttackSpeed.Value.ToString("F2");

        // Cooldown Reduction (CDR)
        baseCDR.text = player.BaseCDR.Value.ToString("F2");
        bonusCDR.text = (player.CurrentCDR.Value - player.BaseCDR.Value).ToString("F2");
        totalCDR.text = player.CurrentCDR.Value.ToString("F2");

        // Speed
        baseSpeed.text = player.BaseSpeed.Value.ToString("F2");
        bonusSpeed.text = (player.CurrentSpeed.Value - player.BaseSpeed.Value).ToString("F2");
        totalSpeed.text = player.CurrentSpeed.Value.ToString("F2");

        // Endurance
        baseEndurance.text = player.Endurance.Value.ToString();
        bonusEndurance.text = (player.MaxEndurance.Value - player.Endurance.Value).ToString();
        totalEndurance.text = player.MaxEndurance.Value.ToString();

        // Armor
        baseArmor.text = player.BaseArmor.Value.ToString();
        bonusArmor.text = (player.CurrentArmor.Value - player.BaseArmor.Value).ToString();
        totalArmor.text = player.CurrentArmor.Value.ToString();
    }

    void GetClassIcon()
    {
        switch (player.playerClass)
        {
            case Player.PlayerClass.Beginner:
                classIcon.sprite = classIcons[0];
                break;
            case Player.PlayerClass.Warrior:
                classIcon.sprite = classIcons[1];
                break;
            case Player.PlayerClass.Magician:
                classIcon.sprite = classIcons[2];
                break;
            case Player.PlayerClass.Archer:
                classIcon.sprite = classIcons[3];
                break;
            case Player.PlayerClass.Rogue:
                classIcon.sprite = classIcons[4];
                break;
        }
    }
}
