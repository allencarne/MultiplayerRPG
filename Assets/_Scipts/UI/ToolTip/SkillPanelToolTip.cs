using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillPanelToolTip : MonoBehaviour, ISelectHandler, IDeselectHandler
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

    [Header("Player")]
    [SerializeField] Player player;
    [SerializeField] int skillIndex;
    [SerializeField] SkillType skillType;

    [Header("Skills")]
    [SerializeField] SkillPanel beginnerSkills;
    [SerializeField] SkillPanel warriorSkills;
    [SerializeField] SkillPanel magicianSkills;
    [SerializeField] SkillPanel archerSkills;
    [SerializeField] SkillPanel rogueSkills;

    [Header("UI")]
    [SerializeField] Image icon;
    [SerializeField] GameObject tooltip;
    [SerializeField] TextMeshProUGUI skillName;
    [SerializeField] TextMeshProUGUI description;
    [SerializeField] TextMeshProUGUI cooldown;

    public void OnSelect(BaseEventData eventData)
    {
        switch (player.playerClass)
        {
            case Player.PlayerClass.Beginner: ToolTip(skillIndex, beginnerSkills, skillType); break;
            case Player.PlayerClass.Warrior: ToolTip(skillIndex, warriorSkills, skillType); break;
            case Player.PlayerClass.Magician: ToolTip(skillIndex, magicianSkills, skillType); break;
            case Player.PlayerClass.Archer: ToolTip(skillIndex, archerSkills, skillType); break;
            case Player.PlayerClass.Rogue: ToolTip(skillIndex, rogueSkills, skillType); break;
        }

        tooltip.SetActive(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        tooltip.SetActive(false);
    }

    void ToolTip(int index, SkillPanel classPanel, SkillType type)
    {
        switch (type)
        {
            case SkillType.Passive1:

                icon.sprite = classPanel.firstPassive[index].SkillIcon;
                skillName.text = "[Passive] " + classPanel.firstPassive[index].name;
                description.text = classPanel.firstPassive[index].Description;
                cooldown.text = "Cooldown: " + classPanel.firstPassive[index].CoolDown;

                break;
            case SkillType.Passive2:

                icon.sprite = classPanel.secondPassive[index].SkillIcon;
                skillName.text = "[Passive] " + classPanel.secondPassive[index].name;
                description.text = classPanel.secondPassive[index].Description;
                cooldown.text = "Cooldown: " + classPanel.secondPassive[index].CoolDown;

                break;
            case SkillType.Passive3:

                icon.sprite = classPanel.thirdPassive[index].SkillIcon;
                skillName.text = "[Passive] " + classPanel.thirdPassive[index].name;
                description.text = classPanel.thirdPassive[index].Description;
                cooldown.text = "Cooldown: " + classPanel.thirdPassive[index].CoolDown;

                break;
            case SkillType.Basic:

                icon.sprite = classPanel.basicAbilities[index].SkillIcon;
                skillName.text = "[Basic] " + classPanel.basicAbilities[index].name;
                description.text = classPanel.basicAbilities[index].Description;
                cooldown.text = "Cooldown: " + classPanel.basicAbilities[index].CoolDown;

                break;
            case SkillType.Offensive:

                icon.sprite = classPanel.offensiveAbilities[index].SkillIcon;
                skillName.text = "[Offensive] " + classPanel.offensiveAbilities[index].name;
                description.text = classPanel.offensiveAbilities[index].Description;
                cooldown.text = "Cooldown: " + classPanel.offensiveAbilities[index].CoolDown;

                break;
            case SkillType.Mobility:

                icon.sprite = classPanel.mobilityAbilities[index].SkillIcon;
                skillName.text = "[Mobility] " + classPanel.mobilityAbilities[index].name;
                description.text = classPanel.mobilityAbilities[index].Description;
                cooldown.text = "Cooldown: " + classPanel.mobilityAbilities[index].CoolDown;

                break;
            case SkillType.Defensive:

                icon.sprite = classPanel.defensiveAbilities[index].SkillIcon;
                skillName.text = "[Defensive] " + classPanel.defensiveAbilities[index].name;
                description.text = classPanel.defensiveAbilities[index].Description;
                cooldown.text = "Cooldown: " + classPanel.defensiveAbilities[index].CoolDown;

                break;
            case SkillType.Utility:

                icon.sprite = classPanel.utilityAbilities[index].SkillIcon;
                skillName.text = "[Utility] " + classPanel.utilityAbilities[index].name;
                description.text = classPanel.utilityAbilities[index].Description;
                cooldown.text = "Cooldown: " + classPanel.utilityAbilities[index].CoolDown;

                break;
            case SkillType.Ultimate:

                icon.sprite = classPanel.ultimateAbilities[index].SkillIcon;
                skillName.text = "[Ultimate] " + classPanel.ultimateAbilities[index].name;
                description.text = classPanel.ultimateAbilities[index].Description;
                cooldown.text = "Cooldown: " + classPanel.ultimateAbilities[index].CoolDown;

                break;
        }
    }
}
