using System;
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

    [Header("Passive 1")]
    [SerializeField] GameObject[] object_Passive1;
    [SerializeField] Image[] icon_Passive1;
    [SerializeField] Image[] icon_Passive1_Lock;
    [SerializeField] TextMeshProUGUI[] text_Passive1;

    [Header("Passive 2")]
    [SerializeField] GameObject[] object_Passive2;
    [SerializeField] Image[] icon_Passive2;
    [SerializeField] Image[] icon_Passive2_Lock;
    [SerializeField] TextMeshProUGUI[] text_Passive2;

    [Header("Passive 3")]
    [SerializeField] GameObject[] object_Passive3;
    [SerializeField] Image[] icon_Passive3;
    [SerializeField] Image[] icon_Passive3_Lock;
    [SerializeField] TextMeshProUGUI[] text_Passive3;

    [Header("Basic")]
    [SerializeField] GameObject[] object_Basic;
    [SerializeField] Image[] icon_Basic;
    [SerializeField] Image[] icon_Basic_Lock;
    [SerializeField] TextMeshProUGUI[] text_Basic;

    [Header("Offensive")]
    [SerializeField] GameObject[] object_Offensive;
    [SerializeField] Image[] icon_Offensive;
    [SerializeField] Image[] icon_Offensive_Lock;
    [SerializeField] TextMeshProUGUI[] text_Offensive;

    [Header("Mobility")]
    [SerializeField] GameObject[] object_Mobility;
    [SerializeField] Image[] icon_Mobility;
    [SerializeField] Image[] icon_Mobility_Lock;
    [SerializeField] TextMeshProUGUI[] text_Mobility;

    [Header("Defensive")]
    [SerializeField] GameObject[] object_Defensive;
    [SerializeField] Image[] icon_Defensive;
    [SerializeField] Image[] icon_Defensive_Lock;
    [SerializeField] TextMeshProUGUI[] text_Defensive;

    [Header("Utility")]
    [SerializeField] GameObject[] object_Utility;
    [SerializeField] Image[] icon_Utility;
    [SerializeField] Image[] icon_Utility_Lock;
    [SerializeField] TextMeshProUGUI[] text_Utility;

    [Header("Ultimate")]
    [SerializeField] GameObject[] object_Ultimate;
    [SerializeField] Image[] icon_Ultimate;
    [SerializeField] Image[] icon_Ultimate_Lock;
    [SerializeField] TextMeshProUGUI[] text_Ultimate;

    [HideInInspector] public UnityEvent OnSkillSelected;

    struct SkillSlot
    {
        public GameObject[] Objects;
        public Image[] Icons;
        public Image[] Locks;
        public TextMeshProUGUI[] Texts;
        public SkillData[] Data;
        public Func<int> GetIndex;
        public int ReqLevel;
    }

    SkillSlot[] slots;

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

    public void Bind(ClassSkillSet set)
    {
        skillSet = set;
        BuildSlots();
        RefreshSlotVisibility();
        SetIcons();
        RefreshRequirementText();
        RefreshLocks();
    }

    void BuildSlots()
    {
        slots = new[]
        {
            new SkillSlot { Objects = object_Passive1, Icons = icon_Passive1, Locks = icon_Passive1_Lock, Texts = text_Passive1, Data = skillSet.firstPassive,  GetIndex = () => player.FirstPassiveIndex,  ReqLevel = skillSet.passive1Req },
            new SkillSlot { Objects = object_Passive2, Icons = icon_Passive2, Locks = icon_Passive2_Lock, Texts = text_Passive2, Data = skillSet.secondPassive, GetIndex = () => player.SecondPassiveIndex, ReqLevel = skillSet.passive2Req },
            new SkillSlot { Objects = object_Passive3, Icons = icon_Passive3, Locks = icon_Passive3_Lock, Texts = text_Passive3, Data = skillSet.thirdPassive,  GetIndex = () => player.ThirdPassiveIndex,  ReqLevel = skillSet.passive3Req },
            new SkillSlot { Objects = object_Basic,     Icons = icon_Basic,     Locks = icon_Basic_Lock,     Texts = text_Basic,     Data = skillSet.basicAbilities,     GetIndex = () => player.BasicIndex,     ReqLevel = skillSet.basicReq },
            new SkillSlot { Objects = object_Offensive, Icons = icon_Offensive, Locks = icon_Offensive_Lock, Texts = text_Offensive, Data = skillSet.offensiveAbilities, GetIndex = () => player.OffensiveIndex, ReqLevel = skillSet.offensiveReq },
            new SkillSlot { Objects = object_Mobility,  Icons = icon_Mobility,  Locks = icon_Mobility_Lock,  Texts = text_Mobility,  Data = skillSet.mobilityAbilities,  GetIndex = () => player.MobilityIndex,  ReqLevel = skillSet.mobilityReq },
            new SkillSlot { Objects = object_Defensive, Icons = icon_Defensive, Locks = icon_Defensive_Lock, Texts = text_Defensive, Data = skillSet.defensiveAbilities, GetIndex = () => player.DefensiveIndex, ReqLevel = skillSet.defensiveReq },
            new SkillSlot { Objects = object_Utility,   Icons = icon_Utility,   Locks = icon_Utility_Lock,   Texts = text_Utility,   Data = skillSet.utilityAbilities,   GetIndex = () => player.UtilityIndex,   ReqLevel = skillSet.utilityReq },
            new SkillSlot { Objects = object_Ultimate,  Icons = icon_Ultimate,  Locks = icon_Ultimate_Lock,  Texts = text_Ultimate,  Data = skillSet.ultimateAbilities,  GetIndex = () => player.UltimateIndex,  ReqLevel = skillSet.ultimateReq },
        };
    }

    void RefreshSlotVisibility()
    {
        foreach (SkillSlot slot in slots)
        {
            for (int i = 0; i < slot.Objects.Length; i++)
            {
                slot.Objects[i].SetActive(i < slot.Data.Length);
            }
        }
    }

    void RefreshRequirementText()
    {
        foreach (SkillSlot slot in slots)
        {
            string label = slot.ReqLevel > 0 ? slot.ReqLevel.ToString() : "";
            foreach (TextMeshProUGUI text in slot.Texts)
            {
                if (text != null) text.text = label;
            }
        }
    }

    void SetIcons()
    {
        foreach (SkillSlot slot in slots)
        {
            for (int i = 0; i < slot.Icons.Length; i++)
            {
                AssignIcon(slot.Icons[i], slot.Data, i);
            }
        }
    }

    void AssignIcon(Image icon, SkillData[] abilities, int index)
    {
        if (icon == null || abilities == null || index >= abilities.Length || abilities[index] == null) return;
        if (abilities[index].Icon != null) icon.sprite = abilities[index].Icon;
    }

    public void SetYellowBorders()
    {
        foreach (SkillSlot slot in slots)
        {
            foreach (Image icon in slot.Icons)
            {
                YellowBorder(slot.GetIndex(), slot.ReqLevel, icon);
            }
        }
    }

    public void SetBlueBorders()
    {
        foreach (SkillSlot slot in slots)
        {
            BlueBorder(slot.GetIndex(), slot.Icons[0], slot.Icons[1], slot.Icons[2]);
        }
    }

    void RefreshLocks()
    {
        foreach (SkillSlot slot in slots)
        {
            if (stats.PlayerLevel.Value < slot.ReqLevel) continue;
            foreach (Image lockIcon in slot.Locks)
            {
                if (lockIcon != null) lockIcon.gameObject.SetActive(false);
            }
        }
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

    void SelectAndHighlight(Image[] icons, int index)
    {
        OnSkillSelected?.Invoke();
        BlueBorder(index, icons[0], icons[1], icons[2]);
    }

    public void FirstPassiveButton(int index) { player.FirstPassiveIndex = index; SelectAndHighlight(icon_Passive1, index); stateMachine.SetFirstPassive(skillSet.firstPassive[index], index); }
    public void SecondPassiveButton(int index) { player.SecondPassiveIndex = index; SelectAndHighlight(icon_Passive2, index); stateMachine.SetSecondPassive(skillSet.secondPassive[index], index); }
    public void ThirdPassiveButton(int index) { player.ThirdPassiveIndex = index; SelectAndHighlight(icon_Passive3, index); stateMachine.SetThirdPassive(skillSet.thirdPassive[index], index); }
    public void BasicButton(int index) { player.BasicIndex = index; SelectAndHighlight(icon_Basic, index); }
    public void OffensiveButton(int index) { player.OffensiveIndex = index; SelectAndHighlight(icon_Offensive, index); }
    public void MobilityButton(int index) { player.MobilityIndex = index; SelectAndHighlight(icon_Mobility, index); }
    public void DefensiveButton(int index) { player.DefensiveIndex = index; SelectAndHighlight(icon_Defensive, index); }
    public void UtilityButton(int index) { player.UtilityIndex = index; SelectAndHighlight(icon_Utility, index); }
    public void UltimateButton(int index) { player.UltimateIndex = index; SelectAndHighlight(icon_Ultimate, index); }

    void OnLevelChanged(int oldValue, int newValue)
    {
        OnLevelUp();
    }

    void OnLevelUp()
    {
        if (stats.PlayerLevel.Value >= skillSet.passive1Req)
        {
            for (int i = 0; i < icon_Passive1_Lock.Length; i++)
            {
                icon_Passive1_Lock[i].gameObject.SetActive(false);
            }
        }

        if (stats.PlayerLevel.Value >= skillSet.passive2Req)
        {
            for (int i = 0; i < icon_Passive2_Lock.Length; i++)
            {
                icon_Passive2_Lock[i].gameObject.SetActive(false);
            }
        }

        if (stats.PlayerLevel.Value >= skillSet.passive3Req)
        {
            for (int i = 0; i < icon_Passive3_Lock.Length; i++)
            {
                icon_Passive3_Lock[i].gameObject.SetActive(false);
            }
        }

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
