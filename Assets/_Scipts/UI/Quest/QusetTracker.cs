using System;
using TMPro;
using UnityEngine;

public class QusetTracker : MonoBehaviour
{
    [SerializeField] PlayerQuest playerQuest;

    [Header("UI References")]
    [SerializeField] GameObject QuestTracker;
    [SerializeField] GameObject QuestUIPrefab;
    [SerializeField] GameObject ObjectiveUIPrefab;

    private void Start()
    {
        UpdateQuestUI();
    }

    public void UpdateQuestUI()
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
        GameObject questUI = Instantiate(QuestUIPrefab, QuestTracker.transform);

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
        foreach (Transform child in QuestTracker.transform)
        {
            Destroy(child.gameObject);
        }
    }
}