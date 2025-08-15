using UnityEngine;
using UnityEngine.UI;

public class NPCQuestIcon : MonoBehaviour
{
    [SerializeField] GetPlayerReference getPlayer;
    [SerializeField] Image questIcon;
    [SerializeField] Sprite[] icons;

    public void InitializeIcon()
    {
        if (getPlayer == null || getPlayer.player == null)
            return;

        NPCQuest npcQuest = GetComponent<NPCQuest>();
        if (npcQuest != null && npcQuest.quests.Count > 0)
        {
            UpdateSprite(npcQuest.quests[0]); // or current quest in NPC
        }
    }

    public void UpdateSprite(Quest quest)
    {
        PlayerQuest playerQuest = getPlayer.player.GetComponent<PlayerQuest>();
        if (playerQuest == null || playerQuest.activeQuests == null)
            return;

        QuestProgress progress = playerQuest.activeQuests.Find(q => q.quest == quest);
        if (progress == null)
        {
            questIcon.enabled = true;
            questIcon.sprite = icons[0];
            return;
        }

        questIcon.enabled = true;

        switch (progress.state)
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
