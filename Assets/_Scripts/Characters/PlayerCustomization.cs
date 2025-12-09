using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerCustomization : NetworkBehaviour
{
    [Header("Player")]
    [SerializeField] PlayerStats stats;

    [Header("UI")]
    public TextMeshProUGUI playerNameText;

    [Header("Sprites")]
    public SpriteRenderer bodySprite;
    public SpriteRenderer hairSprite;

    [Header("Index")]
    public int HairIndex;
    public int HeadAnimIndex;
    public int ChestAnimIndex;
    public int LegsAnimIndex;

    public override void OnNetworkSpawn()
    {
        stats.net_playerName.OnValueChanged += OnNameChanged;
        stats.net_bodyColor.OnValueChanged += OnBodyColorChanged;
        stats.net_hairColor.OnValueChanged += OnHairColorChanged;

        playerNameText.text = stats.net_playerName.Value.ToString();
        bodySprite.color = stats.net_bodyColor.Value;
        hairSprite.color = stats.net_hairColor.Value;
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
