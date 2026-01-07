using UnityEngine;

[CreateAssetMenu(fileName = "CharacterCustomizationData", menuName = "Scriptable Objects/Customization Data")]
public class CharacterCustomizationData : ScriptableObject
{
    public Color[] skinColors;
    public Color[] hairColors;
    public Color[] eyeColors;

    public Sprite[] hairStyles;

    public Sprite[] Eye0;
    public Sprite[] Eye1;
    public Sprite[] Eye2;
    public Sprite[] Eye3;

    public Sprite[] Hair0;
    public Sprite[] Hair1;
    public Sprite[] Hair2;
    public Sprite[] Hair3;

    public Sprite[] Helm0;
    public Sprite[] Helm1;
    public Sprite[] Helm2;
    public Sprite[] Helm3;
}