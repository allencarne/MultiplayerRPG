using UnityEngine;
using UnityEngine.UI;

public class NPCQuestIcon : MonoBehaviour
{
    [SerializeField] Image questIcon;
    [SerializeField] Sprite[] icons;

    public void UpdateIcon(QuestState? questState)
    {
        if (questState == null)
        {
            questIcon.enabled = false;
            return;
        }

        questIcon.enabled = true;

        switch (questState)
        {
            case QuestState.Unavailable:
                questIcon.sprite = icons[0];
                break;
            case QuestState.Available:
                questIcon.sprite = icons[1];
                break;
            case QuestState.InProgress:
                questIcon.sprite = icons[2];
                break;
            case QuestState.ReadyToTurnIn:
                questIcon.sprite = icons[3];
                break;
            case QuestState.Completed:
                questIcon.enabled = false;
                break;
        }
    }
}
