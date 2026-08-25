using TMPro;
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

    [Header("Icons")]
    [SerializeField] GameObject[] object_Passive1;
    [SerializeField] GameObject[] object_Passive2;
    [SerializeField] GameObject[] object_Passive3;
    [SerializeField] GameObject[] object_Basic;
    [SerializeField] GameObject[] object_Offensive;
    [SerializeField] GameObject[] object_Mobility;
    [SerializeField] GameObject[] object_Defensive;
    [SerializeField] GameObject[] object_Utility;
    [SerializeField] GameObject[] object_Ultimate;

    [Header("Icons")]
    [SerializeField] Image[] icon_Passive1;
    [SerializeField] Image[] icon_Passive2;
    [SerializeField] Image[] icon_Passive3;
    [SerializeField] Image[] icon_Basic;
    [SerializeField] Image[] icon_Offensive;
    [SerializeField] Image[] icon_Mobility;
    [SerializeField] Image[] icon_Defensive;
    [SerializeField] Image[] icon_Utility;
    [SerializeField] Image[] icon_Ultimate;

    [Header("Icon Locks")]
    [SerializeField] Image[] icon_Passive1_Lock;
    [SerializeField] Image[] icon_Passive2_Lock;
    [SerializeField] Image[] icon_Passive3_Lock;
    [SerializeField] Image[] icon_Basic_Lock;
    [SerializeField] Image[] icon_Offensive_Lock;
    [SerializeField] Image[] icon_Mobility_Lock;
    [SerializeField] Image[] icon_Defensive_Lock;
    [SerializeField] Image[] icon_Utility_Lock;
    [SerializeField] Image[] icon_Ultimate_Lock;

    [Header("Text")]
    [SerializeField] TextMeshProUGUI[] text_Passive1;
    [SerializeField] TextMeshProUGUI[] text_Passive2;
    [SerializeField] TextMeshProUGUI[] text_Passive3;
    [SerializeField] TextMeshProUGUI[] text_Basic;
    [SerializeField] TextMeshProUGUI[] text_Offensive;
    [SerializeField] TextMeshProUGUI[] text_Mobility;
    [SerializeField] TextMeshProUGUI[] text_Defensive;
    [SerializeField] TextMeshProUGUI[] text_Utility;
    [SerializeField] TextMeshProUGUI[] text_Ultimate;

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
        AssignPassiveIcon(icon_Passive1[0], skillSet.firstPassive, 0);
        AssignPassiveIcon(icon_Passive1[1], skillSet.firstPassive, 1);
        AssignPassiveIcon(icon_Passive1[2], skillSet.firstPassive, 2);
        AssignPassiveIcon(icon_Passive2[0], skillSet.secondPassive, 0);
        AssignPassiveIcon(icon_Passive2[1], skillSet.secondPassive, 1);
        AssignPassiveIcon(icon_Passive2[2], skillSet.secondPassive, 2);
        AssignPassiveIcon(icon_Passive3[0], skillSet.thirdPassive, 0);
        AssignPassiveIcon(icon_Passive3[1], skillSet.thirdPassive, 1);
        AssignPassiveIcon(icon_Passive3[2], skillSet.thirdPassive, 2);

        AssignIcon(icon_Basic[0], skillSet.basicAbilities, 0);
        AssignIcon(icon_Basic[1], skillSet.basicAbilities, 1);
        AssignIcon(icon_Basic[2], skillSet.basicAbilities, 2);
        AssignIcon(icon_Offensive[0], skillSet.offensiveAbilities, 0);
        AssignIcon(icon_Offensive[1], skillSet.offensiveAbilities, 1);
        AssignIcon(icon_Offensive[2], skillSet.offensiveAbilities, 2);
        AssignIcon(icon_Mobility[0], skillSet.mobilityAbilities, 0);
        AssignIcon(icon_Mobility[1], skillSet.mobilityAbilities, 1);
        AssignIcon(icon_Mobility[2], skillSet.mobilityAbilities, 2);
        AssignIcon(icon_Defensive[0], skillSet.defensiveAbilities, 0);
        AssignIcon(icon_Defensive[1], skillSet.defensiveAbilities, 1);
        AssignIcon(icon_Defensive[2], skillSet.defensiveAbilities, 2);
        AssignIcon(icon_Utility[0], skillSet.utilityAbilities, 0);
        AssignIcon(icon_Utility[1], skillSet.utilityAbilities, 1);
        AssignIcon(icon_Utility[2], skillSet.utilityAbilities, 2);
        AssignIcon(icon_Ultimate[0], skillSet.ultimateAbilities, 0);
        AssignIcon(icon_Ultimate[1], skillSet.ultimateAbilities, 1);
        AssignIcon(icon_Ultimate[2], skillSet.ultimateAbilities, 2);
    }

    public void SetYellowBorders()
    {
        YellowBorder(player.FirstPassiveIndex, skillSet.passive1Req, icon_Passive1[0]);
        YellowBorder(player.FirstPassiveIndex, skillSet.passive1Req, icon_Passive1[1]);
        YellowBorder(player.FirstPassiveIndex, skillSet.passive1Req, icon_Passive1[2]);
        YellowBorder(player.SecondPassiveIndex, skillSet.passive2Req, icon_Passive2[0]);
        YellowBorder(player.SecondPassiveIndex, skillSet.passive2Req, icon_Passive2[1]);
        YellowBorder(player.SecondPassiveIndex, skillSet.passive2Req, icon_Passive2[2]);
        YellowBorder(player.ThirdPassiveIndex, skillSet.passive3Req, icon_Passive3[0]);
        YellowBorder(player.ThirdPassiveIndex, skillSet.passive3Req, icon_Passive3[1]);
        YellowBorder(player.ThirdPassiveIndex, skillSet.passive3Req, icon_Passive3[2]);

        YellowBorder(player.BasicIndex, skillSet.basicReq, icon_Basic[0]);
        YellowBorder(player.BasicIndex, skillSet.basicReq, icon_Basic[1]);
        YellowBorder(player.BasicIndex, skillSet.basicReq, icon_Basic[2]);
        YellowBorder(player.OffensiveIndex, skillSet.offensiveReq, icon_Offensive[0]);
        YellowBorder(player.OffensiveIndex, skillSet.offensiveReq, icon_Offensive[1]);
        YellowBorder(player.OffensiveIndex, skillSet.offensiveReq, icon_Offensive[2]);
        YellowBorder(player.MobilityIndex, skillSet.mobilityReq, icon_Mobility[0]);
        YellowBorder(player.MobilityIndex, skillSet.mobilityReq, icon_Mobility[1]);
        YellowBorder(player.MobilityIndex, skillSet.mobilityReq, icon_Mobility[2]);
        YellowBorder(player.DefensiveIndex, skillSet.defensiveReq, icon_Defensive[0]);
        YellowBorder(player.DefensiveIndex, skillSet.defensiveReq, icon_Defensive[1]);
        YellowBorder(player.DefensiveIndex, skillSet.defensiveReq, icon_Defensive[2]);
        YellowBorder(player.UtilityIndex, skillSet.utilityReq, icon_Utility[0]);
        YellowBorder(player.UtilityIndex, skillSet.utilityReq, icon_Utility[1]);
        YellowBorder(player.UtilityIndex, skillSet.utilityReq, icon_Utility[2]);
        YellowBorder(player.UltimateIndex, skillSet.ultimateReq, icon_Ultimate[0]);
        YellowBorder(player.UltimateIndex, skillSet.ultimateReq, icon_Ultimate[1]);
        YellowBorder(player.UltimateIndex, skillSet.ultimateReq, icon_Ultimate[2]);
    }

    public void SetBlueBorders()
    {
        BlueBorder(player.FirstPassiveIndex, icon_Passive1[0], icon_Passive1[1], icon_Passive1[2]);
        BlueBorder(player.SecondPassiveIndex, icon_Passive2[0], icon_Passive2[1], icon_Passive2[2]);
        BlueBorder(player.ThirdPassiveIndex, icon_Passive3[0], icon_Passive3[1], icon_Passive3[2]);

        BlueBorder(player.BasicIndex, icon_Basic[0], icon_Basic[1], icon_Basic[2]);
        BlueBorder(player.OffensiveIndex, icon_Offensive[0], icon_Offensive[1], icon_Offensive[2]);
        BlueBorder(player.MobilityIndex, icon_Mobility[0], icon_Mobility[1], icon_Mobility[2]);
        BlueBorder(player.DefensiveIndex, icon_Defensive[0], icon_Defensive[1], icon_Defensive[2]);
        BlueBorder(player.UtilityIndex, icon_Utility[0], icon_Utility[1], icon_Utility[2]);
        BlueBorder(player.UltimateIndex, icon_Ultimate[0], icon_Ultimate[1], icon_Ultimate[2]);
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
        BlueBorder(index, icon_Passive1[0], icon_Passive1[1], icon_Passive1[2]);
        stateMachine.SetFirstPassive(skillSet.firstPassive[index], index);
    }

    public void SecondPassiveButton(int index)
    {
        player.SecondPassiveIndex = index;
        OnSkillSelected?.Invoke();
        BlueBorder(index, icon_Passive2[0], icon_Passive2[1], icon_Passive2[2]);
        stateMachine.SetFirstPassive(skillSet.secondPassive[index], index);
    }

    public void ThirdPassiveButton(int index)
    {
        player.ThirdPassiveIndex = index;
        OnSkillSelected?.Invoke();
        BlueBorder(index, icon_Passive3[0], icon_Passive3[1], icon_Passive3[2]);
        stateMachine.SetFirstPassive(skillSet.thirdPassive[index], index);
    }

    public void BasicButton(int index)
    {
        player.BasicIndex = index;
        OnSkillSelected?.Invoke();
        BlueBorder(index, icon_Basic[0], icon_Basic[1], icon_Basic[2]);
    }

    public void OffensiveButton(int index)
    {
        player.OffensiveIndex = index;
        OnSkillSelected?.Invoke();
        BlueBorder(index, icon_Offensive[0], icon_Offensive[1], icon_Offensive[2]);
    }

    public void MobilityButton(int index)
    {
        player.MobilityIndex = index;
        OnSkillSelected?.Invoke();
        BlueBorder(index, icon_Mobility[0], icon_Mobility[1], icon_Mobility[2]);
    }

    public void DefensiveButton(int index)
    {
        player.DefensiveIndex = index;
        OnSkillSelected?.Invoke();
        BlueBorder(index, icon_Defensive[0], icon_Defensive[1], icon_Defensive[2]);
    }

    public void UtilityButton(int index)
    {
        player.UtilityIndex = index;
        OnSkillSelected?.Invoke();
        BlueBorder(index, icon_Utility[0], icon_Utility[1], icon_Utility[2]);
    }

    public void UltimateButton(int index)
    {
        player.UltimateIndex = index;
        OnSkillSelected?.Invoke();
        BlueBorder(index, icon_Ultimate[0], icon_Ultimate[1], icon_Ultimate[2]);
    }

    void OnLevelChanged(int oldValue, int newValue)
    {
        OnLevelUp();
    }

    void OnLevelUp()
    {
        if (stats.PlayerLevel.Value >= skillSet.basicReq)
        {
            for (int i = 0; i < icon_Basic_Lock.Length; i++)
            {
                icon_Basic_Lock[i].gameObject.SetActive(false);
            }
        }

        if (stats.PlayerLevel.Value >= skillSet.offensiveReq)
        {
            for (int i = 0; i < icon_Offensive_Lock.Length; i++)
            {
                icon_Offensive_Lock[i].gameObject.SetActive(false);
            }
        }

        if (stats.PlayerLevel.Value >= skillSet.mobilityReq)
        {
            for (int i = 0; i < icon_Mobility_Lock.Length; i++)
            {
                icon_Mobility_Lock[i].gameObject.SetActive(false);
            }
        }

        if (stats.PlayerLevel.Value >= skillSet.defensiveReq)
        {
            for (int i = 0; i < icon_Defensive_Lock.Length; i++)
            {
                icon_Defensive_Lock[i].gameObject.SetActive(false);
            }
        }

        if (stats.PlayerLevel.Value >= skillSet.utilityReq)
        {
            for (int i = 0; i < icon_Utility_Lock.Length; i++)
            {
                icon_Utility_Lock[i].gameObject.SetActive(false);
            }
        }

        if (stats.PlayerLevel.Value >= skillSet.ultimateReq)
        {
            for (int i = 0; i < icon_Ultimate_Lock.Length; i++)
            {
                icon_Ultimate_Lock[i].gameObject.SetActive(false);
            }
        }
    }
}
