using UnityEngine;

public class AttributeSkillButtons : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] PlayerUI playerUI;
    [SerializeField] Player player;
    [SerializeField] PlayerStats stats;
    [SerializeField] SkillPanel skillPanel;

    [SerializeField] AttributePoints ap;
    [SerializeField] PlayerExperience exp;

    [Header("UI")]
    [SerializeField] GameObject attributeUI;
    [SerializeField] GameObject skillUI;

    [Header("Regular")]
    [SerializeField] GameObject attribute_Button;
    [SerializeField] GameObject skill_Button;

    [Header("Mobile")]
    [SerializeField] GameObject m_attribute_Button;
    [SerializeField] GameObject m_skill_Button;

    bool isMobile;

    private void OnEnable()
    {
        stats.PlayerLevel.OnValueChanged += OnLevelChanged;
        stats.AttributePoints.OnValueChanged += OnAPChanged;

        ap.OnStatsApplied.AddListener(HandleAttributes);
        skillPanel.OnSkillSelected.AddListener(HandleAllSkills);
    }

    private void OnDisable()
    {
        stats.PlayerLevel.OnValueChanged -= OnLevelChanged;
        stats.AttributePoints.OnValueChanged -= OnAPChanged;

        ap.OnStatsApplied.RemoveListener(HandleAttributes);
        skillPanel.OnSkillSelected.RemoveListener(HandleAllSkills);
    }

    private void Start()
    {
        if (Application.isMobilePlatform)
        {
            isMobile = true;
        }
    }

    void OnLevelChanged(int oldValue, int newValue)
    {
        UpdateUI();
    }

    void OnAPChanged(int oldValue, int newValue)
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        HandleAttributes();
        HandleAllSkills();
    }

    public void HandleAttributes()
    {
        if (stats.AttributePoints.Value == 0)
        {
            attribute_Button.SetActive(false);
            m_attribute_Button.SetActive(false);
        }
        else
        {
            if (isMobile)
            {
                if (!attributeUI.activeInHierarchy) m_attribute_Button.SetActive(true);
            }
            else
            {
                if (!attributeUI.activeInHierarchy) attribute_Button.SetActive(true);
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
            if (stats.PlayerLevel.Value >= level)
            {
                if (isMobile)
                {
                    if (!skillUI.activeInHierarchy) m_skill_Button.SetActive(true);
                }
                else
                {
                    if (!skillUI.activeInHierarchy) skill_Button.SetActive(true);
                }
            }
        }
        else
        {
            skill_Button.SetActive(false);
            m_skill_Button.SetActive(false);
        }
    }

    public void _AttributeButton()
    {
        playerUI._AttributeUI();

        if (attribute_Button.activeSelf)
        {
            attribute_Button.SetActive(false);
        }
        else
        {
            attribute_Button.SetActive(true);
        }
    }

    public void _SkillButton()
    {
        playerUI._SkillUI();

        if (skill_Button.activeSelf)
        {
            skill_Button.SetActive(false);
        }
        else
        {
            skill_Button.SetActive(true);
        }
    }

    public void _AttributeButton_M()
    {
        playerUI._AttributeUI();

        if (m_attribute_Button.activeSelf)
        {
            m_attribute_Button.SetActive(false);
        }
        else
        {
            m_attribute_Button.SetActive(true);
        }
    }

    public void _SkillButton_M()
    {
        playerUI._SkillUI();

        if (m_skill_Button.activeSelf)
        {
            m_skill_Button.SetActive(false);
        }
        else
        {
            m_skill_Button.SetActive(true);
        }
    }
}
