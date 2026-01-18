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
    [SerializeField] Sprite defaulSprite;

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
            case QuestState.Unavailable:
                questIcon.enabled = true; 
                questIcon.sprite = icons[(int)QuestState.Unavailable];
                miniIcon.sprite = icons[(int)QuestState.Unavailable];

                miniIcon.color = Color.white;
                miniIcon.transform.localScale = new Vector3(4, 4, 1);
                break;
            case QuestState.Available:
                questIcon.enabled = true; 
                questIcon.sprite = icons[(int)QuestState.Available];
                miniIcon.sprite = icons[(int)QuestState.Available];

                miniIcon.color = Color.white;
                miniIcon.transform.localScale = new Vector3(4, 4, 1);
                break;
            case QuestState.InProgress:
                questIcon.enabled = true; 
                questIcon.sprite = icons[(int)QuestState.InProgress];
                miniIcon.sprite = icons[(int)QuestState.InProgress];

                miniIcon.color = Color.white;
                miniIcon.transform.localScale = new Vector3(4, 4, 1);
                break;
            case QuestState.ReadyToTurnIn:
                questIcon.enabled = true; 
                questIcon.sprite = icons[(int)QuestState.ReadyToTurnIn];
                miniIcon.sprite = icons[(int)QuestState.ReadyToTurnIn];

                miniIcon.color = Color.white;
                miniIcon.transform.localScale = new Vector3(4, 4, 1);
                break;
            case QuestState.Completed:
                questIcon.enabled = false;
                miniIcon.sprite = defaulSprite;

                miniIcon.color = Color.grey;
                miniIcon.transform.localScale = new Vector3(2, 2, 1);
                break;
            case QuestState.None:
                questIcon.enabled = false;
                miniIcon.sprite = defaulSprite;

                miniIcon.color = Color.grey;
                miniIcon.transform.localScale = new Vector3(2, 2, 1);
                break;
        }
    }
}
