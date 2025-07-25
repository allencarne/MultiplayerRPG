using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestUI : MonoBehaviour
{
    [SerializeField] Quest[] allQuests;

    [Header("Quest List")]
    [SerializeField] GameObject QuestListUI;
    [SerializeField] GameObject QuestUI_Button;

    [Header("Reward")]
    [SerializeField] GameObject RewardListUI;
    [SerializeField] GameObject RewardUI_Item;

    [Header("Objective")]
    [SerializeField] GameObject ObjectiveListUI;
    [SerializeField] GameObject ObjectiveUI_Text;

    [Header("Text")]
    [SerializeField] TextMeshProUGUI questName;
    [SerializeField] TextMeshProUGUI questInfo;
    [SerializeField] TextMeshProUGUI goldReward;
    [SerializeField] TextMeshProUGUI expReward;

    private void Start()
    {
        ShowQuestDetails(allQuests[0]);

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

        foreach (Transform child in RewardListUI.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in ObjectiveListUI.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Item reward in quest.reward)
        {
            GameObject itmeUI = Instantiate(RewardUI_Item, RewardListUI.transform);

            Image image = itmeUI.GetComponent<Image>();
            if (image != null)
            {
                image.sprite = reward.Icon;
            }
        }

        foreach (string objective in quest.objectives)
        {
            GameObject objectiveText = Instantiate(ObjectiveUI_Text, ObjectiveListUI.transform);

            TextMeshProUGUI text = objectiveText.GetComponent<TextMeshProUGUI>();
            if (text != null)
            {
                text.text = objective;
            }
        }
    }
}
