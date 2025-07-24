using TMPro;
using UnityEngine;

public class QuestUI : MonoBehaviour
{
    [SerializeField] GameObject QuestListUI;
    [SerializeField] Quest[] allQuests;

    [SerializeField] GameObject QuestUI_Button;

    [SerializeField] TextMeshProUGUI questName;
    [SerializeField] TextMeshProUGUI questInfo;
    [SerializeField] TextMeshProUGUI goldReward;
    [SerializeField] TextMeshProUGUI expReward;

    private void Start()
    {
        foreach (Quest quest in allQuests)
        {
            GameObject listItem = Instantiate(QuestUI_Button, QuestListUI.transform);

            TextMeshProUGUI listText = listItem.GetComponentInChildren<TextMeshProUGUI>();
            if (listText != null)
            {
                listText.text = quest.questName;
            }

            QuestButtonHandler handler = listItem.GetComponent<QuestButtonHandler>();
            if (handler != null)
            {
                handler.Setup(quest, this);
            }
        }
    }

    public void ShowQuestDetails(Quest quest)
    {
        questName.text = quest.questName;
        questInfo.text = quest.instructions;
        goldReward.text = quest.goldReward.ToString();
        expReward.text = quest.expReward.ToString();
    }
}
