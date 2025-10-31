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
    }

    private void OnDisable()
    {
        if (playerQuest != null) playerQuest.OnQuestStateChanged.RemoveListener(UpdateSprite);
    }

    public void UpdateSprite()
    {
        if (getPlayer?.player == null) return;
        if (npc.NPC_Name == "Patrol" || npc.NPC_Name == "Guard") return;

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

        Debug.Log(state + " " + npc.NPC_Name);
    }
}
