using UnityEngine;

public class AttributeSkillButtons : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] Player player;

    [Header("Regular")]
    [SerializeField] GameObject attribute_Button;
    [SerializeField] GameObject skill_Button;

    [Header("Mobile")]
    [SerializeField] GameObject m_attribute_Button;
    [SerializeField] GameObject m_skill_Button;

    bool isMobile;

    private void Start()
    {
        if (Application.isMobilePlatform)
        {
            isMobile = true;
        }

        HandleAttributes();
        HandleAllSkills();
    }

    public void HandleAttributes()
    {
        if (player.AttributePoints.Value == 0)
        {
            attribute_Button.SetActive(false);
            m_attribute_Button.SetActive(false);
        }
        else
        {
            if (isMobile)
            {
                m_attribute_Button.SetActive(true);
            }
            else
            {
                attribute_Button.SetActive(true);
            }
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
                if (isMobile)
                {
                    m_skill_Button.SetActive(true);
                }
                else
                {
                    skill_Button.SetActive(true);
                }
            }
        }
        else
        {
            skill_Button.SetActive(false);
            m_skill_Button.SetActive(false);
        }
    }
}
