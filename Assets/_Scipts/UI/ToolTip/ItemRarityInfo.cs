using UnityEngine;

[CreateAssetMenu(fileName = "ItemRarityInfo", menuName = "ScriptableObjects/ItemRarityInfo")]
public class ItemRarityInfo : ScriptableObject
{
    public Color CommonColor;
    public Color UnCommonColor;
    public Color RareColor;
    public Color EpicColor;
    public Color ExoticColor;
    public Color MythicColor;
    public Color LegendaryColor;
}
