using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SkillPanelUI : MonoBehaviour
{
    [SerializeField] PlayerStateMachine stateMachine;
    [SerializeField] Player player;
    [SerializeField] PlayerStats stats;
    [SerializeField] PlayerExperience exp;
    ClassSkillSet skillSet;

    [Header("FirstPassive")]
    [SerializeField] Image icon_FirstPassive0;
    [SerializeField] Image icon_FirstPassive1;
    [SerializeField] Image icon_FirstPassive2;
    [Header("SecondPassive")]
    [SerializeField] Image icon_SecondPassive0;
    [SerializeField] Image icon_SecondPassive1;
    [SerializeField] Image icon_SecondPassive2;
    [Header("ThirdPassive")]
    [SerializeField] Image icon_ThirdPassive0;
    [SerializeField] Image icon_ThirdPassive1;
    [SerializeField] Image icon_ThirdPassive2;
    [Header("Basic")]
    [SerializeField] Image icon_Basic0;
    [SerializeField] Image icon_Basic1;
    [SerializeField] Image icon_Basic2;
    [Header("Offensive 1")]
    [SerializeField] Image icon_Offensive0;
    [SerializeField] Image icon_Offensive1;
    [SerializeField] Image icon_Offensive2;
    [Header("Mobility")]
    [SerializeField] Image icon_Mobility0;
    [SerializeField] Image icon_Mobility1;
    [SerializeField] Image icon_Mobility2;
    [Header("Defensive")]
    [SerializeField] Image icon_Defensive0;
    [SerializeField] Image icon_Defensive1;
    [SerializeField] Image icon_Defensive2;
    [Header("Utility")]
    [SerializeField] Image icon_Utility0;
    [SerializeField] Image icon_Utility1;
    [SerializeField] Image icon_Utility2;
    [Header("Ultimate")]
    [SerializeField] Image icon_Ultimate0;
    [SerializeField] Image icon_Ultimate1;
    [SerializeField] Image icon_Ultimate2;

    [Header("Ability Bar Locks")]
    [SerializeField] Image[] skillBar_Basic_Lock;
    [SerializeField] Image[] skillBar_Offensive_Lock;
    [SerializeField] Image[] skillBar_Mobility_Lock;
    [SerializeField] Image[] skillBar_Defensive_Lock;
    [SerializeField] Image[] skillBar_Utility_Lock;
    [SerializeField] Image[] skillBar_Ultimate_Lock;

    [HideInInspector] public UnityEvent OnSkillSelected;

    private void OnEnable()
    {
        stats.PlayerLevel.OnValueChanged += OnLevelChanged;
        InvokeRepeating("SetYellowBorders", 0, 1);
        InvokeRepeating("SetBlueBorders", 0, 1);
    }

    private void OnDisable()
    {
        stats.PlayerLevel.OnValueChanged -= OnLevelChanged;
        CancelInvoke();
    }

    void SetColor(Image icon, Color color)
    {
        if (icon == null) return;

        Button button = icon.GetComponentInParent<Button>();
        if (button != null)
        {
            ColorBlock colors = button.colors;
            colors.normalColor = color;
            button.colors = colors;
        }
    }

    public void Bind(ClassSkillSet set)
    {
        skillSet = set;
        SetIcons();
    }

    void SetIcons()
    {
        AssignPassiveIcon(icon_FirstPassive0, skillSet.firstPassive, 0);
        AssignPassiveIcon(icon_FirstPassive1, skillSet.firstPassive, 1);
        AssignPassiveIcon(icon_FirstPassive2, skillSet.firstPassive, 2);
        AssignPassiveIcon(icon_SecondPassive0, skillSet.secondPassive, 0);
        AssignPassiveIcon(icon_SecondPassive1, skillSet.secondPassive, 1);
        AssignPassiveIcon(icon_SecondPassive2, skillSet.secondPassive, 2);
        AssignPassiveIcon(icon_ThirdPassive0, skillSet.thirdPassive, 0);
        AssignPassiveIcon(icon_ThirdPassive1, skillSet.thirdPassive, 1);
        AssignPassiveIcon(icon_ThirdPassive2, skillSet.thirdPassive, 2);

        AssignIcon(icon_Basic0, skillSet.basicAbilities, 0);
        AssignIcon(icon_Basic1, skillSet.basicAbilities, 1);
        AssignIcon(icon_Basic2, skillSet.basicAbilities, 2);
        AssignIcon(icon_Offensive0, skillSet.offensiveAbilities, 0);
        AssignIcon(icon_Offensive1, skillSet.offensiveAbilities, 1);
        AssignIcon(icon_Offensive2, skillSet.offensiveAbilities, 2);
        AssignIcon(icon_Mobility0, skillSet.mobilityAbilities, 0);
        AssignIcon(icon_Mobility1, skillSet.mobilityAbilities, 1);
        AssignIcon(icon_Mobility2, skillSet.mobilityAbilities, 2);
        AssignIcon(icon_Defensive0, skillSet.defensiveAbilities, 0);
        AssignIcon(icon_Defensive1, skillSet.defensiveAbilities, 1);
        AssignIcon(icon_Defensive2, skillSet.defensiveAbilities, 2);
        AssignIcon(icon_Utility0, skillSet.utilityAbilities, 0);
        AssignIcon(icon_Utility1, skillSet.utilityAbilities, 1);
        AssignIcon(icon_Utility2, skillSet.utilityAbilities, 2);
        AssignIcon(icon_Ultimate0, skillSet.ultimateAbilities, 0);
        AssignIcon(icon_Ultimate1, skillSet.ultimateAbilities, 1);
        AssignIcon(icon_Ultimate2, skillSet.ultimateAbilities, 2);
    }

    public void SetYellowBorders()
    {
        YellowBorder(player.FirstPassiveIndex, skillSet.passive1Req, icon_FirstPassive0);
        YellowBorder(player.FirstPassiveIndex, skillSet.passive1Req, icon_FirstPassive1);
        YellowBorder(player.FirstPassiveIndex, skillSet.passive1Req, icon_FirstPassive2);
        YellowBorder(player.SecondPassiveIndex, skillSet.passive2Req, icon_SecondPassive0);
        YellowBorder(player.SecondPassiveIndex, skillSet.passive2Req, icon_SecondPassive1);
        YellowBorder(player.SecondPassiveIndex, skillSet.passive2Req, icon_SecondPassive2);
        YellowBorder(player.ThirdPassiveIndex, skillSet.passive3Req, icon_ThirdPassive0);
        YellowBorder(player.ThirdPassiveIndex, skillSet.passive3Req, icon_ThirdPassive1);
        YellowBorder(player.ThirdPassiveIndex, skillSet.passive3Req, icon_ThirdPassive2);

        YellowBorder(player.BasicIndex, skillSet.basicReq, icon_Basic0);
        YellowBorder(player.BasicIndex, skillSet.basicReq, icon_Basic1);
        YellowBorder(player.BasicIndex, skillSet.basicReq, icon_Basic2);
        YellowBorder(player.OffensiveIndex, skillSet.offensiveReq, icon_Offensive0);
        YellowBorder(player.OffensiveIndex, skillSet.offensiveReq, icon_Offensive1);
        YellowBorder(player.OffensiveIndex, skillSet.offensiveReq, icon_Offensive2);
        YellowBorder(player.MobilityIndex, skillSet.mobilityReq, icon_Mobility0);
        YellowBorder(player.MobilityIndex, skillSet.mobilityReq, icon_Mobility1);
        YellowBorder(player.MobilityIndex, skillSet.mobilityReq, icon_Mobility2);
        YellowBorder(player.DefensiveIndex, skillSet.defensiveReq, icon_Defensive0);
        YellowBorder(player.DefensiveIndex, skillSet.defensiveReq, icon_Defensive1);
        YellowBorder(player.DefensiveIndex, skillSet.defensiveReq, icon_Defensive2);
        YellowBorder(player.UtilityIndex, skillSet.utilityReq, icon_Utility0);
        YellowBorder(player.UtilityIndex, skillSet.utilityReq, icon_Utility1);
        YellowBorder(player.UtilityIndex, skillSet.utilityReq, icon_Utility2);
        YellowBorder(player.UltimateIndex, skillSet.ultimateReq, icon_Ultimate0);
        YellowBorder(player.UltimateIndex, skillSet.ultimateReq, icon_Ultimate1);
        YellowBorder(player.UltimateIndex, skillSet.ultimateReq, icon_Ultimate2);
    }

    public void SetBlueBorders()
    {
        BlueBorder(player.FirstPassiveIndex, icon_FirstPassive0, icon_FirstPassive1, icon_FirstPassive2);
        BlueBorder(player.SecondPassiveIndex, icon_SecondPassive0, icon_SecondPassive1, icon_SecondPassive2);
        BlueBorder(player.ThirdPassiveIndex, icon_ThirdPassive0, icon_ThirdPassive1, icon_ThirdPassive2);

        BlueBorder(player.BasicIndex, icon_Basic0, icon_Basic1, icon_Basic2);
        BlueBorder(player.OffensiveIndex, icon_Offensive0, icon_Offensive1, icon_Offensive2);
        BlueBorder(player.MobilityIndex, icon_Mobility0, icon_Mobility1, icon_Mobility2);
        BlueBorder(player.DefensiveIndex, icon_Defensive0, icon_Defensive1, icon_Defensive2);
        BlueBorder(player.UtilityIndex, icon_Utility0, icon_Utility1, icon_Utility2);
        BlueBorder(player.UltimateIndex, icon_Ultimate0, icon_Ultimate1, icon_Ultimate2);
    }

    private void AssignIcon(Image icon, ActiveSkillData[] abilities, int index)
    {
        if (icon == null || abilities == null || index >= abilities.Length || abilities[index] == null) return;

        if (abilities[index].Icon != null) icon.sprite = abilities[index].Icon;

        //SkillPanelToolTip tooltip = icon.GetComponentInParent<SkillPanelToolTip>();
        //if (tooltip != null) tooltip.SetAbility(abilities[index]);
    }

    private void AssignPassiveIcon(Image icon, PassiveSkillData[] abilities, int index)
    {
        if (icon == null || abilities == null || index >= abilities.Length || abilities[index] == null) return;

        if (abilities[index].Icon != null) icon.sprite = abilities[index].Icon;

        //SkillPanelToolTip tooltip = icon.GetComponentInParent<SkillPanelToolTip>();
        //if (tooltip != null) tooltip.SetAbility(abilities[index]);
    }

    void BlueBorder(int index, Image zero, Image one, Image two)
    {
        if (index < 0) return;

        switch (index)
        {
            case 0:
                SetColor(zero, Color.grey);
                SetColor(one, new Color(1f, 1f, 1f, 0f));
                SetColor(two, new Color(1f, 1f, 1f, 0f));
                break;
            case 1:
                SetColor(zero, new Color(1f, 1f, 1f, 0f));
                SetColor(one, Color.grey);
                SetColor(two, new Color(1f, 1f, 1f, 0f));
                break;
            case 2:
                SetColor(zero, new Color(1f, 1f, 1f, 0f));
                SetColor(one, new Color(1f, 1f, 1f, 0f));
                SetColor(two, Color.grey);
                break;
        }
    }

    void YellowBorder(int index, int reqLevel, Image icon)
    {
        if (icon == null) return;
        if (index > -1) return;
        if (stats.PlayerLevel.Value < reqLevel) return;

        SetColor(icon, Color.cyan);
    }

    public void FirstPassiveButton(int index)
    {
        player.FirstPassiveIndex = index;
        OnSkillSelected?.Invoke();
        BlueBorder(index, icon_FirstPassive0, icon_FirstPassive1, icon_FirstPassive2);
        stateMachine.SetFirstPassive(skillSet.firstPassive[index], index);
    }

    public void SecondPassiveButton(int index)
    {
        player.SecondPassiveIndex = index;
        OnSkillSelected?.Invoke();
        BlueBorder(index, icon_SecondPassive0, icon_SecondPassive1, icon_SecondPassive2);
        stateMachine.SetFirstPassive(skillSet.secondPassive[index], index);
    }

    public void ThirdPassiveButton(int index)
    {
        player.ThirdPassiveIndex = index;
        OnSkillSelected?.Invoke();
        BlueBorder(index, icon_ThirdPassive0, icon_ThirdPassive1, icon_ThirdPassive2);
        stateMachine.SetFirstPassive(skillSet.thirdPassive[index], index);
    }

    public void BasicButton(int index)
    {
        player.BasicIndex = index;
        OnSkillSelected?.Invoke();

        BlueBorder(index, icon_Basic0, icon_Basic1, icon_Basic2);
    }

    public void OffensiveButton(int index)
    {
        player.OffensiveIndex = index;
        OnSkillSelected?.Invoke();

        BlueBorder(index, icon_Offensive0, icon_Offensive1, icon_Offensive2);
    }

    public void MobilityButton(int index)
    {
        player.MobilityIndex = index;
        OnSkillSelected?.Invoke();

        BlueBorder(index, icon_Mobility0, icon_Mobility1, icon_Mobility2);
    }

    public void DefensiveButton(int index)
    {
        player.DefensiveIndex = index;
        OnSkillSelected?.Invoke();

        BlueBorder(index, icon_Defensive0, icon_Defensive1, icon_Defensive2);
    }

    public void UtilityButton(int index)
    {
        player.UtilityIndex = index;
        OnSkillSelected?.Invoke();

        BlueBorder(index, icon_Utility0, icon_Utility1, icon_Utility2);
    }

    public void UltimateButton(int index)
    {
        player.UltimateIndex = index;
        OnSkillSelected?.Invoke();

        BlueBorder(index, icon_Ultimate0, icon_Ultimate1, icon_Ultimate2);
    }

    void OnLevelChanged(int oldValue, int newValue)
    {
        OnLevelUp();
    }

    void OnLevelUp()
    {
        if (stats.PlayerLevel.Value >= 1)
        {
            for (int i = 0; i < skillBar_Basic_Lock.Length; i++)
            {
                //skillBar_Basic_Lock[i].gameObject.SetActive(false);
            }
        }

        if (stats.PlayerLevel.Value >= 4)
        {
            for (int i = 0; i < skillBar_Offensive_Lock.Length; i++)
            {
                //skillBar_Offensive_Lock[i].gameObject.SetActive(false);
            }
        }

        if (stats.PlayerLevel.Value >= 8)
        {
            for (int i = 0; i < skillBar_Mobility_Lock.Length; i++)
            {
                //skillBar_Mobility_Lock[i].gameObject.SetActive(false);
            }
        }

        if (stats.PlayerLevel.Value >= 12)
        {
            for (int i = 0; i < skillBar_Defensive_Lock.Length; i++)
            {
                //skillBar_Defensive_Lock[i].gameObject.SetActive(false);
            }
        }

        if (stats.PlayerLevel.Value >= 16)
        {
            for (int i = 0; i < skillBar_Utility_Lock.Length; i++)
            {
                //skillBar_Utility_Lock[i].gameObject.SetActive(false);
            }
        }

        if (stats.PlayerLevel.Value >= 20)
        {
            for (int i = 0; i < skillBar_Ultimate_Lock.Length; i++)
            {
                //skillBar_Ultimate_Lock[i].gameObject.SetActive(false);
            }
        }
    }
}
