using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerEquipment : NetworkBehaviour
{
    [SerializeField] ItemList itemList;

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
                UnequipWeapon();
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
        IsWeaponEquipped = true;

        if (IsServer)
        {
            custom.net_EquippedWeaponId.Value = newWeapon.ITEM_ID;
        }
        else
        {
            EquipWeaponServerRPC(newWeapon.ITEM_ID);
        }
    }

    [ServerRpc]
    void EquipWeaponServerRPC(FixedString64Bytes itemId)
    {
        custom.net_EquippedWeaponId.Value = itemId;
    }

    private void UnequipWeapon()
    {
        IsWeaponEquipped = false;

        if (IsServer)
        {
            custom.net_EquippedWeaponId.Value = default;
        }
        else
        {
            UnequipWeaponServerRPC();
        }
    }

    [ServerRpc]
    void UnequipWeaponServerRPC()
    {
        custom.net_EquippedWeaponId.Value = default;
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

    [ServerRpc]
    public void DropItemServerRPC(FixedString64Bytes itemId, int quantity, Vector3 dropPosition)
    {
        Item item = itemList.GetItemById(itemId);

        if (item == null)
        {
            Debug.LogError($"Failed to find item with ID: {itemId}");
            return;
        }

        GameObject dropped = Instantiate(item.Prefab, dropPosition, Quaternion.identity);
        NetworkObject netObj = dropped.GetComponent<NetworkObject>();
        netObj.Spawn();
        dropped.GetComponent<ItemPickup>().Quantity.Value = quantity;
    }
}
