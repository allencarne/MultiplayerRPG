using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterCustomizationData", menuName = "Scriptable Objects/Customization Data")]
public class CharacterCustomizationData : ScriptableObject
{
    public Color[] skinColors;
    public Color[] hairColors;
    public Color[] eyeColors;

    public Sprite[] hairStyles;

    public List<SpriteSet> eyes;
    public List<SpriteSet> hairs;
    public List<SpriteSet> helms;
}

[System.Serializable]
public class SpriteSet
{
    public Sprite[] sprites = new Sprite[4];
}