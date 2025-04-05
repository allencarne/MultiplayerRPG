using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerInitialize : NetworkBehaviour
{
    [SerializeField] TextMeshProUGUI playerNameText;
    [SerializeField] SpriteRenderer bodySprite;
    [SerializeField] SpriteRenderer hairSprite;

    [Header("Network Variables")]
    private NetworkVariable<FixedString32Bytes> net_playerName = new NetworkVariable<FixedString32Bytes>(writePerm: NetworkVariableWritePermission.Owner);
    private NetworkVariable<Color> net_bodyColor = new NetworkVariable<Color>(writePerm: NetworkVariableWritePermission.Owner);
    private NetworkVariable<Color> net_hairColor = new NetworkVariable<Color>(writePerm: NetworkVariableWritePermission.Owner);

    [SerializeField] Player player;
    [SerializeField] CharacterCustomizationData customizationData;
    private string CharacterNumber => $"Character{PlayerPrefs.GetInt("SelectedCharacter")}_";

    public override void OnNetworkSpawn()
    {
        net_playerName.OnValueChanged += OnNameChanged;
        net_bodyColor.OnValueChanged += OnBodyColorChanged;
        net_hairColor.OnValueChanged += OnHairColorChanged;

        if (IsOwner)
        {
            LoadCustomization();
            LoadPlayerStats();

            // Set Coin Text UI
            player.CoinText.text = player.Coins.ToString();
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

        player.PlayerLevel = PlayerPrefs.GetInt($"{prefix}PlayerLevel", 1);
        player.CurrentExperience = PlayerPrefs.GetFloat($"{prefix}CurrentExperience", 0);
        player.RequiredExperience = PlayerPrefs.GetFloat($"{prefix}RequiredExperience", 10);
        player.Coins = PlayerPrefs.GetFloat($"{prefix}Coins", 0);

        player.Health = PlayerPrefs.GetFloat($"{prefix}Health", 10);
        player.MaxHealth = PlayerPrefs.GetFloat($"{prefix}MaxHealth", 10);

        player.Endurance = PlayerPrefs.GetFloat($"{prefix}Endurance", 100);
        player.MaxEndurance = PlayerPrefs.GetFloat($"{prefix}MaxEndurance", 100);

        player.Speed = PlayerPrefs.GetFloat($"{prefix}Speed", 5);
        player.CurrentSpeed = PlayerPrefs.GetFloat($"{prefix}CurrentSpeed", 5);

        player.Damage = PlayerPrefs.GetInt($"{prefix}Damage", 1);
        player.CurrentDamage = PlayerPrefs.GetInt($"{prefix}CurrentDamage", 1);

        player.AttackSpeed = PlayerPrefs.GetFloat($"{prefix}AttackSpeed", 1);
        player.CurrentAttackSpeed = PlayerPrefs.GetFloat($"{prefix}CurrentAttackSpeed", 1);

        player.CDR = PlayerPrefs.GetFloat($"{prefix}CDR", 1);
        player.CurrentCDR = PlayerPrefs.GetFloat($"{prefix}CurrentCDR", 1);

        player.BaseArmor = PlayerPrefs.GetFloat($"{prefix}BaseArmor", 0);
        player.CurrentArmor = PlayerPrefs.GetFloat($"{prefix}CurrentArmor", 0);
    }


    public void SavePlayerStats()
    {
        string prefix = CharacterNumber;

        PlayerPrefs.SetInt($"{prefix}PlayerLevel", player.PlayerLevel);
        PlayerPrefs.SetFloat($"{prefix}CurrentExperience", player.CurrentExperience);
        PlayerPrefs.SetFloat($"{prefix}RequiredExperience", player.RequiredExperience);
        PlayerPrefs.SetFloat($"{prefix}Coins", player.Coins);

        PlayerPrefs.SetFloat($"{prefix}Health", player.Health);
        PlayerPrefs.SetFloat($"{prefix}MaxHealth", player.MaxHealth);

        PlayerPrefs.SetFloat($"{prefix}Endurance", player.Endurance);
        PlayerPrefs.SetFloat($"{prefix}MaxEndurance", player.MaxEndurance);

        PlayerPrefs.SetFloat($"{prefix}Speed", player.Speed);
        PlayerPrefs.SetFloat($"{prefix}CurrentSpeed", player.CurrentSpeed);

        PlayerPrefs.SetInt($"{prefix}Damage", player.Damage);
        PlayerPrefs.SetInt($"{prefix}CurrentDamage", player.CurrentDamage);

        PlayerPrefs.SetFloat($"{prefix}AttackSpeed", player.AttackSpeed);
        PlayerPrefs.SetFloat($"{prefix}CurrentAttackSpeed", player.CurrentAttackSpeed);

        PlayerPrefs.SetFloat($"{prefix}CDR", player.CDR);
        PlayerPrefs.SetFloat($"{prefix}CurrentCDR", player.CurrentCDR);

        PlayerPrefs.SetFloat($"{prefix}BaseArmor", player.BaseArmor);
        PlayerPrefs.SetFloat($"{prefix}CurrentArmor", player.CurrentArmor);

        PlayerPrefs.Save();
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
