using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillBarUI : MonoBehaviour
{
    [SerializeField] PlayerStats stats;
    [SerializeField] Player player;

    [Header("Basic")]
    [SerializeField] Image icon_Basic;
    [SerializeField] Image icon_Basic_Lock;
    [SerializeField] Image icon_Basic_Tint;
    [SerializeField] TextMeshProUGUI text_Basic;

    [Header("Offensive")]
    [SerializeField] Image icon_Offensive;
    [SerializeField] Image icon_Offensive_Lock;
    [SerializeField] Image icon_Offensive_Tint;
    [SerializeField] TextMeshProUGUI text_Offensive;

    [Header("Mobility")]
    [SerializeField] Image icon_Mobility;
    [SerializeField] Image icon_Mobility_Lock;
    [SerializeField] Image icon_Mobility_Tint;
    [SerializeField] TextMeshProUGUI text_Mobility;

    [Header("Defensive")]
    [SerializeField] Image icon_Defensive;
    [SerializeField] Image icon_Defensive_Lock;
    [SerializeField] Image icon_Defensive_Tint;
    [SerializeField] TextMeshProUGUI text_Defensive;

    [Header("Utility")]
    [SerializeField] Image icon_Utility;
    [SerializeField] Image icon_Utility_Lock;
    [SerializeField] Image icon_Utility_Tint;
    [SerializeField] TextMeshProUGUI text_Utility;

    [Header("Ultimate")]
    [SerializeField] Image icon_Ultimate;
    [SerializeField] Image icon_Ultimate_Lock;
    [SerializeField] Image icon_Ultimate_Tint;
    [SerializeField] TextMeshProUGUI text_Ultimate;

    class SkillBarSlot
    {
        public Image Icon;
        public Image Lock;
        public Image Tint;
        public TextMeshProUGUI Text;
        public ActiveSkillData[] Data;
        public Func<int> GetIndex;
        public int ReqLevel;
        public Coroutine CooldownRoutine;
    }

    Dictionary<ActiveSkillData.SkillType, SkillBarSlot> slots;

    private void OnEnable()
    {
        stats.PlayerLevel.OnValueChanged += OnLevelChanged;
    }

    private void OnDisable()
    {
        stats.PlayerLevel.OnValueChanged -= OnLevelChanged;
    }

    public void Bind(ClassSkillSet set)
    {
        slots = new Dictionary<ActiveSkillData.SkillType, SkillBarSlot>
        {
            [ActiveSkillData.SkillType.Basic] = new SkillBarSlot { Icon = icon_Basic, Lock = icon_Basic_Lock, Tint = icon_Basic_Tint, Text = text_Basic, Data = set.basicAbilities, GetIndex = () => player.BasicIndex, ReqLevel = set.basicReq },
            [ActiveSkillData.SkillType.Offensive] = new SkillBarSlot { Icon = icon_Offensive, Lock = icon_Offensive_Lock, Tint = icon_Offensive_Tint, Text = text_Offensive, Data = set.offensiveAbilities, GetIndex = () => player.OffensiveIndex, ReqLevel = set.offensiveReq },
            [ActiveSkillData.SkillType.Mobility] = new SkillBarSlot { Icon = icon_Mobility, Lock = icon_Mobility_Lock, Tint = icon_Mobility_Tint, Text = text_Mobility, Data = set.mobilityAbilities, GetIndex = () => player.MobilityIndex, ReqLevel = set.mobilityReq },
            [ActiveSkillData.SkillType.Defensive] = new SkillBarSlot { Icon = icon_Defensive, Lock = icon_Defensive_Lock, Tint = icon_Defensive_Tint, Text = text_Defensive, Data = set.defensiveAbilities, GetIndex = () => player.DefensiveIndex, ReqLevel = set.defensiveReq },
            [ActiveSkillData.SkillType.Utility] = new SkillBarSlot { Icon = icon_Utility, Lock = icon_Utility_Lock, Tint = icon_Utility_Tint, Text = text_Utility, Data = set.utilityAbilities, GetIndex = () => player.UtilityIndex, ReqLevel = set.utilityReq },
            [ActiveSkillData.SkillType.Ultimate] = new SkillBarSlot { Icon = icon_Ultimate, Lock = icon_Ultimate_Lock, Tint = icon_Ultimate_Tint, Text = text_Ultimate, Data = set.ultimateAbilities, GetIndex = () => player.UltimateIndex, ReqLevel = set.ultimateReq },
        };

        RefreshIcons();
        RefreshLocks();
    }

    public void RefreshIcons()
    {
        if (slots == null) return;

        foreach (SkillBarSlot slot in slots.Values)
        {
            int index = slot.GetIndex();
            if (index < 0 || index >= slot.Data.Length || slot.Data[index] == null) continue;
            if (slot.Data[index].Icon != null)
            {
                slot.Icon.sprite = slot.Data[index].Icon;
                slot.Icon.color = Color.white;
            }
        }
    }

    void RefreshLocks()
    {
        foreach (SkillBarSlot slot in slots.Values)
        {
            if (slot.Lock == null) continue;
            slot.Lock.gameObject.SetActive(stats.PlayerLevel.Value < slot.ReqLevel);
        }
    }

    public void SkillCoolDown(ActiveSkillData.SkillType type, float coolDown)
    {
        if (slots == null || !slots.TryGetValue(type, out SkillBarSlot slot)) return;

        if (slot.CooldownRoutine != null) StopCoroutine(slot.CooldownRoutine);
        slot.CooldownRoutine = StartCoroutine(TrackCooldown(slot, coolDown));
    }

    IEnumerator TrackCooldown(SkillBarSlot slot, float coolDown)
    {
        if (slot.Tint != null) slot.Tint.enabled = true;

        float timeRemaining = coolDown;
        while (timeRemaining > 0f)
        {
            if (slot.Text != null) slot.Text.text = timeRemaining.ToString("F1");
            yield return null;
            timeRemaining -= Time.deltaTime;
        }

        if (slot.Text != null) slot.Text.text = "";
        if (slot.Tint != null) slot.Tint.enabled = false;
        slot.CooldownRoutine = null;
    }

    void OnLevelChanged(int oldValue, int newValue) => RefreshLocks();
}
