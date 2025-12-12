using UnityEngine;

public class NPCQuest : MonoBehaviour
{
    [SerializeField] NPC npc;
    [HideInInspector] public int QuestIndex = 0;

    public Quest GetAvailableQuest(PlayerQuest playerQuest)
    {
        if (npc.Data.Quests == null || npc.Data.Quests.Count == 0) return null;

        Quest candidate = npc.Data.Quests[QuestIndex];
        PlayerStats stats = playerQuest.GetComponent<PlayerStats>();

        foreach (QuestProgress progress in playerQuest.activeQuests)
        {
            Quest quest = progress.quest;

            // Turn In Talk Quest 
            if (progress.state == QuestState.InProgress && quest.HasTalkObjective() && quest.GetReceiverID() == npc.Data.NPC_ID) return quest;

            // Skip if this quest doesn't belong to this NPC
            if (!npc.Data.Quests.Contains(quest)) continue;

            // Ready To TurnIn
            if (progress.state == QuestState.ReadyToTurnIn) return quest;

            // In-progress Talk Quests
            if (progress.state == QuestState.InProgress)
            {
                foreach (QuestObjective obj in progress.objectives)
                {
                    if (obj.type == ObjectiveType.Talk && obj.ObjectiveID == npc.Data.NPC_ID && !obj.IsCompleted)
                    {
                        return quest;
                    }
                }
            }

            // Available
            if (quest == candidate && progress.state != QuestState.Available) return null;
        }

        // Offer the candidate quest if requirements are met
        if (stats.PlayerLevel.Value < candidate.LevelRequirment) return null;
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
        if (npc.Data.Quests.Contains(quest) && QuestIndex < npc.Data.Quests.Count - 1)
        {
            QuestIndex++;
        }
    }
}