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
        if (allQuests.Length > 0) ShowQuestDetails(allQuests[0]);
        GetQuestList();
    }

    #region UI Methods

    public void ShowQuestDetails(Quest quest)
    {
        SetQuestText(quest);
        ClearAllQuestLists();
        GetRewards(quest);
        GetObjectives(quest);
    }

    public void RefreshQuestUI()
    {
        //@@@ Connected to OnAccept and OnProgress - PlayerQuest Script @@@\\
        ClearQuestList();
        GetQuestList();
        if (allQuests.Length > 0) ShowQuestDetails(allQuests[0]);
    }

    #endregion

    #region Quest List

    private void GetQuestList()
    {
        foreach (Quest quest in allQuests)
        {
            GameObject listItem = Instantiate(questUI_Button, questListUI.transform);
            SetQuestListItemText(listItem, quest);
            SetupQuestButton(listItem, quest);
        }
    }

    private void SetQuestListItemText(GameObject listItem, Quest quest)
    {
        TextMeshProUGUI listText = listItem.GetComponentInChildren<TextMeshProUGUI>();
        if (listText == null) return;

        string state = GetQuestState(quest);
        listText.text = $"{quest.QuestName} - {state}";
    }

    private void SetupQuestButton(GameObject listItem, Quest quest)
    {
        QuestButtonHandler handler = listItem.GetComponent<QuestButtonHandler>();
        if (handler != null)
            handler.Setup(quest, this);
    }

    private string GetQuestState(Quest quest)
    {
        QuestProgress progress = GetProgress(quest);
        if (progress != null)
        {
            if (progress.state == QuestState.Unavailable) return "Unavailable";
            if (progress.state == QuestState.Available) return "Available";
            if (progress.state == QuestState.InProgress) return "In Progress";
            if (progress.state == QuestState.ReadyToTurnIn) return "Ready to Turn In";
            if (progress.state == QuestState.Completed) return "Completed";
        }
        return "Available";
    }

    void ClearQuestList()
    {
        foreach (Transform child in questListUI.transform)
        {
            Destroy(child.gameObject);
        }
    }

    #endregion

    #region Quest Details

    private void SetQuestText(Quest quest)
    {
        questName.text = quest.QuestName;
        questInfo.text = quest.Instructions;
        goldReward.text = quest.goldReward.ToString();
        expReward.text = quest.expReward.ToString();
    }

    void ClearAllQuestLists()
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
        QuestProgress progress = GetProgress(quest);

        if (progress != null)
        {
            foreach (QuestObjective obj in progress.objectives)
                CreateObjectiveUI(obj.Description, obj.CurrentAmount, obj.RequiredAmount);
        }
        else
        {
            foreach (QuestObjective obj in quest.Objectives)
                CreateObjectiveUI(obj.Description, 0, obj.RequiredAmount);
        }
    }

    private void CreateObjectiveUI(string description, int current, int required)
    {
        GameObject objectiveText = Instantiate(objectiveUI_Text, objectiveListUI.transform);
        TextMeshProUGUI text = objectiveText.GetComponent<TextMeshProUGUI>();
        if (text != null) text.text = $"{description} ( {current} / {required} )";
    }

    QuestProgress GetProgress(Quest quest)
    {
        return playerQuest.activeQuests.Find(qp => qp.quest == quest);
    }

    #endregion
}
