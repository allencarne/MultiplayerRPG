using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestUI : MonoBehaviour
{
    [Header("Quests")]
    [SerializeField] Quest[] allQuests;

    [Header("Player")]
    [SerializeField] PlayerStats stats;
    [SerializeField] PlayerExperience exp;
    [SerializeField] PlayerQuest playerQuest;

    [Header("Quest List")]
    [SerializeField] GameObject questListUI;
    [SerializeField] GameObject questUI_Button;
    [SerializeField] Sprite[] questIcons;

    [Header("Reward")]
    [SerializeField] GameObject rewardListUI;
    [SerializeField] GameObject rewardUI_Item;

    [Header("Objective")]
    [SerializeField] GameObject objectiveListUI;
    [SerializeField] GameObject objectiveUI_Text;

    [Header("Text")]
    [SerializeField] TextMeshProUGUI questName;
    [SerializeField] TextMeshProUGUI questInfo;
    [SerializeField] TextMeshProUGUI questRewardText;

    private void OnEnable()
    {
        exp.OnLevelUp.AddListener(RefreshQuestUI);
        playerQuest.OnQuestStateChanged.AddListener(RefreshQuestUI);
    }

    private void OnDisable()
    {
        exp.OnLevelUp.RemoveListener(RefreshQuestUI);
        playerQuest.OnQuestStateChanged.RemoveListener(RefreshQuestUI);
    }

    private void Start()
    {
        if (allQuests.Length > 0) ShowQuestDetails(allQuests[0]);
        GetQuestList();
    }

    public void ShowQuestDetails(Quest quest)
    {
        SetQuestText(quest);
        ClearAllQuestLists();
        GetRewards(quest);
        GetObjectives(quest);
    }

    void RefreshQuestUI()
    {
        ClearQuestList();
        GetQuestList();
        if (allQuests.Length > 0) ShowQuestDetails(allQuests[0]);
    }

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
        if (listText != null)
            listText.text = quest.QuestName;

        // Find the "Sprite" child (case-sensitive)
        Transform spriteTransform = listItem.transform.Find("Border/Sprite");
        if (spriteTransform != null)
        {
            Image iconImage = spriteTransform.GetComponent<Image>();
            if (iconImage != null)
                SetQuestIcon(iconImage, quest);
        }
        else
        {
            Debug.LogWarning($"QuestUI: Could not find 'Border/Sprite' under {listItem.name}");
        }
    }

    private void SetupQuestButton(GameObject listItem, Quest quest)
    {
        QuestButtonHandler handler = listItem.GetComponent<QuestButtonHandler>();
        if (handler != null)
            handler.Setup(quest, this);
    }

    private void SetQuestIcon(Image iconImage, Quest quest)
    {
        QuestProgress progress = GetProgress(quest);

        if (progress != null)
        {
            switch (progress.state)
            {
                case QuestState.Unavailable:
                    iconImage.sprite = questIcons[0];
                    break;
                case QuestState.Available:
                    iconImage.sprite = questIcons[1];
                    break;
                case QuestState.InProgress:
                    iconImage.sprite = questIcons[2];
                    break;
                case QuestState.ReadyToTurnIn:
                    iconImage.sprite = questIcons[3];
                    break;
                case QuestState.Completed:
                    iconImage.sprite = questIcons[4];
                    break;
            }
        }
        else
        {
            if (stats.PlayerLevel.Value < quest.LevelRequirment)
            {
                iconImage.sprite = questIcons[0];
            }
            else if (!HasCompletedRequiredQuests(quest))
            {
                iconImage.sprite = questIcons[0];
            }
            else
            {
                iconImage.sprite = questIcons[1];
            }
        }
    }

    private bool HasCompletedRequiredQuests(Quest quest)
    {
        if (quest.RequiredQuests == null || quest.RequiredQuests.Count == 0)
            return true;

        foreach (Quest requiredQuest in quest.RequiredQuests)
        {
            QuestProgress progress = playerQuest.activeQuests.Find(q => q.quest == requiredQuest);
            if (progress == null || progress.state != QuestState.Completed)
                return false;
        }
        return true;
    }

    void ClearQuestList()
    {
        foreach (Transform child in questListUI.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void SetQuestText(Quest quest)
    {
        questName.text = quest.QuestName;
        questInfo.text = quest.Instructions;
        questRewardText.text = quest.goldReward.ToString() + " <sprite=0>" + quest.expReward.ToString() + " <sprite name=\"EXP Icon\">";
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
                CreateObjectiveUI(obj);
        }
        else
        {
            foreach (QuestObjective obj in quest.Objectives)
            {
                QuestObjective temp = new QuestObjective
                {
                    Description = obj.Description,
                    RequiredAmount = obj.RequiredAmount,
                    CurrentAmount = 0
                };
                CreateObjectiveUI(temp);
            }
        }
    }

    private void CreateObjectiveUI(QuestObjective obj)
    {
        GameObject objectiveText = Instantiate(objectiveUI_Text, objectiveListUI.transform);
        TextMeshProUGUI text = objectiveText.GetComponent<TextMeshProUGUI>();

        if (text != null)
        {
            string displayText = $"{obj.Description} ( {obj.CurrentAmount} / {obj.RequiredAmount} )";

            if (obj.IsCompleted)
            {
                displayText = $"<s><color=#000000>{displayText}</color></s>";
            }

            text.text = displayText;
        }
    }

    QuestProgress GetProgress(Quest quest)
    {
        return playerQuest.activeQuests.Find(qp => qp.quest == quest);
    }
}
