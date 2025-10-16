using System.Collections.Generic;
using UnityEngine;

public class NPCQuest : MonoBehaviour
{
    [Header("Quest")]
    public List<Quest> quests = new List<Quest>();
    public int QuestIndex = 0;

    public Quest GetAvailableQuest(PlayerQuest playerQuest)
    {
        NPC npc = GetComponent<NPC>();

        Quest turnIn = playerQuest.GetQuestReadyToTurnInForReceiver(npc.NPC_ID);
        if (turnIn != null)
            return turnIn;

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

        if (quests == null || quests.Count == 0)
            return null;

        Quest candidate = quests[QuestIndex];
        Player player = playerQuest.GetComponent<Player>();
        if (player.PlayerLevel.Value < candidate.LevelRequirment)
            return null;

        QuestProgress existing = playerQuest.activeQuests.Find(q => q.quest == candidate);
        if (existing == null || existing.state == QuestState.Available)
            return candidate;

        return null;
    }
}