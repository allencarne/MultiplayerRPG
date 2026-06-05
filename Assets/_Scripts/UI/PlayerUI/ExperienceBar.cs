using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExperienceBar : MonoBehaviour
{
    [SerializeField] PlayerStats stats;

    [SerializeField] Image frontXpBar;
    [SerializeField] Image backXpBar;
    [SerializeField] TextMeshProUGUI experienceText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] PlayerExperience playerExperience;

    void OnEnable()
    {
        playerExperience.OnEXPGained.AddListener(UpdateBars);
        playerExperience.OnLevelUp.AddListener(UpdateLevel);
    }

    void OnDisable()
    {
        playerExperience.OnEXPGained.RemoveListener(UpdateBars);
        playerExperience.OnLevelUp.RemoveListener(UpdateLevel);
    }

    void UpdateBars(float exp)
    {
        // Increase player experience bar fill amount based on current experience and required experience
    }
    void UpdateLevel()
    {
        // Increase player level by 1
    }
}
