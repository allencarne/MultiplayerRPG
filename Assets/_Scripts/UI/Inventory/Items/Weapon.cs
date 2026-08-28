using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Item/Item Type/Weapon")]
public class Weapon : Equipment
{
    public Sprite weaponSprite;
    public WeaponType weaponType;
}

public enum WeaponType
{
    Sword,
    Staff,
    Bow,
    Dagger
}
