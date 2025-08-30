using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillPanelToolTip : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] GameObject tooltip;
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI skillName;
    [SerializeField] TextMeshProUGUI skillDescription;
    [SerializeField] TextMeshProUGUI skillCoolDown;

    private PlayerAbility ability;

    public void SetAbility(PlayerAbility ability)
    {
        this.ability = ability;
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (ability == null) return;

        icon.sprite = ability.SkillIcon;
        skillName.text = ability.name;
        skillDescription.text = ability.Description;
        skillCoolDown.text = "Cooldown: " + ability.CoolDown.ToString();

        tooltip.SetActive(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        tooltip.SetActive(false);
    }

    private void OnDisable()
    {
        tooltip.SetActive(false);
    }
}
