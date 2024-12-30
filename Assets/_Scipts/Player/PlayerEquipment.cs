using Unity.Netcode;
using UnityEngine;

public class PlayerEquipment : NetworkBehaviour
{
    [SerializeField] SpriteRenderer Sword;
    [SerializeField] SpriteRenderer Staff;
    [SerializeField] SpriteRenderer Bow;
    [SerializeField] SpriteRenderer Dagger;

    private NetworkVariable<CurrentWeapon> net_currentWeapon = new NetworkVariable<CurrentWeapon>(writePerm: NetworkVariableWritePermission.Owner);

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

        if (IsOwner)
        {
            net_currentWeapon.Value = currentWeapon;
        }
        else
        {
            currentWeapon = net_currentWeapon.Value;
        }
    }

    public override void OnDestroy()
    {
        net_currentWeapon.OnValueChanged -= OnWeaponChanged;
    }

    public void OnEquipmentChanged(Equipment newItem, Equipment oldItem)
	{
		if (newItem != null)
		{
			Weapon newWeapon = newItem as Weapon;
            if (newWeapon != null)
			{
                EquipWeapon(newWeapon);
            }

            // Handle armor equip
            //EquipArmor(newItem);
        }
        else
        {
            Weapon oldWeapon = oldItem as Weapon;
            if (oldWeapon != null)
            {
                UnequipWeapon(oldWeapon);
            }

            // Handle armor unequip
            //UnequipArmor(oldItem);
        }
	}

    void EquipWeapon(Weapon newWeapon)
    {
        switch (newWeapon.weaponType)
        {
            case WeaponType.Sword:
                currentWeapon = CurrentWeapon.Sword;
                net_currentWeapon.Value = currentWeapon;

                Sword.enabled = true;
                Sword.sprite = newWeapon.weaponSprite;
                break;
            case WeaponType.Staff:
                currentWeapon = CurrentWeapon.Staff;
                net_currentWeapon.Value = currentWeapon;

                Staff.enabled = true;
                Staff.sprite = newWeapon.weaponSprite;
                break;
            case WeaponType.Bow:
                currentWeapon = CurrentWeapon.Bow;
                net_currentWeapon.Value = currentWeapon;

                Bow.enabled = true;
                Bow.sprite = newWeapon.weaponSprite;
                break;
            case WeaponType.Dagger:
                currentWeapon = CurrentWeapon.Dagger;
                net_currentWeapon.Value = currentWeapon;

                Dagger.enabled = true;
                Dagger.sprite = newWeapon.weaponSprite;
                break;
        }
    }

    void UnequipWeapon(Weapon oldWeapon)
    {
        switch (oldWeapon.weaponType)
        {
            case WeaponType.Sword:
                currentWeapon = CurrentWeapon.None;
                net_currentWeapon.Value = currentWeapon;

                Sword.enabled = false;
                break;
            case WeaponType.Staff:
                currentWeapon = CurrentWeapon.None;
                net_currentWeapon.Value = currentWeapon;

                Staff.enabled = false;
                break;
            case WeaponType.Bow:
                currentWeapon = CurrentWeapon.None;
                net_currentWeapon.Value = currentWeapon;

                Bow.enabled = false;
                break;
            case WeaponType.Dagger:
                currentWeapon = CurrentWeapon.None;
                net_currentWeapon.Value = currentWeapon;

                Dagger.enabled = false;
                break;
        }
    }

    void OnWeaponChanged(CurrentWeapon previousWeapon, CurrentWeapon newWeapon)
    {
        currentWeapon = newWeapon;
    }
}
