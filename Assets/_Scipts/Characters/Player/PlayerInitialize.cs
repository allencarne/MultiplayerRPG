using System.Collections;
using System.Collections.Generic;
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
            LoadCustomization();

            if (IsServer)
            {
                LoadPlayerStats();
                inventory.LoadInventory();
                equipment.LoadEquipment();
            }

            // Set Coin Text UI
            player.CoinText.text = player.Coins.ToString();
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

        // Update NetworkVariables
        net_playerName.Value = playerNameText.text;
        net_bodyColor.Value = bodySprite.color;
        net_hairColor.Value = hairSprite.color;
    }

    public void LoadPlayerStats()
    {
        string prefix = CharacterNumber;

        player.PlayerLevel.Value = PlayerPrefs.GetInt($"{prefix}PlayerLevel", 1);
        player.CurrentExperience.Value = PlayerPrefs.GetFloat($"{prefix}CurrentExperience", 0);
        player.RequiredExperience.Value = PlayerPrefs.GetFloat($"{prefix}RequiredExperience", 10);
        player.Coins = PlayerPrefs.GetFloat($"{prefix}Coins", 0);
        player.AttributePoints.Value = PlayerPrefs.GetInt($"{prefix}AP", 0);

        // Stats
        player.Health.Value = PlayerPrefs.GetFloat($"{prefix}Health", 10);
        player.MaxHealth.Value = PlayerPrefs.GetFloat($"{prefix}MaxHealth", 10);

        player.Fury.Value = PlayerPrefs.GetFloat($"{prefix}Fury", 0);
        player.MaxFury.Value = PlayerPrefs.GetFloat($"{prefix}MaxFury", 100);

        player.Endurance.Value = PlayerPrefs.GetFloat($"{prefix}Endurance", 100);
        player.MaxEndurance.Value = PlayerPrefs.GetFloat($"{prefix}MaxEndurance", 100);

        player.BaseSpeed.Value = PlayerPrefs.GetFloat($"{prefix}Speed", 5);
        player.CurrentSpeed.Value = PlayerPrefs.GetFloat($"{prefix}CurrentSpeed", 5);

        player.BaseDamage.Value = PlayerPrefs.GetInt($"{prefix}Damage", 1);
        player.CurrentDamage.Value = PlayerPrefs.GetInt($"{prefix}CurrentDamage", 1);

        player.BaseAttackSpeed.Value = PlayerPrefs.GetFloat($"{prefix}AttackSpeed", 1);
        player.CurrentAttackSpeed.Value = PlayerPrefs.GetFloat($"{prefix}CurrentAttackSpeed", 1);

        player.BaseCDR.Value = PlayerPrefs.GetFloat($"{prefix}CDR", 1);
        player.CurrentCDR.Value = PlayerPrefs.GetFloat($"{prefix}CurrentCDR", 1);

        player.BaseArmor.Value = PlayerPrefs.GetFloat($"{prefix}BaseArmor", 0);
        player.CurrentArmor.Value = PlayerPrefs.GetFloat($"{prefix}CurrentArmor", 0);

        // Skills
        player.FirstPassiveIndex = PlayerPrefs.GetInt($"{prefix}FirstPassive", -1);
        player.SecondPassiveIndex = PlayerPrefs.GetInt($"{prefix}SecondPassive", -1);
        player.ThirdPassiveIndex = PlayerPrefs.GetInt($"{prefix}ThirdPassive", -1);
        player.BasicIndex = PlayerPrefs.GetInt($"{prefix}Basic", -1);
        player.OffensiveIndex = PlayerPrefs.GetInt($"{prefix}Offensive", -1);
        player.MobilityIndex = PlayerPrefs.GetInt($"{prefix}Mobility", -1);
        player.DefensiveIndex = PlayerPrefs.GetInt($"{prefix}Defensive", -1);
        player.UtilityIndex = PlayerPrefs.GetInt($"{prefix}Utility", -1);
        player.UltimateIndex = PlayerPrefs.GetInt($"{prefix}Ultimate", -1);

        // Set Values Here
        player.Health.Value = player.MaxHealth.Value;
        player.Fury.Value = 0;
        player.Endurance.Value = player.MaxEndurance.Value;
        player.CurrentSpeed.Value = player.BaseSpeed.Value;
        player.CurrentDamage.Value = player.BaseDamage.Value;
        player.CurrentAttackSpeed.Value = player.BaseAttackSpeed.Value;
        player.CurrentCDR.Value = player.BaseCDR.Value;
        player.CurrentArmor.Value = player.BaseArmor.Value;
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
            Debug.Log($"Cleared {key}");
            return;
        }

        string baseName = item.name.Replace("(Clone)", "").Trim();
        PlayerPrefs.SetString(key, baseName);
        PlayerPrefs.Save();

        Debug.Log($"Saved {key}: {baseName}");
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
}
