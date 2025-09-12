using Unity.Netcode;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

public class PlayerEquipment : NetworkBehaviour
{
    [Header("Player")]
    Player player;
    PlayerInitialize init;
    [SerializeField] CharacterCustomizationData characterData;

    [Header("Armor")]
    [SerializeField] SpriteRenderer headSprite;
    public int HeadIndex;
    [SerializeField] SpriteRenderer chestSprite;
    public int ChestIndex;
    [SerializeField] SpriteRenderer legsSprite;
    public int LegsIndex;

    [Header("WeaponSprites")]
    [SerializeField] SpriteRenderer Sword;
    [SerializeField] SpriteRenderer Staff;
    [SerializeField] SpriteRenderer Bow;
    [SerializeField] SpriteRenderer Dagger;

    public bool IsWeaponEquipt = false;

    private NetworkVariable<CurrentWeapon> net_currentWeapon = new NetworkVariable<CurrentWeapon>(writePerm: NetworkVariableWritePermission.Owner);
    private NetworkVariable<int> net_itemIndex = new NetworkVariable<int>(-1, writePerm: NetworkVariableWritePermission.Owner);

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
        net_itemIndex.OnValueChanged += OnItemIndexChanged;

        if (IsOwner)
        {
            net_currentWeapon.Value = currentWeapon;
            net_itemIndex.Value = -1;
        }
        else
        {
            UpdateWeaponVisuals(net_currentWeapon.Value, FetchSpriteFromItemIndex(net_currentWeapon.Value, net_itemIndex.Value));
        }
    }

    public override void OnDestroy()
    {
        net_currentWeapon.OnValueChanged -= OnWeaponChanged;
        net_itemIndex.OnValueChanged -= OnItemIndexChanged;
    }

    private void Awake()
    {
        player = GetComponent<Player>();
        init = GetComponent<PlayerInitialize>();
    }

    public void OnEquipmentChanged(Equipment newItem, Equipment oldItem)
	{
		if (newItem != null)
		{
            foreach (StatModifier mod in newItem.modifiers)
            {
                switch (mod.statType)
                {
                    case StatType.Health:
                        player.IncreaseHealth(mod.value);
                        break;
                    case StatType.Damage:
                        player.IncreaseDamage(mod.value);
                        break;
                    case StatType.CoolDown:
                        player.IncreaseCoolDown(mod.value);
                        break;
                    case StatType.AttackSpeed:
                        player.IncreaseAttackSpeed(mod.value);
                        break;
                }

                init.SaveStats();
            }

			Weapon newWeapon = newItem as Weapon;
            if (newWeapon != null)
			{
                IsWeaponEquipt = true;
                EquipWeapon(newWeapon);
            }


            Equipment newEquipment = newItem as Equipment;
            if (newEquipment != null)
            {
                EquipEquipment(newItem);
            }
        }
        else
        {
            foreach (StatModifier mod in oldItem.modifiers)
            {
                switch (mod.statType)
                {
                    case StatType.Health:
                        player.IncreaseHealth(-mod.value);
                        break;
                    case StatType.Damage:
                        player.IncreaseDamage(-mod.value);
                        break;
                    case StatType.CoolDown:
                        player.IncreaseCoolDown(-mod.value);
                        break;
                    case StatType.AttackSpeed:
                        player.IncreaseAttackSpeed(-mod.value);
                        break;
                }

                init.SaveStats();
            }

            Weapon oldWeapon = oldItem as Weapon;
            if (oldWeapon != null)
            {
                IsWeaponEquipt = false;
                UnequipWeapon();
            }

            Equipment oldEquipment = oldItem as Equipment;
            if (oldEquipment != null)
            {
                UnEquipEquipment(oldEquipment);
            }
        }
	}

    private void EquipWeapon(Weapon newWeapon)
    {
        switch (newWeapon.weaponType)
        {
            case WeaponType.Sword:
                currentWeapon = CurrentWeapon.Sword;
                break;
            case WeaponType.Staff:
                currentWeapon = CurrentWeapon.Staff;
                break;
            case WeaponType.Bow:
                currentWeapon = CurrentWeapon.Bow;
                break;
            case WeaponType.Dagger:
                currentWeapon = CurrentWeapon.Dagger;
                break;
        }

        if (IsOwner)
        {
            net_currentWeapon.Value = currentWeapon;
            net_itemIndex.Value = newWeapon.itemIndex;
        }

        UpdateWeaponVisuals(currentWeapon, newWeapon.weaponSprite);
    }

    private void UnequipWeapon()
    {
        currentWeapon = CurrentWeapon.None;

        if (IsOwner)
        {
            net_currentWeapon.Value = currentWeapon;
            net_itemIndex.Value = -1;
        }

        UpdateWeaponVisuals(CurrentWeapon.None, null);
    }

    void EquipEquipment(Equipment newEquipment)
    {
        switch (newEquipment.equipmentType)
        {
            case EquipmentType.Head:

                headSprite.enabled = true;

                switch (newEquipment.itemIndex)
                {
                    case 1: HeadIndex = newEquipment.itemIndex; break;
                    case 2: HeadIndex = newEquipment.itemIndex; break;
                }
                break;

            case EquipmentType.Chest:

                chestSprite.enabled = true;

                switch (newEquipment.itemIndex)
                {
                    case 1: ChestIndex = newEquipment.itemIndex; break;
                    case 2: ChestIndex = newEquipment.itemIndex; break;
                }
                break;

            case EquipmentType.Legs:

                legsSprite.enabled = true;

                switch (newEquipment.itemIndex)
                {
                    case 1: LegsIndex = newEquipment.itemIndex; break;
                    case 2: LegsIndex = newEquipment.itemIndex; break;
                }
                break;
        }
    }

    void UnEquipEquipment(Equipment oldItem)
    {
        if (oldItem == null) return;

        switch (oldItem.equipmentType)
        {
            case EquipmentType.Head:

                headSprite.enabled = false;

                switch (oldItem.itemIndex)
                {
                    case 1: HeadIndex = 0; break;
                    case 2: HeadIndex = 0; break;
                }
                break;

            case EquipmentType.Chest:

                chestSprite.enabled = false;

                switch (oldItem.itemIndex)
                {
                    case 1: ChestIndex = 0; break;
                    case 2: ChestIndex = 0; break;
                }
                break;

            case EquipmentType.Legs:

                legsSprite.enabled = false;

                switch (oldItem.itemIndex)
                {
                    case 1: LegsIndex = 0; break;
                    case 2: LegsIndex = 0; break;
                }
                break;
        }
    }

    void OnWeaponChanged(CurrentWeapon previousWeapon, CurrentWeapon newWeapon)
    {
        UpdateWeaponVisuals(newWeapon, FetchSpriteFromItemIndex(newWeapon, net_itemIndex.Value));
    }

    void OnItemIndexChanged(int previousIndex, int newIndex)
    {
        UpdateWeaponVisuals(net_currentWeapon.Value, FetchSpriteFromItemIndex(net_currentWeapon.Value, newIndex));
    }

    private void UpdateWeaponVisuals(CurrentWeapon weapon, Sprite newSprite)
    {
        // Disable all weapon visuals
        Sword.enabled = false;
        Staff.enabled = false;
        Bow.enabled = false;
        Dagger.enabled = false;

        // Enable the appropriate weapon sprite based on the current weapon
        switch (weapon)
        {
            case CurrentWeapon.Sword:
                Sword.enabled = true;
                Sword.sprite = newSprite;
                break;
            case CurrentWeapon.Staff:
                Staff.enabled = true;
                Staff.sprite = newSprite;
                break;
            case CurrentWeapon.Bow:
                Bow.enabled = true;
                Bow.sprite = newSprite;
                break;
            case CurrentWeapon.Dagger:
                Dagger.enabled = true;
                Dagger.sprite = newSprite;
                break;
        }
    }

    private Sprite FetchSpriteFromItemIndex(CurrentWeapon weapon, int itemIndex)
    {
        if (itemIndex < 0) return null; // No weapon equipped

        switch (weapon)
        {
            case CurrentWeapon.Sword:
                return characterData.Swords[itemIndex].weaponSprite;
            case CurrentWeapon.Staff:
                return characterData.Staffs[itemIndex].weaponSprite;
            case CurrentWeapon.Bow:
                return characterData.Bows[itemIndex].weaponSprite;
            case CurrentWeapon.Dagger:
                return characterData.Daggers[itemIndex].weaponSprite;
            default:
                return null;
        }
    }
}
