using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuestInfoPanel : MonoBehaviour
{
    [SerializeField] PlayerQuest playerquests;

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

    Quest currentQuest;
    NPC currentNPC;

    public void UpdateQuestInfo(NPC npc, Quest quest)
    {
        currentQuest = quest;
        currentNPC = npc;

        // Get the progress from playerquests
        QuestProgress progress = playerquests.activeQuests.Find(q => q.quest == quest);

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
            EventSystem.current.SetSelectedGameObject(acceptButton.gameObject);
        }
        else if (progress.state == QuestState.ReadyToTurnIn)
        {
            questInfo.text = quest.Deliver;

            acceptButton.gameObject.SetActive(false);
            turnInButton.gameObject.SetActive(true);
            EventSystem.current.SetSelectedGameObject(turnInButton.gameObject);
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
        playerquests.AcceptQuest(currentQuest);
        currentNPC.GetComponent<NPCQuestIcon>().UpdateSprite();
    }

    public void TurnInButton()
    {
        playerquests.TurnInQuest(currentQuest);

        // Move to next quest if available
        NPCQuest npcQuest = currentNPC.GetComponent<NPCQuest>();
        if (npcQuest.QuestIndex < npcQuest.quests.Count - 1)
        {
            npcQuest.QuestIndex++;
        }

        currentNPC.GetComponent<NPCQuestIcon>().UpdateSprite();
    }
}