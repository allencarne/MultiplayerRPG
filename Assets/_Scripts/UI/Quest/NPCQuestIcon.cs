using UnityEngine;
using UnityEngine.UI;

public class NPCQuestIcon : MonoBehaviour
{
    [SerializeField] GetPlayerReference getPlayer;
    [SerializeField] NPCQuest npcQuest;
    [SerializeField] Image questIcon;
    [SerializeField] Sprite[] icons;

    public void Initialize()
    {
        if (getPlayer?.player == null) return;

        PlayerQuest playerQuest = getPlayer.player.GetComponent<PlayerQuest>();
        PlayerExperience playerEXP = getPlayer.player.GetComponent<PlayerExperience>();

        playerQuest.OnAccept.AddListener(UpdateSprite);
        playerQuest.OnProgress.AddListener(UpdateSprite);
        playerQuest.OnCompleted.AddListener(UpdateSprite);
        playerEXP.OnLevelUp.AddListener(UpdateSprite);
    }

    private void OnDisable()
    {
        if (getPlayer?.player == null) return;

        PlayerQuest playerQuest = getPlayer.player.GetComponent<PlayerQuest>();
        PlayerExperience playerEXP = getPlayer.player.GetComponent<PlayerExperience>();

        playerQuest.OnAccept.RemoveListener(UpdateSprite);
        playerQuest.OnProgress.RemoveListener(UpdateSprite);
        playerQuest.OnCompleted.RemoveListener(UpdateSprite);
        playerEXP.OnLevelUp.RemoveListener(UpdateSprite);
    }

    public void UpdateSprite()
    {
        if (IsInvalidSetup()) return;

        PlayerQuest playerQuest = getPlayer.player.GetComponent<PlayerQuest>();
        Quest currentQuest = npcQuest.quests[npcQuest.QuestIndex];
        QuestProgress progress = playerQuest.activeQuests.Find(q => q.quest == currentQuest);

        questIcon.enabled = true;

        if (IsQuestLocked(currentQuest)) return;
        if (IsQuestUnlocked(progress)) return;
        SetSpriteForState(progress.state);
    }

    void SetSpriteForState(QuestState state)
    {
        switch (state)
        {
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

    bool IsInvalidSetup()
    {
        return getPlayer?.player == null || npcQuest == null || npcQuest.quests.Count == 0;
    }

    bool IsQuestLocked(Quest quest)
    {
        if (getPlayer.player.PlayerLevel.Value < quest.LevelRequirment)
        {
            questIcon.sprite = icons[(int)QuestState.Unavailable];
            return true;
        }
        return false;
    }

    bool IsQuestUnlocked(QuestProgress progress)
    {
        if (progress == null)
        {
            questIcon.sprite = icons[(int)QuestState.Available];
            return true;
        }
        return false;
    }
}
