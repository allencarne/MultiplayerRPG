using System.Collections.Generic;
using UnityEngine;

public class PlayerQuest : MonoBehaviour
{
    public List<QuestProgress> activeQuests = new List<QuestProgress>();

    public void AddQuest(Quest quest)
    {
        if (HasQuest(quest)) return;

        QuestProgress progress = new QuestProgress(quest);
        activeQuests.Add(progress);
    }

    public bool HasQuest(Quest quest)
    {
        return activeQuests.Exists(q => q.quest == quest);
    }

    public QuestProgress GetProgress(Quest quest)
    {
        return activeQuests.Find(q => q.quest == quest);
    }

    public void CompleteObjective(Quest quest, int objectiveIndex)
    {
        var progress = GetProgress(quest);
        if (progress != null)
        {
            progress.CompleteObjective(objectiveIndex);
        }
    }
}
