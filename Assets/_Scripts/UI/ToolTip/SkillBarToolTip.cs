using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillBarToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public enum SkillType
    {
        Passive1,
        Passive2,
        Passive3,
        Basic,
        Offensive,
        Mobility,
        Defensive,
        Utility,
        Ultimate,
    }

    [SerializeField] SkillType skillType;

    [SerializeField] Player player;
    [SerializeField] PlayerStats stats;

    [SerializeField] ClassSkillSet beginnerSkills;
    [SerializeField] ClassSkillSet warriorSkills;
    [SerializeField] ClassSkillSet magicianSkills;
    [SerializeField] ClassSkillSet archerSkills;
    [SerializeField] ClassSkillSet rogueSkills;

    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI skillName;
    [SerializeField] TextMeshProUGUI description;
    [SerializeField] TextMeshProUGUI cooldown;

    public void OnPointerEnter(PointerEventData eventData)
    {
        switch (stats.playerClass)
        {
            case PlayerStats.PlayerClass.Beginner:
                ToolTip(beginnerSkills, skillType);

                break;
            case PlayerStats.PlayerClass.Warrior:
                ToolTip(warriorSkills, skillType);

                break;
            case PlayerStats.PlayerClass.Magician:
                ToolTip(magicianSkills, skillType);

                break;
            case PlayerStats.PlayerClass.Archer:
                ToolTip(archerSkills, skillType);

                break;
            case PlayerStats.PlayerClass.Rogue:
                ToolTip(rogueSkills, skillType);

                break;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        player.HideToolTip();
    }

    void ToolTip(ClassSkillSet classSkill, SkillType type)
    {
        if (player == null || classSkill == null) return;

        SkillData skill = null;

        switch (type)
        {
            case SkillType.Passive1:
                if (player.FirstPassiveIndex < 0) return;
                skill = classSkill.firstPassive[player.FirstPassiveIndex];
                break;
            case SkillType.Passive2:
                if (player.SecondPassiveIndex < 0) return;
                skill = classSkill.secondPassive[player.SecondPassiveIndex];
                break;
            case SkillType.Passive3:
                if (player.ThirdPassiveIndex < 0) return;
                skill = classSkill.thirdPassive[player.ThirdPassiveIndex];
                break;
            case SkillType.Basic:
                if (player.BasicIndex < 0) return;
                skill = classSkill.basicAbilities[player.BasicIndex];
                break;
            case SkillType.Offensive:
                if (player.OffensiveIndex < 0) return;
                skill = classSkill.offensiveAbilities[player.OffensiveIndex];
                break;
            case SkillType.Mobility:
                if (player.MobilityIndex < 0) return;
                skill = classSkill.mobilityAbilities[player.MobilityIndex];
                break;
            case SkillType.Defensive:
                if (player.DefensiveIndex < 0) return;
                skill = classSkill.defensiveAbilities[player.DefensiveIndex];
                break;
            case SkillType.Utility:
                if (player.UtilityIndex < 0) return;
                skill = classSkill.utilityAbilities[player.UtilityIndex];
                break;
            case SkillType.Ultimate:
                if (player.UltimateIndex < 0) return;
                skill = classSkill.ultimateAbilities[player.UltimateIndex];
                break;
        }

        if (skill != null)
        {
            player.ShowSkillToolTip(skill);
        }
    }
}
