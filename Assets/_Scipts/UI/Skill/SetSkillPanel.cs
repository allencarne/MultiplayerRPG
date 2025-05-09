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

    [SerializeField] SkillPanel begginerSkills;
    [SerializeField] SkillPanel warriorSkills;
    [SerializeField] SkillPanel magicianSkills;
    [SerializeField] SkillPanel archerSkills;
    [SerializeField] SkillPanel rogueSkills;

    [Header("Ability Bar")]
    [SerializeField] Image skillBar_Basic;
    [SerializeField] Image skillBar_Offensive;
    [SerializeField] Image skillBar_Mobility;
    [SerializeField] Image skillBar_Defensive;
    [SerializeField] Image skillBar_Utility;
    [SerializeField] Image skillBar_Ultimate;

    private void Start()
    {
        BeginnerPanel.SetActive(false);
        WarriorPanel.SetActive(false);
        MagicianPanel.SetActive(false);
        ArcherPanel.SetActive(false);
        RoguePanel.SetActive(false);

        GetPlayerClass();
    }

    public void GetPlayerClass()
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
            skillBar_Basic.color = Color.white;
            skillBar_Basic.sprite = skills.basicAbilities[player.BasicIndex].SkillIcon;
        }

        if (player.OffensiveIndex > -1)
        {
            skillBar_Offensive.color = Color.white;
            skillBar_Offensive.sprite = skills.offensiveAbilities[player.OffensiveIndex].SkillIcon;
        }

        if (player.MobilityIndex > -1)
        {
            skillBar_Mobility.color = Color.white;
            skillBar_Mobility.sprite = skills.mobilityAbilities[player.MobilityIndex].SkillIcon;
        }

        if (player.DefensiveIndex > -1)
        {
            skillBar_Defensive.color = Color.white;
            skillBar_Defensive.sprite = skills.defensiveAbilities[player.DefensiveIndex].SkillIcon;
        }

        if (player.UtilityIndex > -1)
        {
            skillBar_Utility.color = Color.white;
            skillBar_Utility.sprite = skills.utilityAbilities[player.UtilityIndex].SkillIcon;
        }

        if (player.UltimateIndex > -1)
        {
            skillBar_Ultimate.color = Color.white;
            skillBar_Ultimate.sprite = skills.ultimateAbilities[player.UltimateIndex].SkillIcon;
        }
    }
}
