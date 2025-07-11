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
        switch (player.playerClass)
        {
            case Player.PlayerClass.Beginner:
                ToolTip(beginnerSkills, skillType);

                break;
            case Player.PlayerClass.Warrior:
                ToolTip(warriorSkills, skillType);

                break;
            case Player.PlayerClass.Magician:
                ToolTip(magicianSkills, skillType);

                break;
            case Player.PlayerClass.Archer:
                ToolTip(archerSkills, skillType);

                break;
            case Player.PlayerClass.Rogue:
                ToolTip(rogueSkills, skillType);

                break;
        }

        tooltip.SetActive(true);
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

                icon.sprite = classPanel.firstPassive[player.FirstPassiveIndex].SkillIcon;
                skillName.text = "[Passive] " + classPanel.firstPassive[player.FirstPassiveIndex].name;
                description.text = classPanel.firstPassive[player.FirstPassiveIndex].Description;
                cooldown.text = "Cooldown: " + classPanel.firstPassive[player.FirstPassiveIndex].CoolDown;

                break;
            case SkillType.Passive2:

                icon.sprite = classPanel.secondPassive[player.SecondPassiveIndex].SkillIcon;
                skillName.text = "[Passive] " + classPanel.secondPassive[player.SecondPassiveIndex].name;
                description.text = classPanel.secondPassive[player.SecondPassiveIndex].Description;
                cooldown.text = "Cooldown: " + classPanel.secondPassive[player.SecondPassiveIndex].CoolDown;

                break;
            case SkillType.Passive3:

                icon.sprite = classPanel.thirdPassive[player.ThirdPassiveIndex].SkillIcon;
                skillName.text = "[Passive] " + classPanel.thirdPassive[player.ThirdPassiveIndex].name;
                description.text = classPanel.thirdPassive[player.ThirdPassiveIndex].Description;
                cooldown.text = "Cooldown: " + classPanel.thirdPassive[player.ThirdPassiveIndex].CoolDown;

                break;
            case SkillType.Basic:

                icon.sprite = classPanel.basicAbilities[player.BasicIndex].SkillIcon;
                skillName.text = "[Passive] " + classPanel.basicAbilities[player.BasicIndex].name;
                description.text = classPanel.basicAbilities[player.BasicIndex].Description;
                cooldown.text = "Cooldown: " + classPanel.basicAbilities[player.BasicIndex].CoolDown;

                break;
            case SkillType.Offensive:

                icon.sprite = classPanel.offensiveAbilities[player.OffensiveIndex].SkillIcon;
                skillName.text = "[Passive] " + classPanel.offensiveAbilities[player.OffensiveIndex].name;
                description.text = classPanel.offensiveAbilities[player.OffensiveIndex].Description;
                cooldown.text = "Cooldown: " + classPanel.offensiveAbilities[player.OffensiveIndex].CoolDown;

                break;
            case SkillType.Mobility:

                icon.sprite = classPanel.mobilityAbilities[player.MobilityIndex].SkillIcon;
                skillName.text = "[Passive] " + classPanel.mobilityAbilities[player.MobilityIndex].name;
                description.text = classPanel.mobilityAbilities[player.MobilityIndex].Description;
                cooldown.text = "Cooldown: " + classPanel.mobilityAbilities[player.MobilityIndex].CoolDown;

                break;
            case SkillType.Defensive:

                icon.sprite = classPanel.defensiveAbilities[player.DefensiveIndex].SkillIcon;
                skillName.text = "[Passive] " + classPanel.defensiveAbilities[player.DefensiveIndex].name;
                description.text = classPanel.defensiveAbilities[player.DefensiveIndex].Description;
                cooldown.text = "Cooldown: " + classPanel.defensiveAbilities[player.DefensiveIndex].CoolDown;

                break;
            case SkillType.Utility:

                icon.sprite = classPanel.utilityAbilities[player.UtilityIndex].SkillIcon;
                skillName.text = "[Passive] " + classPanel.utilityAbilities[player.UtilityIndex].name;
                description.text = classPanel.utilityAbilities[player.UtilityIndex].Description;
                cooldown.text = "Cooldown: " + classPanel.utilityAbilities[player.UtilityIndex].CoolDown;

                break;
            case SkillType.Ultimate:

                icon.sprite = classPanel.ultimateAbilities[player.UltimateIndex].SkillIcon;
                skillName.text = "[Passive] " + classPanel.ultimateAbilities[player.UltimateIndex].name;
                description.text = classPanel.ultimateAbilities[player.UltimateIndex].Description;
                cooldown.text = "Cooldown: " + classPanel.ultimateAbilities[player.UltimateIndex].CoolDown;

                break;
        }
    }
}
