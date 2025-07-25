using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NPCQuest : MonoBehaviour
{
    public Quest[] Quests;

    [Header("Text")]
    [SerializeField] TextMeshProUGUI questTitle;
    [SerializeField] TextMeshProUGUI questInfo;
    [SerializeField] TextMeshProUGUI questGold;
    [SerializeField] TextMeshProUGUI questEXP;

    [Header("Buttons")]
    [SerializeField] Button AcceptButton;
    [SerializeField] Button DeclineButton;

    [SerializeField] GameObject QuestUI;
    Player playerReference;
    int questIndex;

    public void StartQuest(Player player)
    {
        QuestUI.SetActive(true);
        UpdateQuestInfo(Quests[questIndex]);
        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(AcceptButton.gameObject);
        if (playerReference == null) playerReference = player;
    }

    void UpdateQuestInfo(Quest quest)
    {
        questTitle.text = quest.questName;
        questInfo.text = quest.instructions;
        questGold.text = quest.goldReward.ToString();
        questEXP.text = quest.expReward.ToString();
    }
}
