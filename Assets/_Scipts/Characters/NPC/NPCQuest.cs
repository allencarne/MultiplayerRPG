using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NPCQuest : MonoBehaviour
{
    public Quest[] Quests;

    [Header("Text")]
    [SerializeField] TextMeshProUGUI questTitle;
    [SerializeField] TextMeshProUGUI questInfo;
    [SerializeField] TextMeshProUGUI questGold;
    [SerializeField] TextMeshProUGUI questEXP;

    [Header("Reward")]
    [SerializeField] GameObject RewardListUI;
    [SerializeField] GameObject RewardUI_Item;

    [Header("Objective")]
    [SerializeField] GameObject ObjectiveListUI;
    [SerializeField] GameObject ObjectiveUI_Text;

    [Header("Buttons")]
    [SerializeField] Button acceptButton;
    [SerializeField] Button declineButton;

    [SerializeField] GameObject QuestUI;
    Player playerReference;
    int questIndex;

    public void StartQuest(Player player)
    {
        QuestUI.SetActive(true);
        UpdateQuestInfo(Quests[questIndex]);
        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(acceptButton.gameObject);
        if (playerReference == null) playerReference = player;
    }

    public void AcceptButton()
    {

    }

    public void DeclineButton()
    {

    }

    void UpdateQuestInfo(Quest quest)
    {
        questTitle.text = quest.questName;
        questInfo.text = quest.instructions;
        questGold.text = quest.goldReward.ToString();
        questEXP.text = quest.expReward.ToString();

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
