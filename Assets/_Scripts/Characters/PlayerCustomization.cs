using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerCustomization : NetworkBehaviour
{
    [Header("Player")]
    [SerializeField] PlayerStateMachine stateMachine;
    [SerializeField] PlayerStats stats;

    [Header("UI")]
    public TextMeshProUGUI playerNameText;

    [Header("Sprites")]
    public SpriteRenderer eyesSprite;
    public SpriteRenderer playerHair; // Remove later

    public SpriteRenderer hairSprite;
    public SpriteRenderer bodySprite;
    public SpriteRenderer playerHeadSprite;

    [Header("Weapons")]
    public SpriteRenderer Sword;
    public SpriteRenderer Staff;
    public SpriteRenderer Bow;
    public SpriteRenderer Dagger;

    [Header("Index")]
    public NetworkVariable<int> net_HairIndex = new NetworkVariable<int>(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<int> net_HeadIndex = new NetworkVariable<int>(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<int> net_ChestIndex = new NetworkVariable<int>(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<int> net_LegsIndex = new NetworkVariable<int>(writePerm: NetworkVariableWritePermission.Server);

    public NetworkVariable<int> net_EyeIndex = new NetworkVariable<int>(writePerm: NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        stats.net_playerName.OnValueChanged += OnNameChanged;
        stats.net_bodyColor.OnValueChanged += OnBodyColorChanged;
        stats.net_hairColor.OnValueChanged += OnHairColorChanged;

        net_HeadIndex.OnValueChanged += OnEquipmentChanged;
        net_ChestIndex.OnValueChanged += OnEquipmentChanged;
        net_LegsIndex.OnValueChanged += OnEquipmentChanged;
    }

    public override void OnDestroy()
    {
        stats.net_playerName.OnValueChanged -= OnNameChanged;
        stats.net_bodyColor.OnValueChanged -= OnBodyColorChanged;
        stats.net_hairColor.OnValueChanged -= OnHairColorChanged;

        net_HeadIndex.OnValueChanged -= OnEquipmentChanged;
        net_ChestIndex.OnValueChanged -= OnEquipmentChanged;
        net_LegsIndex.OnValueChanged -= OnEquipmentChanged;
    }

    void OnNameChanged(FixedString32Bytes oldName, FixedString32Bytes newName)
    {
        switch (stats.playerClass)
        {
            case PlayerStats.PlayerClass.Beginner:
                playerNameText.text = $"<sprite name=\"Beginner_Icon\"> {newName}";
                break;
            case PlayerStats.PlayerClass.Warrior:
                playerNameText.text = $"<sprite name=\"Warrior_Icon\"> {newName}";
                break;
            case PlayerStats.PlayerClass.Magician:
                playerNameText.text = $"<sprite name=\"Magician_Icon\"> {newName}";
                break;
            case PlayerStats.PlayerClass.Archer:
                playerNameText.text = $"<sprite name=\"Archer_Icon\"> {newName}";
                break;
            case PlayerStats.PlayerClass.Rogue:
                playerNameText.text = $"<sprite name=\"Rogue_Icon\"> {newName}";
                break;
        }
    }

    void OnBodyColorChanged(Color previousColor, Color newColor)
    {
        bodySprite.color = newColor;
        playerHeadSprite.color = newColor;
    }

    void OnHairColorChanged(Color previousColor, Color newColor)
    {
        hairSprite.color = newColor;
    }

    void OnEquipmentChanged(int oldValue, int newValue)
    {
        stateMachine.SetState(PlayerStateMachine.State.Idle);
    }
}
