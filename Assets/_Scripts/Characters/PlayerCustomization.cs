using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerCustomization : NetworkBehaviour
{
    [Header("Player")]
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
    }

    public override void OnDestroy()
    {
        stats.net_playerName.OnValueChanged -= OnNameChanged;
        stats.net_bodyColor.OnValueChanged -= OnBodyColorChanged;
        stats.net_hairColor.OnValueChanged -= OnHairColorChanged;
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
