using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Scriptable Objects/Item/Weapon")]
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
