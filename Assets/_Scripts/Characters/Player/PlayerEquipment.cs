using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerEquipment : NetworkBehaviour
{
    public NetworkVariable<CurrentWeapon> net_currentWeapon = new(writePerm: NetworkVariableWritePermission.Owner);
    public NetworkVariable<FixedString64Bytes> net_itemName = new("", writePerm: NetworkVariableWritePermission.Owner);

    [Header("WeaponSprites")]
    [SerializeField] SpriteRenderer Sword;
    [SerializeField] SpriteRenderer Staff;
    [SerializeField] SpriteRenderer Bow;
    [SerializeField] SpriteRenderer Dagger;

    [Header("Player")]
    [SerializeField] ItemList itemDatabase;
    PlayerCustomization customization;
    PlayerStats stats;
    PlayerStateMachine stateMachine;
    PlayerSave save;
    public bool IsWeaponEquipped = false;

    public enum CurrentWeapon
	{
		None,
		Sword,
		Staff,
		Bow,
		Dagger
	}

	public CurrentWeapon currentWeapon = CurrentWeapon.None;

    public override void OnNetworkSpawn()
    {
        net_currentWeapon.OnValueChanged += OnWeaponChanged;
        net_itemName.OnValueChanged += OnItemNameChanged;

        if (IsOwner)
        {
            net_currentWeapon.Value = currentWeapon;
            net_itemName.Value = "";
        }
        else
        {
            UpdateWeaponVisuals(net_currentWeapon.Value, net_itemName.Value.ToString());
        }
    }

    public override void OnNetworkDespawn()
    {
        net_currentWeapon.OnValueChanged -= OnWeaponChanged;
        net_itemName.OnValueChanged -= OnItemNameChanged;
    }

    private void Awake()
    {
        customization = GetComponent<PlayerCustomization>();
        stats = GetComponent<PlayerStats>();
        stateMachine = GetComponent<PlayerStateMachine>();
        save = GetComponent<PlayerSave>();
    }

    public void OnEquipmentChanged(Equipment newItem, Equipment oldItem, bool applyModifiers = true)
    {
        if (newItem != null)
        {
            if (applyModifiers)
            {
                foreach (StatModifier mod in newItem.modifiers) ApplyModifier(mod, true);
            }

            save.SaveStats();

            if (newItem is Weapon newWeapon)
            {
                EquipWeapon(newWeapon);
            }
            else
            {
                EquipArmor(newItem);
            }
        }
        else
        {
            if (applyModifiers)
            {
                foreach (StatModifier mod in oldItem.modifiers) ApplyModifier(mod, false);
            }

            save.SaveStats();

            if (oldItem is Weapon oldWeapon)
            {
                UnequipWeapon();
            }
            else
            {
                UnEquipArmor(oldItem);
            }
        }
    }

    private void EquipWeapon(Weapon newWeapon)
    {
        currentWeapon = MapWeaponType(newWeapon.weaponType);
        IsWeaponEquipped = true;

        if (IsOwner)
        {
            string cleanName = newWeapon.name.Replace("(Clone)", "").Trim();
            net_currentWeapon.Value = currentWeapon;
            net_itemName.Value = cleanName;
        }

        UpdateWeaponVisuals(currentWeapon, newWeapon.name);
    }

    private void UnequipWeapon()
    {
        currentWeapon = CurrentWeapon.None;
        IsWeaponEquipped = false;

        if (IsOwner)
        {
            net_currentWeapon.Value = CurrentWeapon.None;
            net_itemName.Value = "";
        }

        UpdateWeaponVisuals(CurrentWeapon.None, "");
    }

    private void UpdateWeaponVisuals(CurrentWeapon weapon, string itemName)
    {
        // disable all first
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

    private Sprite FetchWeaponSprite(string itemName)
    {
        if (string.IsNullOrEmpty(itemName)) return null;

        string cleanName = itemName.Replace("(Clone)", "").Trim();
        Item baseItem = itemDatabase.GetItemByName(cleanName);
        if (baseItem is Weapon weapon) return weapon.weaponSprite;

        return null;
    }

    void EquipArmor(Equipment newEquipment)
    {
        switch (newEquipment.equipmentType)
        {
            case EquipmentType.Head:

                switch (newEquipment.itemIndex)
                {
                    case 1: customization.HeadAnimIndex = newEquipment.itemIndex; break;
                    case 2: customization.HeadAnimIndex = newEquipment.itemIndex; break;
                }
                break;

            case EquipmentType.Chest:

                switch (newEquipment.itemIndex)
                {
                    case 1: customization.ChestAnimIndex = newEquipment.itemIndex; break;
                    case 2: customization.ChestAnimIndex = newEquipment.itemIndex; break;
                }
                break;

            case EquipmentType.Legs:

                switch (newEquipment.itemIndex)
                {
                    case 1: customization.LegsAnimIndex = newEquipment.itemIndex; break;
                    case 2: customization.LegsAnimIndex = newEquipment.itemIndex; break;
                }
                break;
        }

        stateMachine.SetState(PlayerStateMachine.State.Idle);
    }

    private CurrentWeapon MapWeaponType(WeaponType type)
    {
        switch (type)
        {
            case WeaponType.Sword: return CurrentWeapon.Sword;
            case WeaponType.Staff: return CurrentWeapon.Staff;
            case WeaponType.Bow: return CurrentWeapon.Bow;
            case WeaponType.Dagger: return CurrentWeapon.Dagger;
            default: return CurrentWeapon.None;
        }
    }

    void UnEquipArmor(Equipment oldItem)
    {
        if (oldItem == null) return;

        switch (oldItem.equipmentType)
        {
            case EquipmentType.Head:

                switch (oldItem.itemIndex)
                {
                    case 1: customization.HeadAnimIndex = 0; break;
                    case 2: customization.HeadAnimIndex = 0; break;
                }
                break;

            case EquipmentType.Chest:

                switch (oldItem.itemIndex)
                {
                    case 1: customization.ChestAnimIndex = 0; break;
                    case 2: customization.ChestAnimIndex = 0; break;
                }
                break;

            case EquipmentType.Legs:

                switch (oldItem.itemIndex)
                {
                    case 1: customization.LegsAnimIndex = 0; break;
                    case 2: customization.LegsAnimIndex = 0; break;
                }
                break;
        }

        stateMachine.SetState(PlayerStateMachine.State.Idle);
    }

    private void ApplyModifier(StatModifier mod, bool apply)
    {
        if (apply)
        {
            stats.AddModifier(mod);
        }
        else
        {
            stats.RemoveModifier(mod);
        }
    }

    private void OnWeaponChanged(CurrentWeapon previous, CurrentWeapon next)
    {
        UpdateWeaponVisuals(next, net_itemName.Value.ToString());
    }

    private void OnItemNameChanged(FixedString64Bytes previous, FixedString64Bytes next)
    {
        UpdateWeaponVisuals(net_currentWeapon.Value, next.ToString());
    }
}
