using System.Collections.Generic;
using UnityEngine;

public class NPCQuest : MonoBehaviour
{
    [Header("Quest")]
    public List<Quest> quests = new List<Quest>();
    public int QuestIndex = 0;

    public Quest GetAvailableQuest(PlayerQuest playerQuest)
    {
        if (quests.Count == 0) return null;

        Quest currentQuest = quests[QuestIndex];
        QuestProgress progress = playerQuest.activeQuests.Find(q => q.quest == currentQuest);

        // Player too low level
        if (progress == null && playerQuest.GetComponent<Player>().PlayerLevel.Value < currentQuest.LevelRequirment)
            return null;

        // Player can accept or turn in
        if (progress == null || progress.state == QuestState.Available || progress.state == QuestState.ReadyToTurnIn)
            return currentQuest;

        return null;
    }
}