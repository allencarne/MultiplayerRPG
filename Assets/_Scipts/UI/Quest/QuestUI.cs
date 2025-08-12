using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestUI : MonoBehaviour
{
    [SerializeField] Quest[] allQuests;
    [SerializeField] PlayerQuest playerQuest;

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
                string state = "Unavailable";

                var progress = GetProgressForQuest(quest);
                if (progress != null)
                {
                    if (progress.state == QuestState.InProgress) state = "In Progress";
                    else if (progress.state == QuestState.Completed) state = "Completed";
                }
                else
                {
                    // Optional: check availability logic here
                    state = "Available";
                }

                listText.text = $"{quest.QuestName} - {state}";
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
        QuestProgress progress = GetProgressForQuest(quest);

        if (progress != null)
        {
            // Show live progress
            foreach (QuestObjective obj in progress.objectives)
            {
                GameObject objectiveText = Instantiate(objectiveUI_Text, objectiveListUI.transform);
                TextMeshProUGUI text = objectiveText.GetComponent<TextMeshProUGUI>();

                if (text != null)
                {
                    text.text = $"{obj.Description} ( {obj.CurrentAmount} / {obj.RequiredAmount} )";
                }
            }
        }
        else
        {
            // Show default objectives (player hasn't started yet)
            foreach (QuestObjective obj in quest.Objectives)
            {
                GameObject objectiveText = Instantiate(objectiveUI_Text, objectiveListUI.transform);
                TextMeshProUGUI text = objectiveText.GetComponent<TextMeshProUGUI>();

                if (text != null)
                {
                    text.text = $"{obj.Description} ( 0 / {obj.RequiredAmount} )";
                }
            }
        }
    }

    QuestProgress GetProgressForQuest(Quest quest)
    {
        return playerQuest.activeQuests.Find(qp => qp.quest == quest);
    }

    public void RefreshQuestUI()
    {
        // Clear the quest list UI
        foreach (Transform child in questListUI.transform)
        {
            Destroy(child.gameObject);
        }

        // Re-populate with the latest quest states
        PopulateQuestList();

        // Optional: refresh details if a quest is already selected
        // This ensures objectives update in the detail panel
        if (allQuests.Length > 0)
        {
            ShowQuestDetails(allQuests[0]);
            // You can store the "currently selected" quest instead of defaulting to index 0
        }
    }
}
