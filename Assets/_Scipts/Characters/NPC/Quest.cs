using UnityEngine;

[CreateAssetMenu(fileName = "Quest", menuName = "Scriptable Objects/Quest")]
public class Quest : ScriptableObject
{
    public string questName;
    [TextArea(3, 8)] public string instructions;
    [TextArea(3, 8)] public string deliver;

    [Header("Rewards")]
    public int expReward;
    public int goldReward;

    public Item[] reward;
}
