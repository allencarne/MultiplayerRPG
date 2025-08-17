using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPCQuest : MonoBehaviour
{
    [Header("Quest")]
    public List<Quest> quests = new List<Quest>();
    public int QuestIndex = 0;

    [Header("Text")]
    [SerializeField] TextMeshProUGUI questTitle;
    [SerializeField] TextMeshProUGUI questInfo;
    [SerializeField] TextMeshProUGUI questRewardText;

    [Header("Reward")]
    [SerializeField] GameObject rewardListUI;
    [SerializeField] GameObject rewardUI_Item;

    [Header("Objective")]
    [SerializeField] GameObject objectiveListUI;
    [SerializeField] GameObject objectiveUI_Text;

    [Header("Buttons")]
    [SerializeField] Button acceptButton;
    [SerializeField] Button declineButton;
    [SerializeField] Button turnInButton;

    [Header("References")]
    [SerializeField] GameObject QuestUI;
    [SerializeField] NPCQuestIcon questIcon;
    [SerializeField] GetPlayerReference localPlayer;

    public void ShowQuestUI()
    {
        if (quests.Count == 0) return;

        Quest currentQuest = quests[QuestIndex];
        PlayerQuest playerQuest = localPlayer.player.GetComponent<PlayerQuest>();
        QuestProgress progress = playerQuest.activeQuests.Find(q => q.quest == currentQuest);

        UpdateQuestInfo(currentQuest, progress);
        QuestUI.SetActive(true);
    }

    void UpdateQuestInfo(Quest quest, QuestProgress progress)
    {
        questTitle.text = quest.QuestName;
        questRewardText.text = quest.goldReward.ToString() + " <sprite=0>" + quest.expReward.ToString() + " <sprite name=\"EXP Icon\">";

        ClearList();
        GetRewards(quest);
        GetObjectives(quest);

        if (progress == null)
        {
            questInfo.text = quest.Instructions;

            acceptButton.gameObject.SetActive(true);
            turnInButton.gameObject.SetActive(false);
        }
        else if (progress.state == QuestState.ReadyToTurnIn)
        {
            questInfo.text = quest.Deliver;

            acceptButton.gameObject.SetActive(false);
            turnInButton.gameObject.SetActive(true);
        }
        else
        {
            questInfo.text = quest.Deliver;

            acceptButton.gameObject.SetActive(false);
            turnInButton.gameObject.SetActive(false);
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
            if (image != null) image.sprite = reward.Icon;
        }
    }

    void GetObjectives(Quest quest)
    {
        foreach (QuestObjective objective in quest.Objectives)
        {
            GameObject objectiveText = Instantiate(objectiveUI_Text, objectiveListUI.transform);
            TextMeshProUGUI text = objectiveText.GetComponent<TextMeshProUGUI>();
            if (text != null) text.text = objective.Description;
        }
    }

    public void AcceptButton()
    {
        Quest quest = quests[QuestIndex];
        localPlayer.player.GetComponent<PlayerQuest>().AcceptQuest(quest);
        ShowQuestUI();

        DeclineButton();
        questIcon.UpdateSprite();
    }

    public void TurnInButton()
    {
        Quest quest = quests[QuestIndex];
        localPlayer.player.GetComponent<PlayerQuest>().TurnInQuest(quest);

        // Move to next quest if available
        if (QuestIndex < quests.Count - 1)
        {
            QuestIndex++;
        }

        ShowQuestUI();

        DeclineButton();
        questIcon.UpdateSprite();
    }

    public void DeclineButton()
    {
        QuestUI.SetActive(false);
        localPlayer.player.GetComponent<PlayerInteract>().BackButton();
    }
}