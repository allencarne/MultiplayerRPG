using UnityEngine;

public class NPCQuest : MonoBehaviour
{
    [SerializeField] NPC npc;

    public Quest GetAvailableQuest(PlayerQuest playerQuest)
    {
        if (npc.Data.Quests == null || npc.Data.Quests.Count == 0) return null;

        PlayerStats stats = playerQuest.GetComponent<PlayerStats>();

        foreach (QuestProgress progress in playerQuest.activeQuests)
        {
            Quest quest = progress.quest;

            // If the quest is ReadyToTurnIn, allow turn-in only at the correct NPC:
            if (progress.state == QuestState.ReadyToTurnIn)
            {
                if (quest.HasTalkObjective())
                {
                    // For talk quests the receiver must match this NPC
                    if (quest.GetReceiverID() == npc.Data.NPC_ID) return quest;
                    // Otherwise do not return it here
                    continue;
                }
                else
                {
                    // For non-talk quests, allow turn-in only at the NPC that lists the quest
                    if (npc.Data.Quests.Contains(quest)) return quest;
                    continue;
                }
            }

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
        }

        // No active quests, find the first available quest
        foreach (Quest quest in npc.Data.Quests)
        {
            // Skip if already completed
            QuestProgress progress = playerQuest.activeQuests.Find(q => q.quest == quest);
            if (progress != null && progress.state == QuestState.Completed) continue;

            // Skip if already accepted (in progress)
            if (progress != null) continue;

            // Check level requirement
            if (stats.PlayerLevel.Value < quest.LevelRequirment) continue;

            // Check quest requirements
            if (!HasMetQuestRequirements(playerQuest, quest)) continue;

            // This quest is available
            return quest;
        }

        return null;
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
}