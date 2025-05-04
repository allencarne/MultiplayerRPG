using TMPro;
using UnityEngine;

public class CharacterStatPreview : MonoBehaviour
{
    [SerializeField] int playerIndex;

    [SerializeField] TextMeshProUGUI classText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI coinText;
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] TextMeshProUGUI damageText;
    [SerializeField] TextMeshProUGUI attackSpeedText;
    [SerializeField] TextMeshProUGUI cdrText;

    private void Start()
    {
        classText.text = "Class: Beginner";
        levelText.text = "Level: " + PlayerPrefs.GetInt($"Character{playerIndex}_PlayerLevel", 1).ToString();
        coinText.text = "Coins: " + PlayerPrefs.GetFloat($"Character{playerIndex}_Coins", 0).ToString();
        healthText.text = "Health: " + PlayerPrefs.GetInt($"Character{playerIndex}_MaxHealth", 10).ToString();
        damageText.text = "Damage: " + PlayerPrefs.GetInt($"Character{playerIndex}_Damage", 1).ToString();
        attackSpeedText.text = "Attack Speed: " + PlayerPrefs.GetInt($"Character{playerIndex}_AttackSpeed", 1).ToString();
        cdrText.text = "Cool Down: " + PlayerPrefs.GetInt($"Character{playerIndex}_CDR", 1).ToString();
    }
}
