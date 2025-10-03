using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerEquipment : NetworkBehaviour
{
    [Header("Player")]
    Player player;
    PlayerInitialize init;
    [SerializeField] ItemList itemDatabase;

    [Header("Armor")]
    [SerializeField] SpriteRenderer headSprite;
    public int HeadAnimIndex;
    [SerializeField] SpriteRenderer chestSprite;
    public int ChestAnimIndex;
    [SerializeField] SpriteRenderer legsSprite;
    public int LegsAnimIndex;

    [Header("WeaponSprites")]
    [SerializeField] SpriteRenderer Sword;
    [SerializeField] SpriteRenderer Staff;
    [SerializeField] SpriteRenderer Bow;
    [SerializeField] SpriteRenderer Dagger;

    public bool IsWeaponEquipped = false;

    private NetworkVariable<CurrentWeapon> net_currentWeapon =
        new(writePerm: NetworkVariableWritePermission.Owner);

    private NetworkVariable<FixedString64Bytes> net_itemName =
        new("", writePerm: NetworkVariableWritePermission.Owner);

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

    public override void OnDestroy()
    {
        net_currentWeapon.OnValueChanged -= OnWeaponChanged;
        net_itemName.OnValueChanged -= OnItemNameChanged;
    }

    private void Awake()
    {
        player = GetComponent<Player>();
        init = GetComponent<PlayerInitialize>();
    }

    public void OnEquipmentChanged(Equipment newItem, Equipment oldItem, bool applyModifiers = true)
    {
        if (newItem != null)
        {
            if (applyModifiers)
            {
                foreach (StatModifier mod in newItem.modifiers) ApplyModifier(mod, true);
            }

            init.SaveStats();

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

            init.SaveStats();

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

    private void OnWeaponChanged(CurrentWeapon previous, CurrentWeapon next)
    {
        UpdateWeaponVisuals(next, net_itemName.Value.ToString());
    }

    private void OnItemNameChanged(FixedString64Bytes previous, FixedString64Bytes next)
    {
        UpdateWeaponVisuals(net_currentWeapon.Value, next.ToString());
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

                headSprite.enabled = true;

                switch (newEquipment.itemIndex)
                {
                    case 1: HeadAnimIndex = newEquipment.itemIndex; break;
                    case 2: HeadAnimIndex = newEquipment.itemIndex; break;
                }
                break;

            case EquipmentType.Chest:

                chestSprite.enabled = true;

                switch (newEquipment.itemIndex)
                {
                    case 1: ChestAnimIndex = newEquipment.itemIndex; break;
                    case 2: ChestAnimIndex = newEquipment.itemIndex; break;
                }
                break;

            case EquipmentType.Legs:

                legsSprite.enabled = true;

                switch (newEquipment.itemIndex)
                {
                    case 1: LegsAnimIndex = newEquipment.itemIndex; break;
                    case 2: LegsAnimIndex = newEquipment.itemIndex; break;
                }
                break;
        }
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

                headSprite.enabled = false;

                switch (oldItem.itemIndex)
                {
                    case 1: HeadAnimIndex = 0; break;
                    case 2: HeadAnimIndex = 0; break;
                }
                break;

            case EquipmentType.Chest:

                chestSprite.enabled = false;

                switch (oldItem.itemIndex)
                {
                    case 1: ChestAnimIndex = 0; break;
                    case 2: ChestAnimIndex = 0; break;
                }
                break;

            case EquipmentType.Legs:

                legsSprite.enabled = false;

                switch (oldItem.itemIndex)
                {
                    case 1: LegsAnimIndex = 0; break;
                    case 2: LegsAnimIndex = 0; break;
                }
                break;
        }
    }

    private void ApplyModifier(StatModifier mod, bool apply)
    {
        Debug.Log("ApplyModifier");

        int value = apply ? mod.value : -mod.value;

        switch (mod.statType)
        {
            case StatType.Health:
                player.IncreaseHealth(value);
                break;
            case StatType.Damage:
                player.IncreaseDamage(value);
                break;
            case StatType.CoolDown:
                player.IncreaseCoolDown(value);
                break;
            case StatType.AttackSpeed:
                player.IncreaseAttackSpeed(value);
                break;
        }
    }
}
