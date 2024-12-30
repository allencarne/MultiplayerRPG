using UnityEngine;

[CreateAssetMenu(fileName = "CharacterCustomizationData", menuName = "Character/Customization Data")]
public class CharacterCustomizationData : ScriptableObject
{
    public Color[] skinColors; // Array of skin color names or codes
    public Sprite[] hairStyles; // Array of hair style names
    public Color[] hairColors; // Array of hair color names

    public Weapon[] Swords;
    public Weapon[] Staffs;
}
