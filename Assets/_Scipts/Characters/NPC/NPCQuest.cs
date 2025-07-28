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
    [SerializeField] GameObject rewardListUI;
    [SerializeField] GameObject rewardUI_Item;

    [Header("Objective")]
    [SerializeField] GameObject objectiveListUI;
    [SerializeField] GameObject objectiveUI_Text;

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
        QuestUI.SetActive(false);

        if (playerReference != null)
        {
            PlayerInteract playerInteract = playerReference.GetComponent<PlayerInteract>();
            if (playerInteract != null)
            {
                playerInteract.BackButton();
            }
        }
    }

    void UpdateQuestInfo(Quest quest)
    {
        questTitle.text = quest.questName;
        questInfo.text = quest.instructions;
        questGold.text = quest.goldReward.ToString();
        questEXP.text = quest.expReward.ToString();

        ClearList();
        GetRewards(quest);
        GetObjectives(quest);
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
        foreach (string objective in quest.objectives)
        {
            GameObject objectiveText = Instantiate(objectiveUI_Text, objectiveListUI.transform);

            TextMeshProUGUI text = objectiveText.GetComponent<TextMeshProUGUI>();
            if (text != null)
            {
                text.text = objective;
            }
        }
    }
}
