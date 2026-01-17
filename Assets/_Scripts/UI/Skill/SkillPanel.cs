using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SkillPanel : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] PlayerStats stats;
    [SerializeField] PlayerStateMachine stateMachine;

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

    public int passive1Req = 0;
    public int basicReq = 0;
    public int offensiveReq = 4;
    public int passive2Req = 6;
    public int mobilityReq = 8;
    public int defensiveReq = 12;
    public int passive3Req = 14;
    public int utilityReq = 16;
    public int ultimateReq = 20;

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
        InvokeRepeating("SetYellowBorders", 0, 1);
        InvokeRepeating("SetBlueBorders", 0, 1);
    }

    private void OnDisable()
    {
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

    private void AssignIcon(Image icon, PlayerSkill[] abilities, int index)
    {
        if (icon == null || abilities == null || index >= abilities.Length || abilities[index] == null) return;

        if (abilities[index].SkillIcon != null) icon.sprite = abilities[index].SkillIcon;

        SkillPanelToolTip tooltip = icon.GetComponentInParent<SkillPanelToolTip>();
        if (tooltip != null) tooltip.SetAbility(abilities[index]);
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
        firstPassive[index].StartSkill(stateMachine);
    }

    public void SecondPassiveButton(int index)
    {
        player.SecondPassiveIndex = index;
        OnSkillSelected?.Invoke();

        BlueBorder(index, icon_SecondPassive0, icon_SecondPassive1, icon_SecondPassive2);

        secondPassive[index].StartSkill(stateMachine);
    }

    public void ThirdPassiveButton(int index)
    {
        player.ThirdPassiveIndex = index;
        OnSkillSelected?.Invoke();

        BlueBorder(index, icon_ThirdPassive0, icon_ThirdPassive1, icon_ThirdPassive2);

        thirdPassive[index].StartSkill(stateMachine);
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
}
