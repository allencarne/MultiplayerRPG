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


    [SerializeField] TextMeshProUGUI totalHealth;
    [SerializeField] TextMeshProUGUI totalDamage;
    [SerializeField] TextMeshProUGUI totalAttackSpeed;
    [SerializeField] TextMeshProUGUI totalCDR;
    [SerializeField] TextMeshProUGUI totalSpeed;
    [SerializeField] TextMeshProUGUI totalEndurance;
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
        playerName.text = player.PlayerName;
        playerClass.text = "Class: " + player.playerClass.ToString();
        GetClassIcon();
        playerLevel.text = "LvL: " + player.PlayerLevel.Value.ToString();
        attributePoints.text = "Attribute Points: " + player.AttributePoints.Value.ToString();

        // Health
        totalHealth.text = player.MaxHealth.Value.ToString();

        // Damage
        totalDamage.text = player.CurrentDamage.Value.ToString();

        // Attack Speed
        totalAttackSpeed.text = player.CurrentAttackSpeed.Value.ToString("F2");

        // Cooldown Reduction (CDR)
        totalCDR.text = player.CurrentCDR.Value.ToString("F2");

        // Speed
        totalSpeed.text = player.CurrentSpeed.Value.ToString("F2");

        // Endurance
        totalEndurance.text = player.MaxEndurance.Value.ToString();

        // Armor
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
