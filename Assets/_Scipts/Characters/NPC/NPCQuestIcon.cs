using UnityEngine;
using UnityEngine.UI;

public class NPCQuestIcon : MonoBehaviour
{
    [SerializeField] Image questIcon;
    [SerializeField] Sprite[] icons;
    NPCQuestTracker tracker;

    void Awake()
    {
        tracker = GetComponent<NPCQuestTracker>();
    }

    public void UpdateIcon(PlayerQuest playerQuest)
    {
        Quest quest = tracker.GetCurrentQuest();
        if (quest == null)
        {
            questIcon.enabled = false;
            return;
        }

        QuestProgress progress = playerQuest.GetProgress(quest);
        Sprite icon = icons[0];

        switch (progress.currentState)
        {
            case QuestState.Unavailable: icon = icons[0]; break;
            case QuestState.Available: icon = icons[1]; break;
            case QuestState.InProgress: icon = icons[2]; break;
            case QuestState.ReadyToTurnIn: icon = icons[3]; break;
        }

        questIcon.sprite = icon;
        questIcon.enabled = true;
    }
}
