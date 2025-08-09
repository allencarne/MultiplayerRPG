using UnityEngine;

public class AttributeSkillButtons : MonoBehaviour
{
    [SerializeField] GameObject attribute_Button;
    [SerializeField] GameObject skill_Button;
    [SerializeField] Player player;

    private void Start()
    {
        HandleAttributes();
        HandleAllSkills();
    }

    public void HandleAttributes()
    {
        if (player.AttributePoints.Value == 0)
        {
            attribute_Button.SetActive(false);
        }
        else
        {
            attribute_Button.SetActive(true);
        }
    }

    public void HandleAllSkills()
    {
        HandleSkills(player.FirstPassiveIndex, 1);
        HandleSkills(player.BasicIndex, 1);
        HandleSkills(player.OffensiveIndex, 4);
        HandleSkills(player.SecondPassiveIndex, 6);
        HandleSkills(player.MobilityIndex, 8);
        HandleSkills(player.DefensiveIndex, 12);
        HandleSkills(player.ThirdPassiveIndex, 14);
        HandleSkills(player.UtilityIndex, 16);
        HandleSkills(player.UltimateIndex, 20);
    }

    void HandleSkills(int abilityIndex, int level)
    {
        if (abilityIndex == -1)
        {
            if (player.PlayerLevel.Value >= level)
            {
                skill_Button.SetActive(true);
            }
        }
        else
        {
            skill_Button.SetActive(false);
        }
    }
}
