using UnityEngine;
using UnityEngine.UI;

public class NPCQuestIcon : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GetPlayerReference getPlayer;
    [SerializeField] NPC npc;
    [SerializeField] NPCQuest npcQuest;
    PlayerQuest playerQuest;

    [Header("Sprites")]
    [SerializeField] Image questIcon;
    [SerializeField] SpriteRenderer miniIcon;
    [SerializeField] Sprite[] icons;
    [SerializeField] Sprite[] NPCIcons;

    public void Initialize()
    {
        if (getPlayer?.player == null) return;
        playerQuest = getPlayer.player.GetComponent<PlayerQuest>();
        playerQuest.OnQuestStateChanged.AddListener(UpdateSprite);
    }

    private void OnDisable()
    {
        if (playerQuest != null)
        {
            playerQuest.OnQuestStateChanged.RemoveListener(UpdateSprite);
        }
    }

    public void UpdateSprite()
    {
        if (getPlayer?.player == null) return;

        if (npc.Data.npcClass == NPCClass.Guard)
        {
            miniIcon.sprite = NPCIcons[1];
            return;
        }

        if (npc.Data.npcClass == NPCClass.Patrol)
        {
            miniIcon.sprite = NPCIcons[2];
            return;
        }

        if (npc.Data.npcClass == NPCClass.Vendor)
        {
            miniIcon.sprite = NPCIcons[3];
            return;
        }

        QuestState state = playerQuest.GetQuestStateForNpc(npc, npcQuest);

        switch (state)
        {
            case QuestState.Unavailable:
                questIcon.enabled = true; 
                questIcon.sprite = icons[(int)QuestState.Unavailable];
                miniIcon.sprite = icons[(int)QuestState.Unavailable];
                break;

            case QuestState.Available:
                questIcon.enabled = true; 
                questIcon.sprite = icons[(int)QuestState.Available];
                miniIcon.sprite = icons[(int)QuestState.Available];
                break;

            case QuestState.InProgress:
                questIcon.enabled = true; 
                questIcon.sprite = icons[(int)QuestState.InProgress];
                miniIcon.sprite = icons[(int)QuestState.InProgress];
                break;

            case QuestState.ReadyToTurnIn:
                questIcon.enabled = true; 
                questIcon.sprite = icons[(int)QuestState.ReadyToTurnIn];
                miniIcon.sprite = icons[(int)QuestState.ReadyToTurnIn];
                break;

            case QuestState.Completed:
                questIcon.enabled = false;
                miniIcon.sprite = NPCIcons[0];
                break;

            case QuestState.None:
                questIcon.enabled = false;
                miniIcon.sprite = NPCIcons[0];
                break;
        }
    }
}
