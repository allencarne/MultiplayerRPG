using Unity.Netcode;
using UnityEngine;

public class PlayerEquipment : NetworkBehaviour
{
    [SerializeField] CharacterCustomizationData characterData;
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
            UpdateWeaponVisuals(net_currentWeapon.Value, null);
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
                Debug.Log(newWeapon.itemIndex);
            }

            // Handle armor equip
            //EquipArmor(newItem);
        }
        else
        {
            Weapon oldWeapon = oldItem as Weapon;
            if (oldWeapon != null)
            {
                UnequipWeapon();
                Debug.Log(oldItem.itemIndex);
            }

            // Handle armor unequip
            //UnequipArmor(oldItem);
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
            net_currentWeapon.Value = currentWeapon; // Sync weapon state to other clients
        }

        UpdateWeaponVisuals(currentWeapon, newWeapon.weaponSprite);
    }

    private void UnequipWeapon()
    {
        currentWeapon = CurrentWeapon.None;

        if (IsOwner)
        {
            net_currentWeapon.Value = currentWeapon; // Sync weapon state to other clients
        }

        UpdateWeaponVisuals(CurrentWeapon.None, null);
    }

    void OnWeaponChanged(CurrentWeapon previousWeapon, CurrentWeapon newWeapon)
    {
        UpdateWeaponVisuals(newWeapon, null);
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
}
