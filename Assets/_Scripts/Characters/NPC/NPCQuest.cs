using System.Collections.Generic;
using UnityEngine;

public class NPCQuest : MonoBehaviour
{
    [Header("Quest")]
    public List<Quest> quests = new List<Quest>();
    public int QuestIndex = 0;

    public Quest GetAvailableQuest(PlayerQuest playerQuest)
    {
        // 1) Check player's active quests for any that should be turned in to this NPC.
        var turnIn = playerQuest.GetQuestReadyToTurnInForReceiver(GetComponent<NPC>().NPC_ID);
        if (turnIn != null) return turnIn;

        // 2) Otherwise check this NPC's own list for a quest the player can accept from this NPC.
        if (quests == null || quests.Count == 0) return null;

        // We can return the quest at QuestIndex (or iterate to find first acceptable).
        Quest candidate = quests[QuestIndex];

        // If player is too low level, cannot accept.
        var player = playerQuest.GetComponent<Player>();
        if (player.PlayerLevel.Value < candidate.LevelRequirment) return null;

        // If player does not already have it (or they can re-accept), return it.
        QuestProgress progress = playerQuest.activeQuests.Find(q => q.quest == candidate);
        if (progress == null || progress.state == QuestState.Available)
            return candidate;

        return null;
    }
}