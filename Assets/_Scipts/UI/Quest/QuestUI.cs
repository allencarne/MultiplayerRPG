using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestUI : MonoBehaviour
{
    [SerializeField] Quest[] allQuests;

    [Header("Quest List")]
    [SerializeField] GameObject questListUI;
    [SerializeField] GameObject questUI_Button;

    [Header("Reward")]
    [SerializeField] GameObject rewardListUI;
    [SerializeField] GameObject rewardUI_Item;

    [Header("Objective")]
    [SerializeField] GameObject objectiveListUI;
    [SerializeField] GameObject objectiveUI_Text;

    [Header("Text")]
    [SerializeField] TextMeshProUGUI questName;
    [SerializeField] TextMeshProUGUI questInfo;
    [SerializeField] TextMeshProUGUI goldReward;
    [SerializeField] TextMeshProUGUI expReward;

    private void Start()
    {
        ShowQuestDetails(allQuests[0]);
        PopulateQuestList();
    }

    public void ShowQuestDetails(Quest quest)
    {
        questName.text = quest.QuestName;
        questInfo.text = quest.Instructions;
        goldReward.text = quest.goldReward.ToString();
        expReward.text = quest.expReward.ToString();

        ClearList();
        GetRewards(quest);
        GetObjectives(quest);
    }

    void PopulateQuestList()
    {
        foreach (Quest quest in allQuests)
        {
            GameObject listItem = Instantiate(questUI_Button, questListUI.transform);

            TextMeshProUGUI listText = listItem.GetComponentInChildren<TextMeshProUGUI>();
            if (listText != null)
            {
                listText.text = quest.QuestName;
            }

            QuestButtonHandler handler = listItem.GetComponent<QuestButtonHandler>();
            if (handler != null)
            {
                handler.Setup(quest, this);
            }
        }
    }

    void ClearList()
    {
        foreach (Transform child in rewardListUI.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in objectiveListUI.transform)
        {
            Destroy(child.gameObject);
        }
    }

    void GetRewards(Quest quest)
    {
        foreach (Item reward in quest.reward)
        {
            GameObject itmeUI = Instantiate(rewardUI_Item, rewardListUI.transform);

            Image image = itmeUI.GetComponent<Image>();
            if (image != null)
            {
                image.sprite = reward.Icon;
            }
        }
    }

    void GetObjectives(Quest quest)
    {
        foreach (QuestObjective objective in quest.Objectives)
        {
            GameObject objectiveText = Instantiate(objectiveUI_Text, objectiveListUI.transform);

            TextMeshProUGUI text = objectiveText.GetComponent<TextMeshProUGUI>();
            if (text != null)
            {
                //text.text = objective.Description;
                text.text = $"{objective.Description} ( {objective.CurrentAmount} / {objective.RequiredAmount} )";
            }
        }
    }
}
