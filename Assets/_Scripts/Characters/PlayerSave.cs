using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerSave : NetworkBehaviour
{
    [Header("Data")]
    [SerializeField] CharacterCustomizationData customizationData;

    [Header("References")]
    Player player;
    PlayerCustomization customization;
    PlayerStats stats;
    Inventory inventory;
    EquipmentManager equipment;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI saveText;
    [SerializeField] TextMeshProUGUI coinText;

    private void Awake()
    {
        player = GetComponent<Player>();
        stats = GetComponent<PlayerStats>();
        customization = GetComponent<PlayerCustomization>();
        inventory = GetComponentInChildren<Inventory>();
        equipment = GetComponentInChildren<EquipmentManager>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            SetSelectedCharacterServerRpc(PlayerPrefs.GetInt("SelectedCharacter"));

            LoadCustomization();
            LoadPlayerStats();
            inventory.LoadInventory();
            equipment.LoadEquipment();

            // Set Coin Text UI
            coinText.text = $"{stats.Coins}<sprite index=0>";
            
        }
        else
        {

        }
    }

    void LoadCustomization()
    {
        int prefix = stats.net_CharacterSlot.Value;

        // Load Customization
        customization.playerNameText.text = PlayerPrefs.GetString($"Character{prefix}Name");
        customization.bodySprite.color = customizationData.skinColors[PlayerPrefs.GetInt($"Character{prefix}SkinColor")];
        customization.hairSprite.color = customizationData.hairColors[PlayerPrefs.GetInt($"Character{prefix}HairColor")];
        customization.hairIndex = PlayerPrefs.GetInt($"Character{prefix}HairStyle");

        // Set Customization
        customization.playerNameText.text = stats.net_playerName.Value.ToString();
        stats.net_playerName.Value = customization.playerNameText.text;
        stats.net_bodyColor.Value = customization.bodySprite.color;
        stats.net_hairColor.Value = customization.hairSprite.color;
    }

    public void LoadPlayerStats()
    {
        string prefix = $"Character{PlayerPrefs.GetInt("SelectedCharacter")}_";

        // Load from PlayerPrefs
        int level = PlayerPrefs.GetInt($"{prefix}PlayerLevel", 1);
        float currentExp = PlayerPrefs.GetFloat($"{prefix}CurrentExperience", 0);
        float requiredExp = PlayerPrefs.GetFloat($"{prefix}RequiredExperience", 10);
        float coins = PlayerPrefs.GetFloat($"{prefix}Coins", 0);
        int ap = PlayerPrefs.GetInt($"{prefix}AP", 0);

        float maxHealth = PlayerPrefs.GetFloat($"{prefix}MaxHealth", 10);
        float maxFury = PlayerPrefs.GetFloat($"{prefix}MaxFury", 100);
        float maxEndurance = PlayerPrefs.GetFloat($"{prefix}MaxEndurance", 100);

        float baseSpeed = PlayerPrefs.GetFloat($"{prefix}Speed", 5);
        int baseDamage = PlayerPrefs.GetInt($"{prefix}Damage", 1);
        float baseAttackSpeed = PlayerPrefs.GetFloat($"{prefix}AttackSpeed", 1);
        float baseCDR = PlayerPrefs.GetFloat($"{prefix}CDR", 1);
        float baseArmor = PlayerPrefs.GetFloat($"{prefix}Armor", 0);

        // Non-NetworkVariable stats (can set directly)
        stats.Coins = coins;
        player.FirstPassiveIndex = PlayerPrefs.GetInt($"{prefix}FirstPassive", 0);
        player.SecondPassiveIndex = PlayerPrefs.GetInt($"{prefix}SecondPassive", -1);
        player.ThirdPassiveIndex = PlayerPrefs.GetInt($"{prefix}ThirdPassive", -1);
        player.BasicIndex = PlayerPrefs.GetInt($"{prefix}Basic", 0);
        player.OffensiveIndex = PlayerPrefs.GetInt($"{prefix}Offensive", -1);
        player.MobilityIndex = PlayerPrefs.GetInt($"{prefix}Mobility", -1);
        player.DefensiveIndex = PlayerPrefs.GetInt($"{prefix}Defensive", -1);
        player.UtilityIndex = PlayerPrefs.GetInt($"{prefix}Utility", -1);
        player.UltimateIndex = PlayerPrefs.GetInt($"{prefix}Ultimate", -1);

        // NetworkVariables (need server authority)
        if (IsServer)
        {
            // Server can set directly
            SetPlayerStats(level, currentExp, requiredExp, ap, maxHealth, maxFury, maxEndurance,
                baseSpeed, baseDamage, baseAttackSpeed, baseCDR, baseArmor);
        }
        else
        {
            // Client asks server to set them
            SetPlayerStatsServerRpc(level, currentExp, requiredExp, ap, maxHealth, maxFury, maxEndurance,
                baseSpeed, baseDamage, baseAttackSpeed, baseCDR, baseArmor);
        }
    }

    [ServerRpc]
    void SetPlayerStatsServerRpc(int level, float currentExp, float requiredExp, int ap,
    float maxHealth, float maxFury, float maxEndurance, float baseSpeed, int baseDamage,
    float baseAttackSpeed, float baseCDR, float baseArmor)
    {
        SetPlayerStats(level, currentExp, requiredExp, ap, maxHealth, maxFury, maxEndurance,
            baseSpeed, baseDamage, baseAttackSpeed, baseCDR, baseArmor);
    }

    void SetPlayerStats(int level, float currentExp, float requiredExp, int ap,
        float maxHealth, float maxFury, float maxEndurance, float baseSpeed, int baseDamage,
        float baseAttackSpeed, float baseCDR, float baseArmor)
    {
        // Set NetworkVariables
        stats.PlayerLevel.Value = level;
        stats.CurrentExperience.Value = currentExp;
        stats.RequiredExperience.Value = requiredExp;
        stats.AttributePoints.Value = ap;

        stats.MaxHealth.Value = maxHealth;
        stats.MaxFury.Value = maxFury;
        stats.MaxEndurance.Value = maxEndurance;

        stats.Speed.Value = baseSpeed;
        stats.Damage.Value = baseDamage;
        stats.AttackSpeed.Value = baseAttackSpeed;
        stats.CoolDownReduction.Value = baseCDR;
        stats.Armor.Value = baseArmor;

        // Set current values to full/base
        stats.Health.Value = maxHealth;
        stats.Fury.Value = 0;
        stats.Endurance.Value = maxEndurance;
    }

    public void SaveStats()
    {
        string prefix = $"Character{PlayerPrefs.GetInt("SelectedCharacter")}_";

        PlayerPrefs.SetInt($"{prefix}PlayerLevel", stats.PlayerLevel.Value);
        PlayerPrefs.SetFloat($"{prefix}CurrentExperience", stats.CurrentExperience.Value);
        PlayerPrefs.SetFloat($"{prefix}RequiredExperience", stats.RequiredExperience.Value);
        PlayerPrefs.SetFloat($"{prefix}Coins", stats.Coins);
        PlayerPrefs.SetInt($"{prefix}AP", stats.AttributePoints.Value);

        // Stats
        PlayerPrefs.SetFloat($"{prefix}Health", stats.Health.Value);
        PlayerPrefs.SetFloat($"{prefix}MaxHealth", stats.MaxHealth.Value);

        PlayerPrefs.SetFloat($"{prefix}Fury", stats.Fury.Value);
        PlayerPrefs.SetFloat($"{prefix}MaxFury", stats.MaxFury.Value);

        PlayerPrefs.SetFloat($"{prefix}Endurance", stats.Endurance.Value);
        PlayerPrefs.SetFloat($"{prefix}MaxEndurance", stats.MaxEndurance.Value);

        PlayerPrefs.SetFloat($"{prefix}Speed", stats.Speed.Value);
        PlayerPrefs.SetInt($"{prefix}Damage", stats.Damage.Value);
        PlayerPrefs.SetFloat($"{prefix}AttackSpeed", stats.AttackSpeed.Value);
        PlayerPrefs.SetFloat($"{prefix}CDR", stats.CoolDownReduction.Value);
        PlayerPrefs.SetFloat($"{prefix}Armor", stats.Armor.Value);

        // Skills
        PlayerPrefs.SetInt($"{prefix}FirstPassive", player.FirstPassiveIndex);
        PlayerPrefs.SetInt($"{prefix}SecondPassive", player.SecondPassiveIndex);
        PlayerPrefs.SetInt($"{prefix}ThirdPassive", player.ThirdPassiveIndex);
        PlayerPrefs.SetInt($"{prefix}Basic", player.BasicIndex);
        PlayerPrefs.SetInt($"{prefix}Offensive", player.OffensiveIndex);
        PlayerPrefs.SetInt($"{prefix}Mobility", player.MobilityIndex);
        PlayerPrefs.SetInt($"{prefix}Defensive", player.DefensiveIndex);
        PlayerPrefs.SetInt($"{prefix}Utility", player.UtilityIndex);
        PlayerPrefs.SetInt($"{prefix}Ultimate", player.UltimateIndex);

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

    [ServerRpc]
    void SetSelectedCharacterServerRpc(int slot)
    {
        stats.net_CharacterSlot.Value = slot;
    }
}
