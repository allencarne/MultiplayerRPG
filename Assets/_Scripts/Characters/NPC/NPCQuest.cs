using System.Collections.Generic;
using UnityEngine;

public class NPCQuest : MonoBehaviour
{
    [SerializeField] NPC npc;
    public List<Quest> quests = new List<Quest>();
    public int QuestIndex = 0;

    public Quest GetAvailableQuest(PlayerQuest playerQuest)
    {
        if (quests == null || quests.Count == 0) return null;

        Quest candidate = quests[QuestIndex];
        Player player = playerQuest.GetComponent<Player>();

        foreach (QuestProgress progress in playerQuest.activeQuests)
        {
            Quest quest = progress.quest;

            // Turn In Talk Quest 
            if (progress.state == QuestState.InProgress && quest.HasTalkObjective() && quest.GetReceiverID() == npc.NPC_ID) return quest;

            // Skip if this quest doesn't belong to this NPC
            if (!quests.Contains(quest)) continue;

            // Ready To TurnIn
            if (progress.state == QuestState.ReadyToTurnIn) return quest;

            // In-progress Talk Quests
            if (progress.state == QuestState.InProgress)
            {
                foreach (QuestObjective obj in progress.objectives)
                {
                    if (obj.type == ObjectiveType.Talk && obj.ObjectiveID == npc.NPC_ID && !obj.IsCompleted)
                    {
                        return quest;
                    }
                }
            }

            // Available
            if (quest == candidate && progress.state != QuestState.Available) return null;
        }

        // Offer the candidate quest if requirements are met
        if (player.PlayerLevel.Value < candidate.LevelRequirment) return null;
        if (!HasMetQuestRequirements(playerQuest, candidate)) return null;

        return candidate;
    }

    public bool HasMetQuestRequirements(PlayerQuest playerQuest, Quest quest)
    {
        if (quest.RequiredQuests == null || quest.RequiredQuests.Count == 0) return true;

        foreach (Quest requiredQuest in quest.RequiredQuests)
        {
            QuestProgress requiredProgress = playerQuest.activeQuests.Find(q => q.quest == requiredQuest);
            if (requiredProgress == null || requiredProgress.state != QuestState.Completed) return false;
        }
        return true;
    }

    public void IncreaseQuestIndex(Quest quest)
    {
        if (quests.Contains(quest) && QuestIndex < quests.Count - 1)
        {
            QuestIndex++;
        }
    }
}