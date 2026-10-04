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

    [Header("Potion")]
    [SerializeField] Image img_Potion_Icon;
    [SerializeField] Image img_Potion_Background;
    [SerializeField] Image img_Potion_Quality;
    [SerializeField] Image img_Potion_Tint;
    public TextMeshProUGUI txt_Potion_Amount;
    public TextMeshProUGUI txt_Potion_Cooldown;

    [Header("Food")]
    [SerializeField] Image img_Food_Icon;
    [SerializeField] Image img_Food_Background;
    [SerializeField] Image img_Food_Quality;
    [SerializeField] Image img_Food_Tint;
    public TextMeshProUGUI txt_Food_Amount;
    public TextMeshProUGUI txt_Food_Cooldown;

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

    [Header("Tint Colors")]
    Color cooldownTint = new Color(0.0f, 0.0f, 0.0f, 0.55f);
    Color manaTint = new Color(0.15f, 0.45f, 1f, 0.55f);

    [Header("Consumables")]
    [SerializeField] ConsumableSlots consumableSlots;

    class ConsumableBarSlot
    {
        public Image Icon;
        public Image Background;
        public Image Quality;
        public Image Tint;
        public TextMeshProUGUI Amount;
        public TextMeshProUGUI Cooldown;
        public Coroutine Routine;
    }

    Dictionary<ConsumableSlotType, ConsumableBarSlot> consumableBar;

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
        stats.net_CurrentMana.OnValueChanged += OnManaChanged;

        consumableSlots.OnSlotsChanged.AddListener(RefreshConsumables);
        consumableSlots.OnConsumed += OnConsumableUsed;

        RefreshConsumables();

        // If a cooldown is still running from before this was enabled, pick it back up
        foreach (ConsumableSlotType type in consumableBar.Keys)
        {
            if (consumableSlots.CooldownRemaining(type) > 0f) StartConsumableCooldown(type);
        }
    }

    private void OnDisable()
    {
        stats.PlayerLevel.OnValueChanged -= OnLevelChanged;
        stats.net_CurrentMana.OnValueChanged -= OnManaChanged;

        consumableSlots.OnSlotsChanged.RemoveListener(RefreshConsumables);
        consumableSlots.OnConsumed -= OnConsumableUsed;

        // Unity stops coroutines on disable, so clear the stale handles
        foreach (ConsumableBarSlot bar in consumableBar.Values) bar.Routine = null;
    }

    private void Awake()
    {
        consumableBar = new Dictionary<ConsumableSlotType, ConsumableBarSlot>
        {
            [ConsumableSlotType.Potion] = new ConsumableBarSlot { Icon = img_Potion_Icon, Background = img_Potion_Background, Quality = img_Potion_Quality, Tint = img_Potion_Tint, Amount = txt_Potion_Amount, Cooldown = txt_Potion_Cooldown },
            [ConsumableSlotType.Food] = new ConsumableBarSlot { Icon = img_Food_Icon, Background = img_Food_Background, Quality = img_Food_Quality, Tint = img_Food_Tint, Amount = txt_Food_Amount, Cooldown = txt_Food_Cooldown },
        };
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
        UpdateTintForSlot(slot);
    }

    IEnumerator TrackCooldown(SkillBarSlot slot, float coolDown)
    {
        if (slot.Tint != null)
        {
            slot.Tint.enabled = true;
            slot.Tint.color = cooldownTint;
        }

        float timeRemaining = coolDown;
        while (timeRemaining > 0f)
        {
            if (slot.Text != null) slot.Text.text = timeRemaining.ToString("F1");
            yield return null;
            timeRemaining -= Time.deltaTime;
        }

        if (slot.Text != null) slot.Text.text = "";
        slot.CooldownRoutine = null;

        // cooldown finished — re-evaluate tint (may become mana tint if mana still insufficient)
        UpdateTintForSlot(slot);
    }

    void RefreshTints()
    {
        if (slots == null) return;
        foreach (SkillBarSlot slot in slots.Values)
        {
            UpdateTintForSlot(slot);
        }
    }

    void UpdateTintForSlot(SkillBarSlot slot)
    {
        if (slot == null || slot.Tint == null) return;

        int index = slot.GetIndex();
        bool hasValidSkill = !(index < 0 || index >= slot.Data.Length || slot.Data[index] == null);

        // Skill doesn't exist or is locked by level
        if (!hasValidSkill || stats.PlayerLevel.Value < slot.ReqLevel)
        {
            slot.Tint.enabled = false;

            if (slot.Text != null)
                slot.Text.text = "";

            return;
        }

        // Check mana
        float manaCost = slot.Data[index].ManaCost;
        bool lacksMana = manaCost > 0f && stats.net_CurrentMana.Value < manaCost;

        // Mana takes priority over cooldown color
        if (lacksMana)
        {
            slot.Tint.enabled = true;
            slot.Tint.color = manaTint;
            return;
        }

        // Has enough mana, so show cooldown if active
        if (slot.CooldownRoutine != null)
        {
            slot.Tint.enabled = true;
            slot.Tint.color = cooldownTint;
            return;
        }

        // Skill is available
        slot.Tint.enabled = false;

        if (slot.Text != null)
            slot.Text.text = "";
    }

    void OnLevelChanged(int oldValue, int newValue) => RefreshLocks();
    void OnManaChanged(float oldValue, float newValue) => RefreshTints();

    void OnConsumableUsed(ConsumableSlotType type, float cooldown) => StartConsumableCooldown(type);

    void RefreshConsumables()
    {
        foreach (KeyValuePair<ConsumableSlotType, ConsumableBarSlot> pair in consumableBar)
        {
            ConsumableBarSlot bar = pair.Value;
            InventorySlotData data = consumableSlots.GetSlot(pair.Key);
            bool has = data != null && data.item != null;

            bar.Icon.enabled = has;
            bar.Background.enabled = has;
            bar.Quality.enabled = has;

            if (has)
            {
                bar.Icon.sprite = data.item.Icon;
                bar.Background.color = data.item.GetRarityColor(data.rarity);
                bar.Quality.color = data.item.GetQualityColor(data.quality);
                bar.Amount.text = data.quantity > 1 ? data.quantity.ToString() : "";
            }
            else
            {
                bar.Icon.sprite = null;
                bar.Amount.text = "";
            }

            UpdateConsumableTint(pair.Key, bar);
        }
    }

    void StartConsumableCooldown(ConsumableSlotType type)
    {
        ConsumableBarSlot bar = consumableBar[type];

        if (bar.Routine != null) StopCoroutine(bar.Routine);
        bar.Routine = StartCoroutine(TrackConsumableCooldown(type, bar));
    }

    IEnumerator TrackConsumableCooldown(ConsumableSlotType type, ConsumableBarSlot bar)
    {
        UpdateConsumableTint(type, bar);

        // Read the remaining time from ConsumableSlots so the two can never drift apart
        float remaining = consumableSlots.CooldownRemaining(type);
        while (remaining > 0f)
        {
            bar.Cooldown.text = FormatCooldown(remaining);
            yield return null;
            remaining = consumableSlots.CooldownRemaining(type);
        }

        bar.Routine = null;
        UpdateConsumableTint(type, bar);
    }

    void UpdateConsumableTint(ConsumableSlotType type, ConsumableBarSlot bar)
    {
        bool onCooldown = bar.Routine != null;

        bar.Tint.enabled = onCooldown;
        if (onCooldown) bar.Tint.color = cooldownTint;
        else bar.Cooldown.text = "";
    }

    string FormatCooldown(float seconds)
    {
        int total = Mathf.CeilToInt(seconds);
        return total >= 60 ? $"{total / 60}:{total % 60:00}" : total.ToString();
    }
}
