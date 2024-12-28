using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    [SerializeField] SpriteRenderer Sword;
    [SerializeField] SpriteRenderer Staff;
    [SerializeField] SpriteRenderer Bow;
    [SerializeField] SpriteRenderer Dagger;

    public enum CurrentWeapon
	{
		None,
		Sword,
		Staff,
		Bow,
		Dagger
	}

	public CurrentWeapon currentWeapon = CurrentWeapon.None;

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
                Sword.enabled = true;
                Sword.sprite = newWeapon.weaponSprite;
                break;
            case WeaponType.Staff:
                Staff.enabled = true;
                Staff.sprite = newWeapon.weaponSprite;
                break;
            case WeaponType.Bow:
                Bow.enabled = true;
                Bow.sprite = newWeapon.weaponSprite;
                break;
            case WeaponType.Dagger:
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
                Sword.enabled = false;
                break;
            case WeaponType.Staff:
                Staff.enabled = false;
                break;
            case WeaponType.Bow:
                Bow.enabled = false;
                break;
            case WeaponType.Dagger:
                Dagger.enabled = false;
                break;
        }
    }
}
