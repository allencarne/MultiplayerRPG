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
        var quest = tracker.GetCurrentQuest();
        if (quest == null)
        {
            questIcon.enabled = false;
            return;
        }

        var progress = playerQuest.GetProgress(quest);
        Sprite icon = icons[0];

        if (progress == null)
        {
            icon = icons[2]; // Available (!)
        }
        else if (progress.currentState == QuestState.InProgress)
        {
            icon = icons[3]; // In progress
        }
        else if (progress.currentState == QuestState.ReadyToTurnIn)
        {
            icon = icons[4]; // Ready to turn in (?)
        }
        else if (progress.currentState == QuestState.Completed)
        {
            icon = icons[5]; // Completed (optional)
        }

        questIcon.sprite = icon;
        questIcon.enabled = true;
    }
}
