using UnityEngine;
using UnityEngine.UI;

public class NPCQuestIcon : MonoBehaviour
{
    [SerializeField] GetPlayerReference getPlayer;
    [SerializeField] NPCQuest npcQuest;
    [SerializeField] Image questIcon;
    [SerializeField] Sprite[] icons;

    public void InitializeIcon()
    {
        if (getPlayer?.player == null || npcQuest == null || npcQuest.quests.Count == 0)
            return;

        UpdateSprite();
    }

    public void UpdateSprite()
    {
        if (getPlayer?.player == null || npcQuest == null || npcQuest.quests.Count == 0)
            return;

        PlayerQuest playerQuest = getPlayer.player.GetComponent<PlayerQuest>();
        Quest currentQuest = npcQuest.quests[npcQuest.QuestIndex];
        QuestProgress progress = playerQuest.activeQuests.Find(q => q.quest == currentQuest);

        questIcon.enabled = true;

        if (getPlayer.player.PlayerLevel.Value < currentQuest.LevelRequirment)
        {
            questIcon.sprite = icons[(int)QuestState.Unavailable];
            return;
        }

        if (progress == null)
        {
            questIcon.sprite = icons[(int)QuestState.Available];
            return;
        }

        GetState(progress);
    }

    void GetState(QuestProgress progress)
    {
        switch (progress.state)
        {
            case QuestState.Unavailable:
                questIcon.sprite = icons[(int)QuestState.Unavailable];
                break;
            case QuestState.Available:
                questIcon.sprite = icons[(int)QuestState.Available];
                break;
            case QuestState.InProgress:
                questIcon.sprite = icons[(int)QuestState.InProgress];
                break;
            case QuestState.ReadyToTurnIn:
                questIcon.sprite = icons[(int)QuestState.ReadyToTurnIn];
                break;
            case QuestState.Completed:
                questIcon.enabled = false;
                break;
        }
    }
}
