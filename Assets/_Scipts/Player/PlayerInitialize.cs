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

    private void Start()
    {
        LoadPlayerStats();

        // Set Coin Text UI
        player.CoinText.text = player.Coins.ToString();
    }

    public void SavePlayerStats()
    {
        PlayerPrefs.SetInt("PlayerLevel", player.PlayerLevel);
        PlayerPrefs.SetFloat("CurrentExperience", player.CurrentExperience);
        PlayerPrefs.SetFloat("RequiredExperience", player.RequiredExperience);
        PlayerPrefs.SetFloat("Coins", player.Coins);

        PlayerPrefs.SetFloat("Health", player.Health);
        PlayerPrefs.SetFloat("MaxHealth", player.MaxHealth);

        PlayerPrefs.SetFloat("Speed", player.Speed);
        PlayerPrefs.SetFloat("CurrentSpeed", player.CurrentSpeed);

        PlayerPrefs.SetInt("Damage", player.Damage);
        PlayerPrefs.SetInt("CurrentDamage", player.CurrentDamage);

        PlayerPrefs.SetFloat("AttackSpeed", player.AttackSpeed);
        PlayerPrefs.SetFloat("CurrentAttackSpeed", player.CurrentAttackSpeed);

        PlayerPrefs.SetFloat("CDR", player.CDR);
        PlayerPrefs.SetFloat("CurrentCDR", player.CurrentCDR);

        PlayerPrefs.SetFloat("BaseArmor", player.BaseArmor);
        PlayerPrefs.SetFloat("CurrentArmor", player.CurrentArmor);

        PlayerPrefs.Save();
    }

    public void LoadPlayerStats()
    {
        player.PlayerLevel = PlayerPrefs.GetInt("PlayerLevel", 1);
        player.CurrentExperience = PlayerPrefs.GetFloat("CurrentExperience", 0);
        player.RequiredExperience = PlayerPrefs.GetFloat("RequiredExperience", 10);
        player.Coins = PlayerPrefs.GetFloat("Coins", 0);

        player.Health = PlayerPrefs.GetFloat("Health",10);
        player.MaxHealth = PlayerPrefs.GetFloat("MaxHealth", 10);

        player.Speed = PlayerPrefs.GetFloat("Speed", 5);
        player.CurrentSpeed = PlayerPrefs.GetFloat("CurrentSpeed", 5);

        player.Damage = PlayerPrefs.GetInt("Damage", 1);
        player.CurrentDamage = PlayerPrefs.GetInt("CurrentDamage", 1);

        player.AttackSpeed = PlayerPrefs.GetFloat("AttackSpeed", 1);
        player.CurrentAttackSpeed = PlayerPrefs.GetFloat("CurrentAttackSpeed", 1);

        player.CDR = PlayerPrefs.GetFloat("CDR", 1);
        player.CurrentCDR = PlayerPrefs.GetFloat("CurrentCDR", 1);

        player.BaseArmor = PlayerPrefs.GetFloat("BaseArmor", 0);
        player.CurrentArmor = PlayerPrefs.GetFloat("CurrentArmor", 0);
    }

    public override void OnNetworkSpawn()
    {
        net_playerName.OnValueChanged += OnNameChanged;
        net_bodyColor.OnValueChanged += OnBodyColorChanged;
        net_hairColor.OnValueChanged += OnHairColorChanged;

        if (IsOwner)
        {
            InitializeOwnerCharacter();
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

    void InitializeOwnerCharacter()
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
