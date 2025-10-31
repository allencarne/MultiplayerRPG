
using System.Collections.Generic;

[System.Serializable]
public class QuestProgress
{
    public Quest quest;
    public List<QuestObjective> objectives;
    public QuestState state;

    public QuestProgress(Quest quest)
    {
        this.quest = quest;
        objectives = new List<QuestObjective>();

        foreach (QuestObjective obj in quest.Objectives)
        {
            objectives.Add(new QuestObjective
            {
                ObjectiveID = obj.ObjectiveID,
                Description = obj.Description,
                type = obj.type,
                RequiredAmount = obj.RequiredAmount,
                CurrentAmount = 0
            });
        }

        state = QuestState.InProgress;
    }

    public bool IsCompleted => objectives.TrueForAll(x => x.IsCompleted);
    public string QuestID => quest.QuestID;

    public void CheckCompletion()
    {
        bool allCompleted = IsCompleted;

        if (allCompleted && state == QuestState.InProgress)
        {
            state = QuestState.ReadyToTurnIn;
        }
        else if (!allCompleted && state == QuestState.ReadyToTurnIn)
        {
            state = QuestState.InProgress;
        }
    }
}