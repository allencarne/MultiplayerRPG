using UnityEngine;
using UnityEngine.UI;

public class QuestButtonHandler : MonoBehaviour
{
    private Quest questData;
    private QuestUI questUI;

    public void Setup(Quest quest, QuestUI ui)
    {
        questData = quest;
        questUI = ui;

        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        questUI.ShowQuestDetails(questData);
    }
}
