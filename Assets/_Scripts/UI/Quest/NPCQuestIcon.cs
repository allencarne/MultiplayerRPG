using UnityEngine;
using UnityEngine.UI;

public class NPCQuestIcon : MonoBehaviour
{
    [SerializeField] GetPlayerReference getPlayer;
    [SerializeField] NPC npc;
    [SerializeField] NPCQuest npcQuest;
    [SerializeField] Image questIcon;
    [SerializeField] Sprite[] icons;
    PlayerQuest playerQuest;

    public void Initialize()
    {
        if (getPlayer?.player == null) return;
        playerQuest = getPlayer.player.GetComponent<PlayerQuest>();
        playerQuest.OnQuestStateChanged.AddListener(UpdateSprite);
        playerQuest.OnQuestTurnedIn.AddListener(npcQuest.IncreaseQuestIndex);
    }

    private void OnDisable()
    {
        if (playerQuest != null)
        {
            playerQuest.OnQuestStateChanged.RemoveListener(UpdateSprite);
            playerQuest.OnQuestTurnedIn.RemoveListener(npcQuest.IncreaseQuestIndex);
        }
    }

    public void UpdateSprite()
    {
        if (getPlayer?.player == null) return;
        if (npc.Data.NPCName == "Patrol" || npc.Data.NPCName == "Guard") return;

        QuestState state = playerQuest.GetQuestStateForNpc(npc, npcQuest);

        switch (state)
        {
            case QuestState.Unavailable: questIcon.enabled = true; questIcon.sprite = icons[(int)QuestState.Unavailable]; break;
            case QuestState.Available: questIcon.enabled = true; questIcon.sprite = icons[(int)QuestState.Available]; break;
            case QuestState.InProgress: questIcon.enabled = true; questIcon.sprite = icons[(int)QuestState.InProgress]; break;
            case QuestState.ReadyToTurnIn: questIcon.enabled = true; questIcon.sprite = icons[(int)QuestState.ReadyToTurnIn]; break;
            case QuestState.Completed: questIcon.enabled = false; break;
            case QuestState.None: questIcon.enabled = false; break;
        }
    }
}
