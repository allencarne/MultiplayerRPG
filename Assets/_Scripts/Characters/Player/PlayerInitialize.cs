using System.Collections;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerInitialize : NetworkBehaviour
{
    [SerializeField] TextMeshProUGUI saveText;
    [SerializeField] TextMeshProUGUI playerNameText;
    [SerializeField] SpriteRenderer bodySprite;
    [SerializeField] SpriteRenderer hairSprite;

    [Header("Network Variables")]
    public NetworkVariable<int> net_CharacterSlot = new(writePerm: NetworkVariableWritePermission.Server);
    private NetworkVariable<FixedString32Bytes> net_playerName = new NetworkVariable<FixedString32Bytes>(writePerm: NetworkVariableWritePermission.Owner);
    public NetworkVariable<Color> net_bodyColor = new NetworkVariable<Color>(writePerm: NetworkVariableWritePermission.Owner);
    private NetworkVariable<Color> net_hairColor = new NetworkVariable<Color>(writePerm: NetworkVariableWritePermission.Owner);

    [SerializeField] Player player;
    [SerializeField] CharacterCustomizationData customizationData;
    [SerializeField] Inventory inventory;
    [SerializeField] EquipmentManager equipment;
    public string CharacterNumber => $"Character{PlayerPrefs.GetInt("SelectedCharacter")}_";

    public override void OnNetworkSpawn()
    {
        net_playerName.OnValueChanged += OnNameChanged;
        net_bodyColor.OnValueChanged += OnBodyColorChanged;
        net_hairColor.OnValueChanged += OnHairColorChanged;

        if (IsOwner)
        {
            int slot = PlayerPrefs.GetInt("SelectedCharacter");
            SetSelectedCharacterServerRpc(slot);

            LoadCustomization();
            LoadPlayerStats();
            inventory.LoadInventory();
            equipment.LoadEquipment();

            // Set Coin Text UI
            player.CoinText.text = $"{player.Coins}<sprite index=0>";
            player.PlayerName = net_playerName.Value.ToString();
        }
        else
        {
            playerNameText.text = net_playerName.Value.ToString();
            bodySprite.color = net_bodyColor.Value;
            hairSprite.color = net_hairColor.Value;
        }
    }

    public override void OnDestroy()
    {
        // Unsubscribe from the callbacks to avoid memory leaks
        net_playerName.OnValueChanged -= OnNameChanged;
        net_bodyColor.OnValueChanged -= OnBodyColorChanged;
        net_hairColor.OnValueChanged -= OnHairColorChanged;
    }

    void LoadCustomization()
    {
        // Set initial values based on the selected character
        switch (PlayerPrefs.GetInt("SelectedCharacter"))
        {
            case 1:
                playerNameText.text = PlayerPrefs.GetString("Character1Name");
                bodySprite.color = customizationData.skinColors[PlayerPrefs.GetInt("Character1SkinColor")];
                hairSprite.color = customizationData.hairColors[PlayerPrefs.GetInt("Character1HairColor")];
                player.hairIndex = PlayerPrefs.GetInt("Character1HairStyle");
                break;
            case 2:
                playerNameText.text = PlayerPrefs.GetString("Character2Name");
                bodySprite.color = customizationData.skinColors[PlayerPrefs.GetInt("Character2SkinColor")];
                hairSprite.color = customizationData.hairColors[PlayerPrefs.GetInt("Character2HairColor")];
                player.hairIndex = PlayerPrefs.GetInt("Character2HairStyle");
                break;
            case 3:
                playerNameText.text = PlayerPrefs.GetString("Character3Name");
                bodySprite.color = customizationData.skinColors[PlayerPrefs.GetInt("Character3SkinColor")];
                hairSprite.color = customizationData.hairColors[PlayerPrefs.GetInt("Character3HairColor")];
                player.hairIndex = PlayerPrefs.GetInt("Character3HairStyle");
                break;
        }

        net_playerName.Value = playerNameText.text;
        net_bodyColor.Value = bodySprite.color;
        net_hairColor.Value = hairSprite.color;
    }

    public void LoadPlayerStats()
    {
        string prefix = CharacterNumber;

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
        float baseArmor = PlayerPrefs.GetFloat($"{prefix}BaseArmor", 0);

        // Non-NetworkVariable stats (can set directly)
        player.Coins = coins;
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
        player.PlayerLevel.Value = level;
        player.CurrentExperience.Value = currentExp;
        player.RequiredExperience.Value = requiredExp;
        player.AttributePoints.Value = ap;

        player.MaxHealth.Value = maxHealth;
        player.MaxFury.Value = maxFury;
        player.MaxEndurance.Value = maxEndurance;

        player.BaseSpeed.Value = baseSpeed;
        player.BaseDamage.Value = baseDamage;
        player.BaseAttackSpeed.Value = baseAttackSpeed;
        player.BaseCDR.Value = baseCDR;
        player.BaseArmor.Value = baseArmor;

        // Set current values to full/base
        player.Health.Value = maxHealth;
        player.Fury.Value = 0;
        player.Endurance.Value = maxEndurance;
        player.CurrentSpeed.Value = baseSpeed;
        player.CurrentDamage.Value = baseDamage;
        player.CurrentAttackSpeed.Value = baseAttackSpeed;
        player.CurrentCDR.Value = baseCDR;
        player.CurrentArmor.Value = baseArmor;
    }

    public void SaveStats()
    {
        string prefix = CharacterNumber;

        PlayerPrefs.SetInt($"{prefix}PlayerLevel", player.PlayerLevel.Value);
        PlayerPrefs.SetFloat($"{prefix}CurrentExperience", player.CurrentExperience.Value);
        PlayerPrefs.SetFloat($"{prefix}RequiredExperience", player.RequiredExperience.Value);
        PlayerPrefs.SetFloat($"{prefix}Coins", player.Coins);
        PlayerPrefs.SetInt($"{prefix}AP", player.AttributePoints.Value);

        // Stats
        PlayerPrefs.SetFloat($"{prefix}Health", player.Health.Value);
        PlayerPrefs.SetFloat($"{prefix}MaxHealth", player.MaxHealth.Value);

        PlayerPrefs.SetFloat($"{prefix}Fury", player.Fury.Value);
        PlayerPrefs.SetFloat($"{prefix}MaxFury", player.MaxFury.Value);

        PlayerPrefs.SetFloat($"{prefix}Endurance", player.Endurance.Value);
        PlayerPrefs.SetFloat($"{prefix}MaxEndurance", player.MaxEndurance.Value);

        PlayerPrefs.SetFloat($"{prefix}Speed", player.BaseSpeed.Value);
        PlayerPrefs.SetFloat($"{prefix}CurrentSpeed", player.CurrentSpeed.Value);

        PlayerPrefs.SetInt($"{prefix}Damage", player.BaseDamage.Value);
        PlayerPrefs.SetInt($"{prefix}CurrentDamage", player.CurrentDamage.Value);

        PlayerPrefs.SetFloat($"{prefix}AttackSpeed", player.BaseAttackSpeed.Value);
        PlayerPrefs.SetFloat($"{prefix}CurrentAttackSpeed", player.CurrentAttackSpeed.Value);

        PlayerPrefs.SetFloat($"{prefix}CDR", player.BaseCDR.Value);
        PlayerPrefs.SetFloat($"{prefix}CurrentCDR", player.CurrentCDR.Value);

        PlayerPrefs.SetFloat($"{prefix}BaseArmor", player.BaseArmor.Value);
        PlayerPrefs.SetFloat($"{prefix}CurrentArmor", player.CurrentArmor.Value);

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
        string prefix = CharacterNumber;
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
        string prefix = CharacterNumber;
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

    void OnNameChanged(FixedString32Bytes oldName, FixedString32Bytes newName)
    {
        playerNameText.text = newName.ToString();
    }

    void OnBodyColorChanged(Color previousColor, Color newColor)
    {
        bodySprite.color = newColor;
    }

    void OnHairColorChanged(Color previousColor, Color newColor)
    {
        hairSprite.color = newColor;
    }

    [ServerRpc]
    void SetSelectedCharacterServerRpc(int slot)
    {
        net_CharacterSlot.Value = slot;
    }
}
