using System.Collections;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerEquipment : NetworkBehaviour
{
    public NetworkVariable<CurrentWeapon> net_currentWeapon = new(writePerm: NetworkVariableWritePermission.Owner);
    public NetworkVariable<FixedString64Bytes> net_itemName = new("", writePerm: NetworkVariableWritePermission.Owner);

    [Header("Player")]
    PlayerCustomization custom;
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

        custom.UpdateWeaponVisuals(net_currentWeapon.Value, net_itemName.Value.ToString());
    }

    public override void OnNetworkDespawn()
    {
        net_currentWeapon.OnValueChanged -= OnWeaponChanged;
        net_itemName.OnValueChanged -= OnItemNameChanged;
    }

    private void Awake()
    {
        custom = GetComponent<PlayerCustomization>();
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
    }

    void EquipArmor(Equipment newEquipment)
    {
        switch (newEquipment.equipmentType)
        {
            case EquipmentType.Head:

                switch (newEquipment.itemIndex)
                {
                    case 1: custom.HeadAnimIndex = newEquipment.itemIndex; break;
                    case 2: custom.HeadAnimIndex = newEquipment.itemIndex; break;
                }
                break;

            case EquipmentType.Chest:

                switch (newEquipment.itemIndex)
                {
                    case 1: custom.ChestAnimIndex = newEquipment.itemIndex; break;
                    case 2: custom.ChestAnimIndex = newEquipment.itemIndex; break;
                }
                break;

            case EquipmentType.Legs:

                switch (newEquipment.itemIndex)
                {
                    case 1: custom.LegsAnimIndex = newEquipment.itemIndex; break;
                    case 2: custom.LegsAnimIndex = newEquipment.itemIndex; break;
                }
                break;
        }

        stateMachine.SetState(PlayerStateMachine.State.Idle);
    }

    void UnEquipArmor(Equipment oldItem)
    {
        if (oldItem == null) return;

        switch (oldItem.equipmentType)
        {
            case EquipmentType.Head:

                switch (oldItem.itemIndex)
                {
                    case 1: custom.HeadAnimIndex = 0; break;
                    case 2: custom.HeadAnimIndex = 0; break;
                }
                break;

            case EquipmentType.Chest:

                switch (oldItem.itemIndex)
                {
                    case 1: custom.ChestAnimIndex = 0; break;
                    case 2: custom.ChestAnimIndex = 0; break;
                }
                break;

            case EquipmentType.Legs:

                switch (oldItem.itemIndex)
                {
                    case 1: custom.LegsAnimIndex = 0; break;
                    case 2: custom.LegsAnimIndex = 0; break;
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

        save.SaveStats();
    }

    private void OnWeaponChanged(CurrentWeapon previous, CurrentWeapon next)
    {
        custom.UpdateWeaponVisuals(next, net_itemName.Value.ToString());
    }

    private void OnItemNameChanged(FixedString64Bytes previous, FixedString64Bytes next)
    {
        custom.UpdateWeaponVisuals(net_currentWeapon.Value, next.ToString());
    }
}
