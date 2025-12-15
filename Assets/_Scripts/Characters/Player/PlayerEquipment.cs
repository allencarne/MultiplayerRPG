using Unity.Netcode;
using UnityEngine;

public class PlayerEquipment : NetworkBehaviour
{
    [Header("Player")]
    PlayerCustomization custom;
    PlayerStats stats;
    PlayerStateMachine stateMachine;
    PlayerSave save;
    public bool IsWeaponEquipped = false;

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
                UnequipWeapon(oldWeapon);
            }
            else
            {
                UnEquipArmor(oldItem);
            }
        }
    }

    private void EquipWeapon(Weapon newWeapon)
    {
        switch (newWeapon.weaponType)
        {
            case WeaponType.Sword:
                custom.Sword.enabled = true;
                custom.Sword.sprite = newWeapon.weaponSprite;
                break;
            case WeaponType.Staff:
                custom.Staff.enabled = true;
                custom.Staff.sprite = newWeapon.weaponSprite;
                break;
            case WeaponType.Bow:
                custom.Bow.enabled = true;
                custom.Bow.sprite = newWeapon.weaponSprite;
                break;
            case WeaponType.Dagger:
                custom.Dagger.enabled = true;
                custom.Dagger.sprite = newWeapon.weaponSprite;
                break;
        }

        IsWeaponEquipped = true;
    }

    private void UnequipWeapon(Weapon oldWeapon)
    {
        switch (oldWeapon.weaponType)
        {
            case WeaponType.Sword:
                custom.Sword.enabled = false;
                break;
            case WeaponType.Staff:
                custom.Staff.enabled = false;
                break;
            case WeaponType.Bow:
                custom.Bow.enabled = false;
                break;
            case WeaponType.Dagger:
                custom.Dagger.enabled = false;
                break;
        }

        IsWeaponEquipped = false;
    }

    void EquipArmor(Equipment newEquipment)
    {
        switch (newEquipment.equipmentType)
        {
            case EquipmentType.Head:

                switch (newEquipment.AnimationIndex)
                {
                    case 1: custom.net_HeadIndex.Value = newEquipment.AnimationIndex; break;
                    case 2: custom.net_HeadIndex.Value = newEquipment.AnimationIndex; break;
                }
                break;

            case EquipmentType.Chest:

                switch (newEquipment.AnimationIndex)
                {
                    case 1: custom.net_ChestIndex.Value = newEquipment.AnimationIndex; break;
                    case 2: custom.net_ChestIndex.Value = newEquipment.AnimationIndex; break;
                }
                break;

            case EquipmentType.Legs:

                switch (newEquipment.AnimationIndex)
                {
                    case 1: custom.net_LegsIndex.Value = newEquipment.AnimationIndex; break;
                    case 2: custom.net_LegsIndex.Value = newEquipment.AnimationIndex; break;
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

                switch (oldItem.AnimationIndex)
                {
                    case 1: custom.net_HeadIndex.Value = 0; break;
                    case 2: custom.net_HeadIndex.Value = 0; break;
                }
                break;

            case EquipmentType.Chest:

                switch (oldItem.AnimationIndex)
                {
                    case 1: custom.net_ChestIndex.Value = 0; break;
                    case 2: custom.net_ChestIndex.Value = 0; break;
                }
                break;

            case EquipmentType.Legs:

                switch (oldItem.AnimationIndex)
                {
                    case 1: custom.net_LegsIndex.Value = 0; break;
                    case 2: custom.net_LegsIndex.Value = 0; break;
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

        save.SaveStats();
    }
}
