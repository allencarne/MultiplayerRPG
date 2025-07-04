using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillBarToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public enum SkillType
    {
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

                switch (skillType)
                {
                    case SkillType.Basic:

                        if (player.BasicIndex == -1) return;

                        switch (player.BasicIndex)
                        {
                            case 0:
                                icon.sprite = beginnerSkills.basicAbilities[0].SkillIcon;
                                skillName.text = "[BASIC] " + beginnerSkills.basicAbilities[0].name;
                                description.text = beginnerSkills.basicAbilities[0].Description;
                                cooldown.text = "Cooldown: " + beginnerSkills.basicAbilities[0].CoolDown;
                                break;
                            case 1:
                                icon.sprite = beginnerSkills.basicAbilities[1].SkillIcon;
                                skillName.text = "[BASIC] " + beginnerSkills.basicAbilities[1].name;
                                description.text = beginnerSkills.basicAbilities[1].Description;
                                cooldown.text = "Cooldown: " + beginnerSkills.basicAbilities[1].CoolDown;
                                break;

                            case 2:
                                icon.sprite = beginnerSkills.basicAbilities[2].SkillIcon;
                                skillName.text = "[BASIC] " + beginnerSkills.basicAbilities[2].name;
                                description.text = beginnerSkills.basicAbilities[2].Description;
                                cooldown.text = "Cooldown: " + beginnerSkills.basicAbilities[1].CoolDown;
                                break;
                        }

                        break;
                    case SkillType.Offensive:

                        if (player.OffensiveIndex == -1) return;

                        switch (player.OffensiveIndex)
                        {
                            case 0:
                                icon.sprite = beginnerSkills.offensiveAbilities[0].SkillIcon;
                                skillName.text = "[Offensive] " + beginnerSkills.offensiveAbilities[0].name;
                                description.text = beginnerSkills.offensiveAbilities[0].Description;
                                cooldown.text = "Cooldown: " + beginnerSkills.offensiveAbilities[0].CoolDown;
                                break;
                            case 1:
                                icon.sprite = beginnerSkills.offensiveAbilities[1].SkillIcon;
                                skillName.text = "[Offensive] " + beginnerSkills.offensiveAbilities[1].name;
                                description.text = beginnerSkills.offensiveAbilities[1].Description;
                                cooldown.text = "Cooldown: " + beginnerSkills.offensiveAbilities[1].CoolDown;
                                break;

                            case 2:
                                icon.sprite = beginnerSkills.offensiveAbilities[2].SkillIcon;
                                skillName.text = "[Offensive] " + beginnerSkills.offensiveAbilities[2].name;
                                description.text = beginnerSkills.offensiveAbilities[2].Description;
                                cooldown.text = "Cooldown: " + beginnerSkills.offensiveAbilities[2].CoolDown;
                                break;
                        }

                        break;
                    case SkillType.Mobility:

                        if (player.MobilityIndex == -1) return;

                        switch (player.MobilityIndex)
                        {
                            case 0:
                                icon.sprite = beginnerSkills.mobilityAbilities[0].SkillIcon;
                                skillName.text = "[Mobility] " + beginnerSkills.mobilityAbilities[0].name;
                                description.text = beginnerSkills.mobilityAbilities[0].Description;
                                cooldown.text = "Cooldown: " + beginnerSkills.mobilityAbilities[0].CoolDown;
                                break;
                            case 1:
                                icon.sprite = beginnerSkills.mobilityAbilities[1].SkillIcon;
                                skillName.text = "[Mobility] " + beginnerSkills.mobilityAbilities[1].name;
                                description.text = beginnerSkills.mobilityAbilities[1].Description;
                                cooldown.text = "Cooldown: " + beginnerSkills.mobilityAbilities[1].CoolDown;
                                break;

                            case 2:
                                icon.sprite = beginnerSkills.mobilityAbilities[2].SkillIcon;
                                skillName.text = "[Mobility] " + beginnerSkills.mobilityAbilities[2].name;
                                description.text = beginnerSkills.mobilityAbilities[2].Description;
                                cooldown.text = "Cooldown: " + beginnerSkills.mobilityAbilities[2].CoolDown;
                                break;
                        }

                        break;
                    case SkillType.Defensive:

                        if (player.DefensiveIndex == -1) return;

                        switch (player.MobilityIndex)
                        {
                            case 0:
                                icon.sprite = beginnerSkills.defensiveAbilities[0].SkillIcon;
                                skillName.text = "[Defensive] " + beginnerSkills.defensiveAbilities[0].name;
                                description.text = beginnerSkills.defensiveAbilities[0].Description;
                                cooldown.text = "Cooldown: " + beginnerSkills.defensiveAbilities[0].CoolDown;
                                break;
                            case 1:
                                icon.sprite = beginnerSkills.defensiveAbilities[1].SkillIcon;
                                skillName.text = "[Defensive] " + beginnerSkills.defensiveAbilities[1].name;
                                description.text = beginnerSkills.defensiveAbilities[1].Description;
                                cooldown.text = "Cooldown: " + beginnerSkills.defensiveAbilities[1].CoolDown;
                                break;

                            case 2:
                                icon.sprite = beginnerSkills.defensiveAbilities[2].SkillIcon;
                                skillName.text = "[Defensive] " + beginnerSkills.defensiveAbilities[2].name;
                                description.text = beginnerSkills.defensiveAbilities[2].Description;
                                cooldown.text = "Cooldown: " + beginnerSkills.defensiveAbilities[2].CoolDown;
                                break;
                        }

                        break;
                    case SkillType.Utility:

                        if (player.UtilityIndex == -1) return;

                        switch (player.MobilityIndex)
                        {
                            case 0:
                                icon.sprite = beginnerSkills.utilityAbilities[0].SkillIcon;
                                skillName.text = "[Utility] " + beginnerSkills.utilityAbilities[0].name;
                                description.text = beginnerSkills.utilityAbilities[0].Description;
                                cooldown.text = "Cooldown: " + beginnerSkills.utilityAbilities[0].CoolDown;
                                break;
                            case 1:
                                icon.sprite = beginnerSkills.utilityAbilities[1].SkillIcon;
                                skillName.text = "[Utility] " + beginnerSkills.utilityAbilities[1].name;
                                description.text = beginnerSkills.utilityAbilities[1].Description;
                                cooldown.text = "Cooldown: " + beginnerSkills.utilityAbilities[1].CoolDown;
                                break;

                            case 2:
                                icon.sprite = beginnerSkills.utilityAbilities[2].SkillIcon;
                                skillName.text = "[Utility] " + beginnerSkills.utilityAbilities[2].name;
                                description.text = beginnerSkills.utilityAbilities[2].Description;
                                cooldown.text = "Cooldown: " + beginnerSkills.utilityAbilities[2].CoolDown;
                                break;
                        }

                        break;
                    case SkillType.Ultimate:

                        if (player.UltimateIndex == -1) return;

                        switch (player.MobilityIndex)
                        {
                            case 0:
                                icon.sprite = beginnerSkills.ultimateAbilities[0].SkillIcon;
                                skillName.text = "[Ultimate] " + beginnerSkills.ultimateAbilities[0].name;
                                description.text = beginnerSkills.ultimateAbilities[0].Description;
                                cooldown.text = "Cooldown: " + beginnerSkills.ultimateAbilities[0].CoolDown;
                                break;
                            case 1:
                                icon.sprite = beginnerSkills.ultimateAbilities[1].SkillIcon;
                                skillName.text = "[Ultimate] " + beginnerSkills.ultimateAbilities[1].name;
                                description.text = beginnerSkills.ultimateAbilities[1].Description;
                                cooldown.text = "Cooldown: " + beginnerSkills.ultimateAbilities[1].CoolDown;
                                break;

                            case 2:
                                icon.sprite = beginnerSkills.ultimateAbilities[2].SkillIcon;
                                skillName.text = "[Ultimate] " + beginnerSkills.ultimateAbilities[2].name;
                                description.text = beginnerSkills.ultimateAbilities[2].Description;
                                cooldown.text = "Cooldown: " + beginnerSkills.ultimateAbilities[2].CoolDown;
                                break;
                        }

                        break;
                }

                break;
            case Player.PlayerClass.Warrior:


                break;
            case Player.PlayerClass.Magician:
                break;
            case Player.PlayerClass.Archer:
                break;
            case Player.PlayerClass.Rogue:
                break;
        }

        tooltip.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.SetActive(false);
    }
}
