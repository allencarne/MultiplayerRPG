using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerCustomization : NetworkBehaviour
{
    [Header("Player")]
    [SerializeField] PlayerStateMachine stateMachine;
    [SerializeField] ItemList itemDatabase;
    [SerializeField] PlayerStats stats;

    [Header("UI")]
    public TextMeshProUGUI playerNameText;

    [Header("Sprites")]
    public SpriteRenderer hairSprite;
    public SpriteRenderer bodySprite;

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

    void OnEquipmentChanged(int oldValue, int newValue)
    {
        stateMachine.SetState(PlayerStateMachine.State.Idle);
    }
}
