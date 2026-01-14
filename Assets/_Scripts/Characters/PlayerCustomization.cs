using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerCustomization : NetworkBehaviour
{
    [Header("Player")]
    [SerializeField] ItemList itemList;
    [SerializeField] PlayerHead playerHead;
    [SerializeField] PlayerStateMachine stateMachine;
    [SerializeField] PlayerStats stats;

    [Header("UI")]
    public TextMeshProUGUI playerNameText;

    [Header("Head")]
    public SpriteRenderer eyesSprite;
    public SpriteRenderer hairSprite;
    public SpriteRenderer helmSprite;

    [Header("BodySprites")]
    public SpriteRenderer playerHeadSprite;
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
    public NetworkVariable<int> net_EyeIndex = new NetworkVariable<int>(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<Vector2> net_FacingDirection = new NetworkVariable<Vector2>(new Vector2(0, -1), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<FixedString64Bytes> net_EquippedWeaponId = new NetworkVariable<FixedString64Bytes>(writePerm: NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        stats.net_playerName.OnValueChanged += OnNameChanged;
        stats.net_bodyColor.OnValueChanged += OnBodyColorChanged;

        stats.net_hairColor.OnValueChanged += OnHairColorChanged;
        stats.net_eyeColor.OnValueChanged += OnEyeColorChanged;

        net_HeadIndex.OnValueChanged += OnEquipmentChanged;
        net_ChestIndex.OnValueChanged += OnEquipmentChanged;
        net_LegsIndex.OnValueChanged += OnEquipmentChanged;

        net_HairIndex.OnValueChanged += OnHairIndexChanged;
        net_EyeIndex.OnValueChanged += OnEyeIndexChanged;
        net_HeadIndex.OnValueChanged += OnHelmIndexChanged;

        net_FacingDirection.OnValueChanged += OnFacingDirectionChanged;
        net_EquippedWeaponId.OnValueChanged += OnWeaponChanged;

        playerHead.SetHair(net_FacingDirection.Value);
        playerHead.SetEyes(net_FacingDirection.Value);
        playerHead.SetHelm(net_FacingDirection.Value);
        UpdateWeaponVisuals(net_EquippedWeaponId.Value);
    }

    public override void OnNetworkDespawn()
    {
        stats.net_playerName.OnValueChanged -= OnNameChanged;
        stats.net_bodyColor.OnValueChanged -= OnBodyColorChanged;

        stats.net_hairColor.OnValueChanged -= OnHairColorChanged;
        stats.net_eyeColor.OnValueChanged -= OnEyeColorChanged;

        net_HeadIndex.OnValueChanged -= OnEquipmentChanged;
        net_ChestIndex.OnValueChanged -= OnEquipmentChanged;
        net_LegsIndex.OnValueChanged -= OnEquipmentChanged;

        net_HairIndex.OnValueChanged -= OnHairIndexChanged;
        net_EyeIndex.OnValueChanged -= OnEyeIndexChanged;
        net_HeadIndex.OnValueChanged -= OnHelmIndexChanged;

        net_FacingDirection.OnValueChanged -= OnFacingDirectionChanged;
        net_EquippedWeaponId.OnValueChanged -= OnWeaponChanged;
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

    void OnEyeColorChanged(Color previousColor, Color newColor)
    {
        Material eyeMat = eyesSprite.material;
        eyeMat.SetColor("_NewColor", newColor);
    }

    void OnEquipmentChanged(int oldValue, int newValue)
    {
        stateMachine.SetState(PlayerStateMachine.State.Idle);
    }

    void OnHairIndexChanged(int oldValue, int newValue)
    {
        playerHead.SetHair(net_FacingDirection.Value);
    }

    void OnEyeIndexChanged(int oldValue, int newValue)
    {
        playerHead.SetEyes(net_FacingDirection.Value);
    }

    void OnHelmIndexChanged(int oldValue, int newValue)
    {
        playerHead.SetHelm(net_FacingDirection.Value);
    }

    void OnFacingDirectionChanged(Vector2 oldDirection, Vector2 newDirection)
    {
        playerHead.SetHair(newDirection);
        playerHead.SetEyes(newDirection);
        playerHead.SetHelm(newDirection);
    }

    void OnWeaponChanged(FixedString64Bytes oldId, FixedString64Bytes newId)
    {
        UpdateWeaponVisuals(newId);
    }

    public void UpdateWeaponVisuals(FixedString64Bytes weaponId)
    {
        Sword.enabled = false;
        Staff.enabled = false;
        Bow.enabled = false;
        Dagger.enabled = false;

        if (string.IsNullOrEmpty(weaponId.ToString())) return;
        Item baseItem = itemList.GetItemById(weaponId);
        if (baseItem == null || !(baseItem is Weapon weapon)) return;

        switch (weapon.weaponType)
        {
            case WeaponType.Sword:
                Sword.enabled = true;
                Sword.sprite = weapon.weaponSprite;
                break;
            case WeaponType.Staff:
                Staff.enabled = true;
                Staff.sprite = weapon.weaponSprite;
                break;
            case WeaponType.Bow:
                Bow.enabled = true;
                Bow.sprite = weapon.weaponSprite;
                break;
            case WeaponType.Dagger:
                Dagger.enabled = true;
                Dagger.sprite = weapon.weaponSprite;
                break;
        }
    }
}
