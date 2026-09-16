using UnityEngine;
using UnityEngine.EventSystems;

public class SkillPanelToolTip : MonoBehaviour, ISelectHandler, IDeselectHandler, ISubmitHandler, IPointerClickHandler, ICancelHandler
{
    [SerializeField] Player player;
    SkillData skillData;

    public void SetSkillData(SkillData data)
    {
        skillData = data;
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (skillData == null) return;
        player.ShowSkillToolTip(skillData);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        player.HideToolTip();
    }

    private void OnDisable()
    {
        player.HideToolTip();
    }

    public void OnSubmit(BaseEventData eventData)
    {
        player.HideToolTip();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        player.HideToolTip();
    }

    public void OnCancel(BaseEventData eventData)
    {
        player.HideToolTip();
    }
}
