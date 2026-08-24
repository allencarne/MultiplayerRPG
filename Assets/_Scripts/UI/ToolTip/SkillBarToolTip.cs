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

    [SerializeField] GameObject tooltip;
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
        tooltip.SetActive(false);
    }

    void ToolTip(ClassSkillSet classSkill, SkillType type)
    {
        switch (type)
        {
            case SkillType.Passive1:

                if (player.FirstPassiveIndex < 0) return;
                icon.sprite = classSkill.firstPassive[player.FirstPassiveIndex].Icon;
                skillName.text = "[Passive] " + classSkill.firstPassive[player.FirstPassiveIndex].name;
                description.text = classSkill.firstPassive[player.FirstPassiveIndex].Description;
                cooldown.text = "Cooldown: " + classSkill.firstPassive[player.FirstPassiveIndex].CoolDown;
                tooltip.SetActive(true);
                break;
            case SkillType.Passive2:

                if (player.SecondPassiveIndex < 0) return;
                icon.sprite = classSkill.secondPassive[player.SecondPassiveIndex].Icon;
                skillName.text = "[Passive] " + classSkill.secondPassive[player.SecondPassiveIndex].name;
                description.text = classSkill.secondPassive[player.SecondPassiveIndex].Description;
                cooldown.text = "Cooldown: " + classSkill.secondPassive[player.SecondPassiveIndex].CoolDown;
                tooltip.SetActive(true);
                break;
            case SkillType.Passive3:

                if (player.ThirdPassiveIndex < 0) return;
                icon.sprite = classSkill.thirdPassive[player.ThirdPassiveIndex].Icon;
                skillName.text = "[Passive] " + classSkill.thirdPassive[player.ThirdPassiveIndex].name;
                description.text = classSkill.thirdPassive[player.ThirdPassiveIndex].Description;
                cooldown.text = "Cooldown: " + classSkill.thirdPassive[player.ThirdPassiveIndex].CoolDown;
                tooltip.SetActive(true);
                break;
            case SkillType.Basic:

                if (player.BasicIndex < 0) return;
                icon.sprite = classSkill.basicAbilities[player.BasicIndex].Icon;
                skillName.text = "[Basic] " + classSkill.basicAbilities[player.BasicIndex].name;
                description.text = classSkill.basicAbilities[player.BasicIndex].Description;
                cooldown.text = "Cooldown: " + classSkill.basicAbilities[player.BasicIndex].CoolDown;
                tooltip.SetActive(true);
                break;
            case SkillType.Offensive:

                if (player.OffensiveIndex < 0) return;
                icon.sprite = classSkill.offensiveAbilities[player.OffensiveIndex].Icon;
                skillName.text = "[Offensive] " + classSkill.offensiveAbilities[player.OffensiveIndex].name;
                description.text = classSkill.offensiveAbilities[player.OffensiveIndex].Description;
                cooldown.text = "Cooldown: " + classSkill.offensiveAbilities[player.OffensiveIndex].CoolDown;
                tooltip.SetActive(true);
                break;
            case SkillType.Mobility:

                if (player.MobilityIndex < 0) return;
                icon.sprite = classSkill.mobilityAbilities[player.MobilityIndex].Icon;
                skillName.text = "[Mobility] " + classSkill.mobilityAbilities[player.MobilityIndex].name;
                description.text = classSkill.mobilityAbilities[player.MobilityIndex].Description;
                cooldown.text = "Cooldown: " + classSkill.mobilityAbilities[player.MobilityIndex].CoolDown;
                tooltip.SetActive(true);
                break;
            case SkillType.Defensive:

                if (player.DefensiveIndex < 0) return;
                icon.sprite = classSkill.defensiveAbilities[player.DefensiveIndex].Icon;
                skillName.text = "[Defensive] " + classSkill.defensiveAbilities[player.DefensiveIndex].name;
                description.text = classSkill.defensiveAbilities[player.DefensiveIndex].Description;
                cooldown.text = "Cooldown: " + classSkill.defensiveAbilities[player.DefensiveIndex].CoolDown;
                tooltip.SetActive(true);
                break;
            case SkillType.Utility:

                if (player.UtilityIndex < 0) return;
                icon.sprite = classSkill.utilityAbilities[player.UtilityIndex].Icon;
                skillName.text = "[Utility] " + classSkill.utilityAbilities[player.UtilityIndex].name;
                description.text = classSkill.utilityAbilities[player.UtilityIndex].Description;
                cooldown.text = "Cooldown: " + classSkill.utilityAbilities[player.UtilityIndex].CoolDown;
                tooltip.SetActive(true);
                break;
            case SkillType.Ultimate:

                if (player.UltimateIndex < 0) return;
                icon.sprite = classSkill.ultimateAbilities[player.UltimateIndex].Icon;
                skillName.text = "[Ultimate] " + classSkill.ultimateAbilities[player.UltimateIndex].name;
                description.text = classSkill.ultimateAbilities[player.UltimateIndex].Description;
                cooldown.text = "Cooldown: " + classSkill.ultimateAbilities[player.UltimateIndex].CoolDown;
                tooltip.SetActive(true);
                break;
        }
    }
}
