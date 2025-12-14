using System;
using TMPro;
using UnityEngine;

public class QusetTracker : MonoBehaviour
{
    [SerializeField] PlayerQuest playerQuest;
    bool isMobile;
    private GameObject activeTracker;

    [Header("UI")]
    [SerializeField] GameObject QuestTracker;
    [SerializeField] GameObject QuestTrackerM;

    [Header("Prefab")]
    [SerializeField] GameObject QuestUIPrefab;
    [SerializeField] GameObject ObjectiveUIPrefab;

    private void OnEnable()
    {
        playerQuest.OnQuestStateChanged.AddListener(UpdateQuestUI);
    }

    private void OnDisable()
    {
        playerQuest.OnQuestStateChanged.RemoveListener(UpdateQuestUI);
    }

    private void Start()
    {
        isMobile = Application.isMobilePlatform;
        activeTracker = isMobile ? QuestTrackerM : QuestTracker;

        UpdateQuestUI();
    }

    void UpdateQuestUI()
    {
        ClearQuestTracker();

        foreach (QuestProgress progress in playerQuest.activeQuests)
        {
            if (progress.state == QuestState.InProgress || progress.state == QuestState.ReadyToTurnIn)
            {
                CreateQuestEntry(progress);
            }
        }
    }

    private void CreateQuestEntry(QuestProgress progress)
    {
        GameObject questUI = Instantiate(QuestUIPrefab, activeTracker.transform);

        SetQuestTitle(questUI, progress.quest.QuestName);

        foreach (QuestObjective obj in progress.objectives)
        {
            CreateObjectiveEntry(questUI.transform, obj);
        }
    }

    private void SetQuestTitle(GameObject questUI, string questName)
    {
        TextMeshProUGUI questTitle = questUI.GetComponentInChildren<TextMeshProUGUI>();
        if (questTitle != null)
        {
            questTitle.text = questName;
        }
    }

    private void CreateObjectiveEntry(Transform parent, QuestObjective obj)
    {
        GameObject objectiveText = Instantiate(ObjectiveUIPrefab, parent);
        TextMeshProUGUI text = objectiveText.GetComponent<TextMeshProUGUI>();

        if (text != null)
        {
            string displayText = $"{obj.Description} ( {obj.CurrentAmount} / {obj.RequiredAmount} )";

            if (obj.IsCompleted)
            {
                if (obj.IsCompleted) displayText = $"<s><color=#000000>{displayText}</color></s>";
            }

            text.text = displayText;
        }
    }

    void ClearQuestTracker()
    {
        foreach (Transform child in activeTracker.transform)
        {
            Destroy(child.gameObject);
        }
    }
}