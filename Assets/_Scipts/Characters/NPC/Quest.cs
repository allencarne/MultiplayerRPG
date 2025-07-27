using UnityEngine;

[CreateAssetMenu(fileName = "Quest", menuName = "Scriptable Objects/Quest")]
public class Quest : ScriptableObject
{
    public string questName;
    [TextArea(3, 8)] public string instructions;
    [TextArea(3, 8)] public string deliver;

    [Header("Objectives")]
    public string[] objectives;

    [Header("Rewards")]
    public int expReward;
    public int goldReward;

    public Item[] reward;
}

public enum QuestState 
{
    Unavailable,
    Available,
    InProgress,
    ReadyToTurnIn,
    Completed,
}