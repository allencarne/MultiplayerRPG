using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using static PlayerEquipment;

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
    public int HairIndex;
    public int HeadAnimIndex;
    public int ChestAnimIndex;
    public int LegsAnimIndex;

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

    public Sprite FetchWeaponSprite(string itemName)
    {
        if (string.IsNullOrEmpty(itemName)) return null;

        string cleanName = itemName.Replace("(Clone)", "").Trim();
        Item baseItem = itemDatabase.GetItemByName(cleanName);
        if (baseItem is Weapon weapon) return weapon.weaponSprite;

        return null;
    }

    public void UpdateWeaponVisuals(CurrentWeapon weapon, string itemName)
    {
        Sword.enabled = Staff.enabled = Bow.enabled = Dagger.enabled = false;
        Sprite sprite = FetchWeaponSprite(itemName);

        switch (weapon)
        {
            case CurrentWeapon.Sword:
                Sword.enabled = true;
                Sword.sprite = sprite;
                break;
            case CurrentWeapon.Staff:
                Staff.enabled = true;
                Staff.sprite = sprite;
                break;
            case CurrentWeapon.Bow:
                Bow.enabled = true;
                Bow.sprite = sprite;
                break;
            case CurrentWeapon.Dagger:
                Dagger.enabled = true;
                Dagger.sprite = sprite;
                break;
        }
    }
}
