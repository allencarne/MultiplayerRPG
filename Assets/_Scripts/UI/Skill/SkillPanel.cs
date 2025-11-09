using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SkillPanel : MonoBehaviour
{
    [SerializeField] Player player;

    [Header("Skills")]
    public PlayerSkill[] firstPassive;
    public PlayerSkill[] secondPassive;
    public PlayerSkill[] thirdPassive;
    public PlayerSkill[] basicAbilities;
    public PlayerSkill[] offensiveAbilities;
    public PlayerSkill[] mobilityAbilities;
    public PlayerSkill[] defensiveAbilities;
    public PlayerSkill[] utilityAbilities;
    public PlayerSkill[] ultimateAbilities;

    int passive1Req = 1;
    int basicReq = 1;
    int offensiveReq = 4;
    int passive2Req = 6;
    int mobilityReq = 8;
    int defensiveReq = 12;
    int passive3Req = 14;
    int utilityReq = 16;
    int ultimateReq = 20;

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

    public UnityEvent OnSkillSelected;

    private void Start()
    {
        SetIcons();
    }

    private void OnEnable()
    {
        SetYellowBorders();
        SetBlueBorders();
    }

    private void AssignIcon(Image icon, PlayerSkill[] abilities, int index)
    {
        if (icon == null || abilities == null || index >= abilities.Length || abilities[index] == null) return;

        if (abilities[index].SkillIcon != null) icon.sprite = abilities[index].SkillIcon;

        SkillPanelToolTip tooltip = icon.GetComponentInParent<SkillPanelToolTip>();
        if (tooltip != null) tooltip.SetAbility(abilities[index]);
    }

    void YellowBorder(int index, int reqLevel, Image icon)
    {
        if (icon == null) return;
        if (index > -1) return;
        if (player.PlayerLevel.Value < reqLevel) return;

        SetColor(icon, Color.cyan);
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

    void SetIcons()
    {
        AssignIcon(icon_FirstPassive0, firstPassive, 0);
        AssignIcon(icon_FirstPassive1, firstPassive, 1);
        AssignIcon(icon_FirstPassive2, firstPassive, 2);
        AssignIcon(icon_SecondPassive0, secondPassive, 0);
        AssignIcon(icon_SecondPassive1, secondPassive, 1);
        AssignIcon(icon_SecondPassive2, secondPassive, 2);
        AssignIcon(icon_ThirdPassive0, thirdPassive, 0);
        AssignIcon(icon_ThirdPassive1, thirdPassive, 1);
        AssignIcon(icon_ThirdPassive2, thirdPassive, 2);

        AssignIcon(icon_Basic0, basicAbilities, 0);
        AssignIcon(icon_Basic1, basicAbilities, 1);
        AssignIcon(icon_Basic2, basicAbilities, 2);
        AssignIcon(icon_Offensive0, offensiveAbilities, 0);
        AssignIcon(icon_Offensive1, offensiveAbilities, 1);
        AssignIcon(icon_Offensive2, offensiveAbilities, 2);
        AssignIcon(icon_Mobility0, mobilityAbilities, 0);
        AssignIcon(icon_Mobility1, mobilityAbilities, 1);
        AssignIcon(icon_Mobility2, mobilityAbilities, 2);
        AssignIcon(icon_Defensive0, defensiveAbilities, 0);
        AssignIcon(icon_Defensive1, defensiveAbilities, 1);
        AssignIcon(icon_Defensive2, defensiveAbilities, 2);
        AssignIcon(icon_Utility0, utilityAbilities, 0);
        AssignIcon(icon_Utility1, utilityAbilities, 1);
        AssignIcon(icon_Utility2, utilityAbilities, 2);
        AssignIcon(icon_Ultimate0, ultimateAbilities, 0);
        AssignIcon(icon_Ultimate1, ultimateAbilities, 1);
        AssignIcon(icon_Ultimate2, ultimateAbilities, 2);
    }

    public void SetYellowBorders()
    {
        YellowBorder(player.FirstPassiveIndex, passive1Req, icon_FirstPassive0);
        YellowBorder(player.FirstPassiveIndex, passive1Req, icon_FirstPassive1);
        YellowBorder(player.FirstPassiveIndex, passive1Req, icon_FirstPassive2);
        YellowBorder(player.SecondPassiveIndex, passive2Req, icon_SecondPassive0);
        YellowBorder(player.SecondPassiveIndex, passive2Req, icon_SecondPassive1);
        YellowBorder(player.SecondPassiveIndex, passive2Req, icon_SecondPassive2);
        YellowBorder(player.ThirdPassiveIndex, passive3Req, icon_ThirdPassive0);
        YellowBorder(player.ThirdPassiveIndex, passive3Req, icon_ThirdPassive1);
        YellowBorder(player.ThirdPassiveIndex, passive3Req, icon_ThirdPassive2);

        YellowBorder(player.BasicIndex, basicReq, icon_Basic0);
        YellowBorder(player.BasicIndex, basicReq, icon_Basic1);
        YellowBorder(player.BasicIndex, basicReq, icon_Basic2);
        YellowBorder(player.OffensiveIndex, offensiveReq, icon_Offensive0);
        YellowBorder(player.OffensiveIndex, offensiveReq, icon_Offensive1);
        YellowBorder(player.OffensiveIndex, offensiveReq, icon_Offensive2);
        YellowBorder(player.MobilityIndex, mobilityReq, icon_Mobility0);
        YellowBorder(player.MobilityIndex, mobilityReq, icon_Mobility1);
        YellowBorder(player.MobilityIndex, mobilityReq, icon_Mobility2);
        YellowBorder(player.DefensiveIndex, defensiveReq, icon_Defensive0);
        YellowBorder(player.DefensiveIndex, defensiveReq, icon_Defensive1);
        YellowBorder(player.DefensiveIndex, defensiveReq, icon_Defensive2);
        YellowBorder(player.UtilityIndex, utilityReq, icon_Utility0);
        YellowBorder(player.UtilityIndex, utilityReq, icon_Utility1);
        YellowBorder(player.UtilityIndex, utilityReq, icon_Utility2);
        YellowBorder(player.UltimateIndex, ultimateReq, icon_Ultimate0);
        YellowBorder(player.UltimateIndex, ultimateReq, icon_Ultimate1);
        YellowBorder(player.UltimateIndex, ultimateReq, icon_Ultimate2);
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

    public void FirstPassiveButton(int index)
    {
        if (player.PlayerLevel.Value < passive1Req) return;
        player.FirstPassiveIndex = index;
        OnSkillSelected?.Invoke();

        BlueBorder(index, icon_FirstPassive0, icon_FirstPassive1, icon_FirstPassive2);

        PlayerStateMachine stateMachine = player.GetComponent<PlayerStateMachine>();
        firstPassive[index].StartSkill(stateMachine);
    }

    public void SecondPassiveButton(int index)
    {
        if (player.PlayerLevel.Value < passive2Req) return;
        player.SecondPassiveIndex = index;
        OnSkillSelected?.Invoke();

        BlueBorder(index, icon_SecondPassive0, icon_SecondPassive1, icon_SecondPassive2);

        PlayerStateMachine stateMachine = player.GetComponent<PlayerStateMachine>();
        secondPassive[index].StartSkill(stateMachine);
    }

    public void ThirdPassiveButton(int index)
    {
        if (player.PlayerLevel.Value < passive3Req) return;
        player.ThirdPassiveIndex = index;
        OnSkillSelected?.Invoke();

        BlueBorder(index, icon_ThirdPassive0, icon_ThirdPassive1, icon_ThirdPassive2);

        PlayerStateMachine stateMachine = player.GetComponent<PlayerStateMachine>();
        thirdPassive[index].StartSkill(stateMachine);
    }

    public void BasicButton(int index)
    {
        if (player.PlayerLevel.Value < basicReq) return;
        player.BasicIndex = index;
        OnSkillSelected?.Invoke();

        BlueBorder(index, icon_Basic0, icon_Basic1, icon_Basic2);
    }

    public void OffensiveButton(int index)
    {
        if (player.PlayerLevel.Value < offensiveReq) return;
        player.OffensiveIndex = index;
        OnSkillSelected?.Invoke();

        BlueBorder(index, icon_Offensive0, icon_Offensive1, icon_Offensive2);
    }

    public void MobilityButton(int index)
    {
        if (player.PlayerLevel.Value < mobilityReq) return;
        player.MobilityIndex = index;
        OnSkillSelected?.Invoke();

        BlueBorder(index, icon_Mobility0, icon_Mobility1, icon_Mobility2);
    }

    public void DefensiveButton(int index)
    {
        if (player.PlayerLevel.Value < defensiveReq) return;
        player.DefensiveIndex = index;
        OnSkillSelected?.Invoke();

        BlueBorder(index, icon_Defensive0, icon_Defensive1, icon_Defensive2);
    }

    public void UtilityButton(int index)
    {
        if (player.PlayerLevel.Value < utilityReq) return;
        player.UtilityIndex = index;
        OnSkillSelected?.Invoke();

        BlueBorder(index, icon_Utility0, icon_Utility1, icon_Utility2);
    }

    public void UltimateButton(int index)
    {
        if (player.PlayerLevel.Value < ultimateReq) return;
        player.UltimateIndex = index;
        OnSkillSelected?.Invoke();

        BlueBorder(index, icon_Ultimate0, icon_Ultimate1, icon_Ultimate2);
    }
}
