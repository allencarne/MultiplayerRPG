using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AttributeUI : MonoBehaviour
{
    [SerializeField] Player player;

    [SerializeField] TextMeshProUGUI playerName;
    [SerializeField] TextMeshProUGUI playerLevel;
    [SerializeField] TextMeshProUGUI playerClass;
    [SerializeField] TextMeshProUGUI attributePoints;

    [SerializeField] TextMeshProUGUI baseHealth;
    [SerializeField] TextMeshProUGUI bonusHealth;
    [SerializeField] TextMeshProUGUI totalHealth;

    private void Start()
    {

    }

    private void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        // Character Stats
        playerName.text = player.PlayerName;
        playerLevel.text = "LvL: " + player.PlayerLevel.ToString();
        playerClass.text = "Class: " + player.playerClass.ToString();
        attributePoints.text = "Avaliable Points: " + player.AttributePoints.ToString();

        // Attributes
        baseHealth.text = player.Health.Value.ToString();
        bonusHealth.text = (player.MaxHealth.Value - player.Health.Value).ToString();
        totalHealth.text = player.MaxHealth.Value.ToString();
    }
}
