using UnityEngine;

public class NPCQuestTracker : MonoBehaviour
{
    public Quest[] quests;
    public int currentQuestIndex = 0;
    public bool IsQuestActive;

    public Quest GetCurrentQuest() => quests.Length > currentQuestIndex ? quests[currentQuestIndex] : null;

    public void AdvanceQuest()
    {
        currentQuestIndex++;
    }
}
