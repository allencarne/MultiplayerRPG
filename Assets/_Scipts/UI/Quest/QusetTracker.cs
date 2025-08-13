using TMPro;
using UnityEngine;

public class QusetTracker : MonoBehaviour
{
    [SerializeField] PlayerQuest playerQuest;

    [SerializeField] GameObject QuestTracker;
    [SerializeField] GameObject QuestUIPrefab;
    [SerializeField] GameObject ObjectiveUIPrefab;

    private void Start()
    {
        UpdateQuestUI();
    }

    public void UpdateQuestUI()
    {
        foreach (Transform child in QuestTracker.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (QuestProgress progress in playerQuest.activeQuests)
        {
            GameObject questUI = Instantiate(QuestUIPrefab, QuestTracker.transform);

            TextMeshProUGUI questTitle = questUI.GetComponentInChildren<TextMeshProUGUI>();
            if (questTitle != null)
            {
                questTitle.text = progress.quest.QuestName;
            }

            foreach (QuestObjective obj in progress.objectives)
            {
                GameObject objectiveText = Instantiate(ObjectiveUIPrefab, questUI.transform);
                TextMeshProUGUI text = objectiveText.GetComponent<TextMeshProUGUI>();

                if (text != null)
                {
                    text.text = $"{obj.Description} ( {obj.CurrentAmount} / {obj.RequiredAmount} )";
                }
            }
        }
    }
}
