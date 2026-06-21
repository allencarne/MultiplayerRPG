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

    [SerializeField] SkillPanel beginnerSkills;
    [SerializeField] SkillPanel warriorSkills;
    [SerializeField] SkillPanel magicianSkills;
    [SerializeField] SkillPanel archerSkills;
    [SerializeField] SkillPanel rogueSkills;

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

    void ToolTip(SkillPanel classPanel, SkillType type)
    {
        switch (type)
        {
            case SkillType.Passive1:

                if (player.FirstPassiveIndex < 0) return;
                icon.sprite = classPanel.firstPassive[player.FirstPassiveIndex].skillData.SkillIcon;
                skillName.text = "[Passive] " + classPanel.firstPassive[player.FirstPassiveIndex].name;
                description.text = classPanel.firstPassive[player.FirstPassiveIndex].skillData.Description;
                cooldown.text = "Cooldown: " + classPanel.firstPassive[player.FirstPassiveIndex].skillData.CoolDown;
                tooltip.SetActive(true);
                break;
            case SkillType.Passive2:

                if (player.SecondPassiveIndex < 0) return;
                icon.sprite = classPanel.secondPassive[player.SecondPassiveIndex].skillData.SkillIcon;
                skillName.text = "[Passive] " + classPanel.secondPassive[player.SecondPassiveIndex].name;
                description.text = classPanel.secondPassive[player.SecondPassiveIndex].skillData.Description;
                cooldown.text = "Cooldown: " + classPanel.secondPassive[player.SecondPassiveIndex].skillData.CoolDown;
                tooltip.SetActive(true);
                break;
            case SkillType.Passive3:

                if (player.ThirdPassiveIndex < 0) return;
                icon.sprite = classPanel.thirdPassive[player.ThirdPassiveIndex].skillData.SkillIcon;
                skillName.text = "[Passive] " + classPanel.thirdPassive[player.ThirdPassiveIndex].name;
                description.text = classPanel.thirdPassive[player.ThirdPassiveIndex].skillData.Description;
                cooldown.text = "Cooldown: " + classPanel.thirdPassive[player.ThirdPassiveIndex].skillData.CoolDown;
                tooltip.SetActive(true);
                break;
            case SkillType.Basic:

                if (player.BasicIndex < 0) return;
                icon.sprite = classPanel.basicAbilities[player.BasicIndex].skillData.SkillIcon;
                skillName.text = "[Basic] " + classPanel.basicAbilities[player.BasicIndex].name;
                description.text = classPanel.basicAbilities[player.BasicIndex].skillData.Description;
                cooldown.text = "Cooldown: " + classPanel.basicAbilities[player.BasicIndex].skillData.CoolDown;
                tooltip.SetActive(true);
                break;
            case SkillType.Offensive:

                if (player.OffensiveIndex < 0) return;
                icon.sprite = classPanel.offensiveAbilities[player.OffensiveIndex].skillData.SkillIcon;
                skillName.text = "[Offensive] " + classPanel.offensiveAbilities[player.OffensiveIndex].name;
                description.text = classPanel.offensiveAbilities[player.OffensiveIndex].skillData.Description;
                cooldown.text = "Cooldown: " + classPanel.offensiveAbilities[player.OffensiveIndex].skillData.CoolDown;
                tooltip.SetActive(true);
                break;
            case SkillType.Mobility:

                if (player.MobilityIndex < 0) return;
                icon.sprite = classPanel.mobilityAbilities[player.MobilityIndex].skillData.SkillIcon;
                skillName.text = "[Mobility] " + classPanel.mobilityAbilities[player.MobilityIndex].name;
                description.text = classPanel.mobilityAbilities[player.MobilityIndex].skillData.Description;
                cooldown.text = "Cooldown: " + classPanel.mobilityAbilities[player.MobilityIndex].skillData.CoolDown;
                tooltip.SetActive(true);
                break;
            case SkillType.Defensive:

                if (player.DefensiveIndex < 0) return;
                icon.sprite = classPanel.defensiveAbilities[player.DefensiveIndex].skillData.SkillIcon;
                skillName.text = "[Defensive] " + classPanel.defensiveAbilities[player.DefensiveIndex].name;
                description.text = classPanel.defensiveAbilities[player.DefensiveIndex].skillData.Description;
                cooldown.text = "Cooldown: " + classPanel.defensiveAbilities[player.DefensiveIndex].skillData.CoolDown;
                tooltip.SetActive(true);
                break;
            case SkillType.Utility:

                if (player.UtilityIndex < 0) return;
                icon.sprite = classPanel.utilityAbilities[player.UtilityIndex].skillData.SkillIcon;
                skillName.text = "[Utility] " + classPanel.utilityAbilities[player.UtilityIndex].name;
                description.text = classPanel.utilityAbilities[player.UtilityIndex].skillData.Description;
                cooldown.text = "Cooldown: " + classPanel.utilityAbilities[player.UtilityIndex].skillData.CoolDown;
                tooltip.SetActive(true);
                break;
            case SkillType.Ultimate:

                if (player.UltimateIndex < 0) return;
                icon.sprite = classPanel.ultimateAbilities[player.UltimateIndex].skillData.SkillIcon;
                skillName.text = "[Ultimate] " + classPanel.ultimateAbilities[player.UltimateIndex].name;
                description.text = classPanel.ultimateAbilities[player.UltimateIndex].skillData.Description;
                cooldown.text = "Cooldown: " + classPanel.ultimateAbilities[player.UltimateIndex].skillData.CoolDown;
                tooltip.SetActive(true);
                break;
        }
    }
}
