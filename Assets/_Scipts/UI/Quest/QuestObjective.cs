
[System.Serializable]
public class QuestObjective
{
    public ObjectiveType type;
    public string ObjectiveID;
    public string Description;
    public int RequiredAmount;
    public int CurrentAmount;
    public bool IsCompleted => CurrentAmount >= RequiredAmount;
}

public enum ObjectiveType
{
    Kill,
    Collect,
    Talk,
    Equip,
    Hit
}