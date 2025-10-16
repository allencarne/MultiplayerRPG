using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest", menuName = "Scriptable Objects/Quest")]
public class Quest : ScriptableObject
{
    public string QuestID;
    public string QuestName;
    public int LevelRequirment;
    [TextArea(3, 8)] public string Instructions;
    [TextArea(3, 8)] public string Deliver;

    [Header("Quest Flow")]
    public string QuestReceiverID;

    [Header("Objectives")]
    public List<QuestObjective> Objectives;

    [Header("Rewards")]
    public int expReward;
    public int goldReward;

    public Item[] Starter;
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