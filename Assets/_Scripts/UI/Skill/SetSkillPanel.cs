using UnityEngine;
using UnityEngine.UI;

public class SetSkillPanel : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] PlayerStateMachine stateMachine;

    [SerializeField] GameObject BeginnerPanel;
    [SerializeField] GameObject WarriorPanel;
    [SerializeField] GameObject MagicianPanel;
    [SerializeField] GameObject ArcherPanel;
    [SerializeField] GameObject RoguePanel;

    public SkillPanel begginerSkills;
    public SkillPanel warriorSkills;
    public SkillPanel magicianSkills;
    public SkillPanel archerSkills;
    public SkillPanel rogueSkills;

    [Header("Ability Bar")]
    [SerializeField] Image[] skillBar_Basic;
    [SerializeField] Image[] skillBar_Offensive;
    [SerializeField] Image[] skillBar_Mobility;
    [SerializeField] Image[] skillBar_Defensive;
    [SerializeField] Image[] skillBar_Utility;
    [SerializeField] Image[] skillBar_Ultimate;

    [Header("Ability Bar Locks")]
    [SerializeField] Image[] skillBar_Basic_Lock;
    [SerializeField] Image[] skillBar_Offensive_Lock;
    [SerializeField] Image[] skillBar_Mobility_Lock;
    [SerializeField] Image[] skillBar_Defensive_Lock;
    [SerializeField] Image[] skillBar_Utility_Lock;
    [SerializeField] Image[] skillBar_Ultimate_Lock;

    private void Start()
    {
        BeginnerPanel.SetActive(false);
        WarriorPanel.SetActive(false);
        MagicianPanel.SetActive(false);
        ArcherPanel.SetActive(false);
        RoguePanel.SetActive(false);

        SetSkills();
        OnLevelUp();
    }

    public void SetSkills()
    {
        switch (player.playerClass)
        {
            case Player.PlayerClass.Beginner:
                BeginnerPanel.SetActive(true);
                stateMachine.skills = begginerSkills;
                AssignIcons(begginerSkills);
                break;
            case Player.PlayerClass.Warrior:
                WarriorPanel.SetActive(true);
                stateMachine.skills = warriorSkills;
                AssignIcons(warriorSkills);
                break;
            case Player.PlayerClass.Magician:
                MagicianPanel.SetActive(true);
                stateMachine.skills = magicianSkills;
                AssignIcons(magicianSkills);
                break;
            case Player.PlayerClass.Archer:
                ArcherPanel.SetActive(true);
                stateMachine.skills = archerSkills;
                AssignIcons(archerSkills);
                break;
            case Player.PlayerClass.Rogue:
                RoguePanel.SetActive(true);
                stateMachine.skills = rogueSkills;
                AssignIcons(rogueSkills);
                break;
        }
    }

    void AssignIcons(SkillPanel skills)
    {
        if (player.BasicIndex > -1)
        {
            for (int i = 0; i < skillBar_Basic.Length; i++)
            {
                skillBar_Basic[i].color = Color.white;
                skillBar_Basic[i].sprite = skills.basicAbilities[player.BasicIndex].SkillIcon;
            }
        }

        if (player.OffensiveIndex > -1)
        {
            for (int i = 0; i < skillBar_Basic.Length; i++)
            {
                skillBar_Offensive[i].color = Color.white;
                skillBar_Offensive[i].sprite = skills.offensiveAbilities[player.OffensiveIndex].SkillIcon;
            }
        }

        if (player.MobilityIndex > -1)
        {
            for (int i = 0; i < skillBar_Basic.Length; i++)
            {
                skillBar_Mobility[i].color = Color.white;
                skillBar_Mobility[i].sprite = skills.mobilityAbilities[player.MobilityIndex].SkillIcon;
            }
        }

        if (player.DefensiveIndex > -1)
        {
            for (int i = 0; i < skillBar_Basic.Length; i++)
            {
                skillBar_Defensive[i].color = Color.white;
                skillBar_Defensive[i].sprite = skills.defensiveAbilities[player.DefensiveIndex].SkillIcon;
            }
        }

        if (player.UtilityIndex > -1)
        {
            for (int i = 0; i < skillBar_Basic.Length; i++)
            {
                skillBar_Utility[i].color = Color.white;
                skillBar_Utility[i].sprite = skills.utilityAbilities[player.UtilityIndex].SkillIcon;
            }
        }

        if (player.UltimateIndex > -1)
        {
            for (int i = 0; i < skillBar_Basic.Length; i++)
            {
                skillBar_Ultimate[i].color = Color.white;
                skillBar_Ultimate[i].sprite = skills.ultimateAbilities[player.UltimateIndex].SkillIcon;
            }
        }
    }

    public void OnLevelUp()
    {
        if (player.PlayerLevel.Value >= 1)
        {
            for (int i = 0; i < skillBar_Basic_Lock.Length; i++)
            {
                skillBar_Basic_Lock[i].gameObject.SetActive(false);
            }
        }

        if (player.PlayerLevel.Value >= 4)
        {
            for (int i = 0; i < skillBar_Offensive_Lock.Length; i++)
            {
                skillBar_Offensive_Lock[i].gameObject.SetActive(false);
            }
        }

        if (player.PlayerLevel.Value >= 8)
        {
            for (int i = 0; i < skillBar_Mobility_Lock.Length; i++)
            {
                skillBar_Mobility_Lock[i].gameObject.SetActive(false);
            }
        }

        if (player.PlayerLevel.Value >= 12)
        {
            for (int i = 0; i < skillBar_Defensive_Lock.Length; i++)
            {
                skillBar_Defensive_Lock[i].gameObject.SetActive(false);
            }
        }

        if (player.PlayerLevel.Value >= 16)
        {
            for (int i = 0; i < skillBar_Utility_Lock.Length; i++)
            {
                skillBar_Utility_Lock[i].gameObject.SetActive(false);
            }
        }

        if (player.PlayerLevel.Value >= 20)
        {
            for (int i = 0; i < skillBar_Ultimate_Lock.Length; i++)
            {
                skillBar_Ultimate_Lock[i].gameObject.SetActive(false);
            }
        }
    }
}
