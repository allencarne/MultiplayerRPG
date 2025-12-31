using Unity.Netcode;
using UnityEngine;

public class PlayerEquipment : NetworkBehaviour
{
    [Header("Player")]
    PlayerCustomization custom;
    PlayerStats stats;
    PlayerSave save;
    public bool IsWeaponEquipped = false;

    private void Awake()
    {
        custom = GetComponent<PlayerCustomization>();
        stats = GetComponent<PlayerStats>();
        save = GetComponent<PlayerSave>();
    }

    public void OnEquipmentChanged(Equipment newItem, Equipment oldItem, bool applyModifiers = true)
    {
        if (newItem != null)
        {
            if (applyModifiers)
            {
                if (oldItem != null)
                {
                    foreach (StatModifier mod in oldItem.modifiers) ApplyModifier(mod, false);
                }
                foreach (StatModifier mod in newItem.modifiers) ApplyModifier(mod, true);
            }

            if (newItem is Weapon newWeapon)
            {
                EquipWeapon(newWeapon);
            }
            else
            {
                if (IsServer)
                {
                    EquipArmor(newItem.equipmentType, newItem.AnimationIndex);
                }
                else
                {
                    EquipArmorServerRPC(newItem.equipmentType, newItem.AnimationIndex);
                }
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
                if (IsServer)
                {
                    UnEquipArmor(oldItem.equipmentType);
                }
                else
                {
                    UnEquipArmorServerRPC(oldItem.equipmentType);
                }
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

    void EquipArmor(EquipmentType type, int index)
    {
        switch (type)
        {
            case EquipmentType.Head: custom.net_HeadIndex.Value = index; break;
            case EquipmentType.Chest: custom.net_ChestIndex.Value = index; break;
            case EquipmentType.Legs: custom.net_LegsIndex.Value = index; break;
        }
    }

    [ServerRpc]
    void EquipArmorServerRPC(EquipmentType type, int index)
    {
        EquipArmor(type, index);
    }

    void UnEquipArmor(EquipmentType type)
    {
        switch (type)
        {
            case EquipmentType.Head: custom.net_HeadIndex.Value = 0; break;
            case EquipmentType.Chest: custom.net_ChestIndex.Value = 0; break;
            case EquipmentType.Legs: custom.net_LegsIndex.Value = 0; break;
        }
    }

    [ServerRpc]
    void UnEquipArmorServerRPC(EquipmentType type)
    {
        UnEquipArmor(type);
    }

    private void ApplyModifier(StatModifier mod, bool apply)
    {
        mod.source = ModSource.Equipment;

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
