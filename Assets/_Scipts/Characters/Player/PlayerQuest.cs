using System.Collections.Generic;
using UnityEngine;

public class PlayerQuest : MonoBehaviour
{
    public List<QuestProgress> activeQuests = new List<QuestProgress>();

    public void AcceptQuest(Quest quest)
    {
        // Check if the quest is already active
        if (activeQuests.Exists(q => q.quest == quest))
        {
            Debug.LogWarning("Quest is already active!");
            return;
        }

        // Add a new QuestProgress for the accepted quest
        QuestProgress progress = new QuestProgress(quest);
        activeQuests.Add(progress);
        Debug.Log($"Quest '{quest.name}' accepted!");

        // Update QuestUI in Journal
        // Update QuestUI in HUD
    }

    public void UpdateObjective(ObjectiveType type, string id, int amount = 1)
    {
        foreach (QuestProgress progress in activeQuests)
        {
            if (progress.state != QuestState.InProgress) continue;

            foreach (QuestObjective obj in progress.objectives)
            {
                if (obj.type == type && obj.ObjectiveID == id && !obj.IsCompleted)
                {
                    obj.CurrentAmount += amount;
                    if (obj.CurrentAmount > obj.RequiredAmount)
                        obj.CurrentAmount = obj.RequiredAmount;
                }
            }

            progress.CheckCompletion();
        }
    }
}
