using System.Collections.Generic;
using UnityEngine;

public class NPCQuest : MonoBehaviour
{
    [SerializeField] NPC npc;
    public List<Quest> quests = new List<Quest>();
    public int QuestIndex = 0;

    public Quest GetAvailableQuest(PlayerQuest playerQuest)
    {
        // Check player's active quests for any that should be turned in to this NPC
        Quest turnIn = playerQuest.GetQuestReadyToTurnInForReceiver(npc.NPC_ID);
        if (turnIn != null) return turnIn;

        // check this NPC's own list for a quest the player can accept.
        if (quests == null || quests.Count == 0) return null;

        Quest candidate = quests[QuestIndex];
        Player player = playerQuest.GetComponent<Player>();

        if (player.PlayerLevel.Value < candidate.LevelRequirment) return null;
        if (!HasMetQuestRequirements(playerQuest, candidate)) return null;

        foreach (QuestProgress progress in playerQuest.activeQuests)
        {
            if (progress.state == QuestState.InProgress)
            {
                foreach (QuestObjective obj in progress.objectives)
                {
                    if (obj.type == ObjectiveType.Talk && obj.ObjectiveID == npc.NPC_ID && !obj.IsCompleted)
                    {
                        // This NPC is the one we need to talk to
                        return progress.quest;
                    }
                }
            }
        }

        QuestProgress existing = playerQuest.activeQuests.Find(q => q.quest == candidate);
        if (existing == null || existing.state == QuestState.Available)
            return candidate;

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

    public void IncreaseQuestIndex(Quest quest)
    {
        if (quests.Contains(quest) && QuestIndex < quests.Count - 1)
        {
            QuestIndex++;
        }
    }
}