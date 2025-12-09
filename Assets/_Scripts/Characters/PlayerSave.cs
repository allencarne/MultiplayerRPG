using System.Collections;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerSave : NetworkBehaviour
{
    [Header("Data")]
    [SerializeField] CharacterCustomizationData customizationData;

    [Header("References")]
    Player player;
    PlayerStats stats;
    Inventory inventory;
    EquipmentManager equipment;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI saveText;

    private void Awake()
    {
        player = GetComponent<Player>();
        stats = GetComponent<PlayerStats>();
        inventory = GetComponentInChildren<Inventory>();
        equipment = GetComponentInChildren<EquipmentManager>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            LoadCustomization();
            LoadPlayerStats();
            LoadCharacterStats();
            LoadPlayerSkills();
            inventory.LoadInventory();
            equipment.LoadEquipment();
        }
    }

    void LoadCustomization()
    {
        int slot = PlayerPrefs.GetInt("SelectedCharacter");

        string name = PlayerPrefs.GetString($"Character{slot}Name", "No Name");
        int skinIndex = PlayerPrefs.GetInt($"Character{slot}SkinColor");
        int hairIndex = PlayerPrefs.GetInt($"Character{slot}HairColor");
        Color skinCol = customizationData.skinColors[skinIndex];
        Color hairCol = customizationData.hairColors[hairIndex];

        if (IsServer)
        {
            ApplyCustomization(slot, name, skinCol, hairCol);
        }
        else
        {
            LoadCustomizationServerRPC(slot, name, skinCol, hairCol);
        }
    }

    void ApplyCustomization(int slot, FixedString32Bytes name, Color skin, Color hair)
    {
        stats.net_CharacterSlot.Value = slot;
        stats.net_playerName.Value = name;
        stats.net_bodyColor.Value = skin;
        stats.net_hairColor.Value = hair;
    }

    [ServerRpc]
    void LoadCustomizationServerRPC(int slot, FixedString32Bytes name, Color skin, Color hair)
    {
        ApplyCustomization(slot, name, skin, hair);
    }

    void LoadPlayerStats()
    {
        int slot = stats.net_CharacterSlot.Value;

        int level = PlayerPrefs.GetInt($"{slot}PlayerLevel", 1);
        float currentExp = PlayerPrefs.GetFloat($"{slot}CurrentExperience", 0);
        float requiredExp = PlayerPrefs.GetFloat($"{slot}RequiredExperience", 10);
        int ap = PlayerPrefs.GetInt($"{slot}AP", 0);
        float coins = PlayerPrefs.GetFloat($"{slot}Coins", 0);

        if (IsServer)
        {
            ApplyPlayerStats(level, currentExp, requiredExp, coins, ap);
        }
        else
        {
            LoadPlayerStatsServerRPC(level, currentExp, requiredExp, coins, ap);
        }
    }

    void ApplyPlayerStats(int level, float cEXP, float rEXP, float coins, int ap)
    {
        stats.PlayerLevel.Value = level;
        stats.CurrentExperience.Value = cEXP;
        stats.RequiredExperience.Value = rEXP;
        stats.AttributePoints.Value = ap;
        stats.Coins = coins;
    }

    [ServerRpc]
    void LoadPlayerStatsServerRPC(int level, float cEXP, float rEXP, float coins, int ap)
    {
        ApplyPlayerStats(level, cEXP,rEXP, coins, ap);
    }

    void LoadCharacterStats()
    {
        int slot = stats.net_CharacterSlot.Value;

        float health = PlayerPrefs.GetFloat($"{slot}MaxHealth", 10);
        float fury = PlayerPrefs.GetFloat($"{slot}MaxFury", 100);
        float end = PlayerPrefs.GetFloat($"{slot}MaxEndurance", 100);

        float speed = PlayerPrefs.GetFloat($"{slot}Speed", 5);
        int damage = PlayerPrefs.GetInt($"{slot}Damage", 1);
        float atspd = PlayerPrefs.GetFloat($"{slot}AttackSpeed", 1);
        float cdr = PlayerPrefs.GetFloat($"{slot}CDR", 1);
        float armor = PlayerPrefs.GetFloat($"{slot}Armor", 0);

        if (IsServer)
        {
            ApplyCharacterStats(health, fury, end, speed, damage, atspd, cdr, armor);
        }
        else
        {
            LoadCharacterStatsServerRPC(health, fury, end, speed, damage, atspd, cdr, armor);
        }
    }

    void ApplyCharacterStats(float health, float fury, float end, float speed, int damage, float atspd, float cdr, float armor)
    {
        stats.MaxHealth.Value = health;
        stats.MaxFury.Value = fury;
        stats.MaxEndurance.Value = end;

        stats.Speed = speed;
        stats.Damage = damage;
        stats.AttackSpeed = atspd;
        stats.CoolDownReduction = cdr;
        stats.Armor = armor;

        stats.Health.Value = health;
        stats.Fury.Value = 0;
        stats.Endurance.Value = end;
    }

    [ServerRpc]
    void LoadCharacterStatsServerRPC(float health, float fury, float end, float speed, int damage, float atspd, float cdr, float armor)
    {
        ApplyCharacterStats(health, fury, end, speed, damage, atspd, cdr, armor);
    }

    void LoadPlayerSkills()
    {
        int slot = stats.net_CharacterSlot.Value;

        player.FirstPassiveIndex = PlayerPrefs.GetInt($"{slot}FirstPassive", 0);
        player.SecondPassiveIndex = PlayerPrefs.GetInt($"{slot}SecondPassive", -1);
        player.ThirdPassiveIndex = PlayerPrefs.GetInt($"{slot}ThirdPassive", -1);
        player.BasicIndex = PlayerPrefs.GetInt($"{slot}Basic", 0);
        player.OffensiveIndex = PlayerPrefs.GetInt($"{slot}Offensive", -1);
        player.MobilityIndex = PlayerPrefs.GetInt($"{slot}Mobility", -1);
        player.DefensiveIndex = PlayerPrefs.GetInt($"{slot}Defensive", -1);
        player.UtilityIndex = PlayerPrefs.GetInt($"{slot}Utility", -1);
        player.UltimateIndex = PlayerPrefs.GetInt($"{slot}Ultimate", -1);
    }

    public void SaveStats()
    {
        int slot = PlayerPrefs.GetInt("SelectedCharacter");

        PlayerPrefs.SetInt($"{slot}PlayerLevel", stats.PlayerLevel.Value);
        PlayerPrefs.SetFloat($"{slot}CurrentExperience", stats.CurrentExperience.Value);
        PlayerPrefs.SetFloat($"{slot}RequiredExperience", stats.RequiredExperience.Value);
        PlayerPrefs.SetFloat($"{slot}Coins", stats.Coins);
        PlayerPrefs.SetInt($"{slot}AP", stats.AttributePoints.Value);

        // Stats
        PlayerPrefs.SetFloat($"{slot}MaxHealth", stats.MaxHealth.Value);
        PlayerPrefs.SetFloat($"{slot}MaxFury", stats.MaxFury.Value);
        PlayerPrefs.SetFloat($"{slot}MaxEndurance", stats.MaxEndurance.Value);

        PlayerPrefs.SetFloat($"{slot}Speed", stats.Speed);
        PlayerPrefs.SetInt($"{slot}Damage", stats.Damage);
        PlayerPrefs.SetFloat($"{slot}AttackSpeed", stats.AttackSpeed);
        PlayerPrefs.SetFloat($"{slot}CDR", stats.CoolDownReduction);
        PlayerPrefs.SetFloat($"{slot}Armor", stats.Armor);

        // Skills
        PlayerPrefs.SetInt($"{slot}FirstPassive", player.FirstPassiveIndex);
        PlayerPrefs.SetInt($"{slot}SecondPassive", player.SecondPassiveIndex);
        PlayerPrefs.SetInt($"{slot}ThirdPassive", player.ThirdPassiveIndex);
        PlayerPrefs.SetInt($"{slot}Basic", player.BasicIndex);
        PlayerPrefs.SetInt($"{slot}Offensive", player.OffensiveIndex);
        PlayerPrefs.SetInt($"{slot}Mobility", player.MobilityIndex);
        PlayerPrefs.SetInt($"{slot}Defensive", player.DefensiveIndex);
        PlayerPrefs.SetInt($"{slot}Utility", player.UtilityIndex);
        PlayerPrefs.SetInt($"{slot}Ultimate", player.UltimateIndex);

        PlayerPrefs.Save();
        StartCoroutine(SaveText());
    }

    public void SaveInventory(Item item, int slotIndex, int quantity, bool saveImmediately = true)
    {
        string prefix = $"Character{PlayerPrefs.GetInt("SelectedCharacter")}_";
        string key = $"{prefix}InventorySlot_{slotIndex}";

        if (item == null || quantity <= 0)
        {
            PlayerPrefs.DeleteKey(key);
            if (saveImmediately) PlayerPrefs.Save();
            return;
        }

        string value = item.name.Replace("(Clone)", "").Trim() + "|" + quantity;
        PlayerPrefs.SetString(key, value);
        if (saveImmediately) PlayerPrefs.Save();
    }

    public void SaveEquipment(Item item, int slotIndex)
    {
        string prefix = $"Character{PlayerPrefs.GetInt("SelectedCharacter")}_";
        string key = $"{prefix}EquipmentSlot_{slotIndex}";

        if (item == null)
        {
            PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();
            return;
        }

        string baseName = item.name.Replace("(Clone)", "").Trim();
        PlayerPrefs.SetString(key, baseName);
        PlayerPrefs.Save();
    }

    IEnumerator SaveText()
    {
        saveText.text = "Save";
        yield return new WaitForSeconds(1);
        saveText.text = "";
    }
}
