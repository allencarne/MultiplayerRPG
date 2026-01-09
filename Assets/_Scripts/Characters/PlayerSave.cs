using System.Collections;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerSave : NetworkBehaviour
{
    [Header("Data")]
    [SerializeField] CharacterCustomizationData customizationData;
    bool statsInitialized;

    [Header("References")]
    Player player;
    PlayerCustomization custom;
    PlayerStats stats;
    Inventory inventory;
    EquipmentManager equipment;
    [SerializeField] AttributePoints ap;
    [SerializeField] PlayerExperience exp;
    [SerializeField] SkillPanel skillPanel;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI saveText;

    private void Awake()
    {
        player = GetComponent<Player>();
        custom = GetComponent<PlayerCustomization>();
        stats = GetComponent<PlayerStats>();
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
            LoadCharacterStats();
            LoadPlayerSkills();
            inventory.LoadInventory();
            equipment.LoadEquipment();
            exp.Initialize();

            statsInitialized = true;
        }
        else
        {
            switch (stats.playerClass)
            {
                case PlayerStats.PlayerClass.Beginner:
                    custom.playerNameText.text = $"<sprite name=\"Beginner_Icon\"> {stats.net_playerName.Value}";
                    break;
                case PlayerStats.PlayerClass.Warrior:
                    custom.playerNameText.text = $"<sprite name=\"Warrior_Icon\"> {stats.net_playerName.Value}";
                    break;
                case PlayerStats.PlayerClass.Magician:
                    custom.playerNameText.text = $"<sprite name=\"Magician_Icon\"> {stats.net_playerName.Value}";
                    break;
                case PlayerStats.PlayerClass.Archer:
                    custom.playerNameText.text = $"<sprite name=\"Archer_Icon\"> {stats.net_playerName.Value}";
                    break;
                case PlayerStats.PlayerClass.Rogue:
                    custom.playerNameText.text = $"<sprite name=\"Rogue_Icon\"> {stats.net_playerName.Value}";
                    break;
            }
            custom.bodySprite.color = stats.net_bodyColor.Value;
            custom.playerHeadSprite.color = stats.net_bodyColor.Value;
            custom.hairSprite.color = stats.net_hairColor.Value;

            Material eyeMat = custom.eyesSprite.material;
            eyeMat.SetColor("_NewColor", stats.net_eyeColor.Value);
        }

        ap.OnStatsApplied.AddListener(SaveStats);
        exp.OnEXP.AddListener(SaveStats);
        skillPanel.OnSkillSelected.AddListener(SaveStats);
    }

    public override void OnNetworkDespawn()
    {
        ap.OnStatsApplied.RemoveListener(SaveStats);
        exp.OnEXP.RemoveListener(SaveStats);
        skillPanel.OnSkillSelected.RemoveListener(SaveStats);
    }

    [ServerRpc]
    void SetSelectedCharacterServerRpc(int slot)
    {
        stats.net_CharacterSlot.Value = slot;
    }

    void LoadCustomization()
    {
        int slot = PlayerPrefs.GetInt("SelectedCharacter");

        string name = PlayerPrefs.GetString($"Character{slot}Name", "No Name");
        int hairIndex = PlayerPrefs.GetInt($"Character{slot}HairStyle");
        int eyeIndex = PlayerPrefs.GetInt($"Character{slot}EyeStyle");
        Color skinCol = customizationData.skinColors[PlayerPrefs.GetInt($"Character{slot}SkinColor")];
        Color hairCol = customizationData.hairColors[PlayerPrefs.GetInt($"Character{slot}HairColor")];
        Color eyeCol = customizationData.eyeColors[PlayerPrefs.GetInt($"Character{slot}EyeColor")];

        switch (stats.playerClass)
        {
            case PlayerStats.PlayerClass.Beginner:
                custom.playerNameText.text = $"<sprite name=\"Beginner_Icon\"> {name}";
                break;
            case PlayerStats.PlayerClass.Warrior:
                custom.playerNameText.text = $"<sprite name=\"Warrior_Icon\"> {name}";
                break;
            case PlayerStats.PlayerClass.Magician:
                custom.playerNameText.text = $"<sprite name=\"Magician_Icon\"> {name}";
                break;
            case PlayerStats.PlayerClass.Archer:
                custom.playerNameText.text = $"<sprite name=\"Archer_Icon\"> {name}";
                break;
            case PlayerStats.PlayerClass.Rogue:
                custom.playerNameText.text = $"<sprite name=\"Rogue_Icon\"> {name}";
                break;
        }

        custom.bodySprite.color = skinCol;
        custom.playerHeadSprite.color = skinCol;
        custom.hairSprite.color = hairCol;

        Material eyeMat = custom.eyesSprite.material;
        eyeMat.SetColor("_NewColor", eyeCol);

        if (IsServer)
        {
            ApplyCustomization(slot, name, skinCol, hairCol, eyeCol, hairIndex, eyeIndex);
        }
        else
        {
            LoadCustomizationServerRPC(slot, name, skinCol, hairCol, eyeCol, hairIndex, eyeIndex);
        }
    }

    void ApplyCustomization(int slot, FixedString32Bytes name, Color skin, Color hair, Color eye, int hairIndex, int eyeIndex)
    {
        stats.net_CharacterSlot.Value = slot;
        stats.net_playerName.Value = name;
        stats.net_bodyColor.Value = skin;
        stats.net_hairColor.Value = hair;
        stats.net_eyeColor.Value = eye;

        custom.net_HairIndex.Value = hairIndex;
        custom.net_EyeIndex.Value = eyeIndex;
    }

    [ServerRpc]
    void LoadCustomizationServerRPC(int slot, FixedString32Bytes name, Color skin, Color hair, Color eye, int hairIndex, int eyeIndex)
    {
        ApplyCustomization(slot, name, skin, hair, eye, hairIndex, eyeIndex);
    }

    void LoadPlayerStats()
    {
        int slot = PlayerPrefs.GetInt("SelectedCharacter");

        int level = PlayerPrefs.GetInt($"{slot}PlayerLevel", 1);
        int playerClass = PlayerPrefs.GetInt($"{slot}PlayerClass", 0);
        float currentExp = PlayerPrefs.GetFloat($"{slot}CurrentExperience", 0);
        int ap = PlayerPrefs.GetInt($"{slot}AP", 0);
        float coins = PlayerPrefs.GetFloat($"{slot}Coins", 0);

        stats.playerClass = (PlayerStats.PlayerClass)playerClass;

        if (IsServer)
        {
            ApplyPlayerStats(level, currentExp, coins, ap);
        }
        else
        {
            LoadPlayerStatsServerRPC(level, currentExp, coins, ap);
        }
    }

    void ApplyPlayerStats(int level, float cEXP, float coins, int ap)
    {
        stats.PlayerLevel.Value = level;
        stats.CurrentExperience.Value = cEXP;
        stats.AttributePoints.Value = ap;
        stats.Coins = coins;
    }

    [ServerRpc]
    void LoadPlayerStatsServerRPC(int level, float cEXP, float coins, int ap)
    {
        ApplyPlayerStats(level, cEXP, coins, ap);
    }

    void LoadCharacterStats()
    {
        int slot = PlayerPrefs.GetInt("SelectedCharacter");

        float health = PlayerPrefs.GetFloat($"{slot}MaxHealth", 10);
        float fury = PlayerPrefs.GetFloat($"{slot}MaxFury", 100);
        float end = PlayerPrefs.GetFloat($"{slot}MaxEndurance", 100);
        float endrech = PlayerPrefs.GetFloat($"{slot}EnduranceRecharge", 1);

        stats.BaseSpeed = PlayerPrefs.GetFloat($"{slot}Speed", 5);
        stats.BaseDamage = PlayerPrefs.GetFloat($"{slot}Damage", 1);
        stats.BaseAS = PlayerPrefs.GetFloat($"{slot}AttackSpeed", 1);
        stats.BaseCDR = PlayerPrefs.GetFloat($"{slot}CDR", 1);
        stats.BaseArmor = PlayerPrefs.GetFloat($"{slot}Armor", 0);

        if (IsServer)
        {
            ApplyCharacterStats(health, fury, end, endrech);
        }
        else
        {
            LoadCharacterStatsServerRPC(health, fury, end, endrech);
        }
    }

    void ApplyCharacterStats(float health, float fury, float end, float endrech)
    {
        stats.net_BaseHP.Value = health;
        stats.MaxFury.Value = fury;
        stats.MaxEndurance.Value = end;
        stats.EnduranceRechargeRate.Value = endrech;


        stats.net_CurrentHP.Value = health;
        stats.Fury.Value = 0;
        stats.Endurance.Value = end;

        float modHealth = stats.GetModifier(StatType.Health);
        stats.RecalculateTotalHealth(modHealth);
    }

    [ServerRpc]
    void LoadCharacterStatsServerRPC(float health, float fury, float end, float endrech)
    {
        ApplyCharacterStats(health, fury, end, endrech);
    }

    void LoadPlayerSkills()
    {
        int slot = PlayerPrefs.GetInt("SelectedCharacter");

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
        if (!IsOwner) return;
        if (!statsInitialized) return;

        int slot = PlayerPrefs.GetInt("SelectedCharacter");

        PlayerPrefs.SetInt($"{slot}PlayerLevel", stats.PlayerLevel.Value);
        PlayerPrefs.SetInt($"{slot}PlayerClass", (int)stats.playerClass);
        PlayerPrefs.SetFloat($"{slot}CurrentExperience", stats.CurrentExperience.Value);
        PlayerPrefs.SetFloat($"{slot}RequiredExperience", stats.RequiredExperience.Value);
        PlayerPrefs.SetFloat($"{slot}Coins", stats.Coins);
        PlayerPrefs.SetInt($"{slot}AP", stats.AttributePoints.Value);

        // Stats
        PlayerPrefs.SetFloat($"{slot}MaxHealth", stats.net_BaseHP.Value);
        PlayerPrefs.SetFloat($"{slot}MaxFury", stats.MaxFury.Value);
        PlayerPrefs.SetFloat($"{slot}MaxEndurance", stats.MaxEndurance.Value);
        PlayerPrefs.SetFloat($"{slot}EnduranceRecharge", stats.EnduranceRechargeRate.Value);

        PlayerPrefs.SetFloat($"{slot}Speed", stats.BaseSpeed);
        PlayerPrefs.SetFloat($"{slot}Damage", stats. BaseDamage);
        PlayerPrefs.SetFloat($"{slot}AttackSpeed", stats.BaseAS);
        PlayerPrefs.SetFloat($"{slot}CDR", stats.BaseCDR);
        PlayerPrefs.SetFloat($"{slot}Armor", stats.BaseArmor);

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
