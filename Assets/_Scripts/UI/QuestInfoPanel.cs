using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuestInfoPanel : MonoBehaviour
{
    [SerializeField] PlayerQuest playerquests;
    [SerializeField] Player player;
    [SerializeField] Inventory inventory;

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

        QuestProgress progress = playerquests.activeQuests.Find(q => q.quest == quest);

        questTitle.text = quest.QuestName;
        questRewardText.text = quest.goldReward.ToString() + " <sprite=0>" + quest.expReward.ToString() + " <sprite name=\"EXP Icon\">";

        ClearList();
        GetRewards(quest);
        GetObjectives(quest);

        acceptButton.gameObject.SetActive(false);
        turnInButton.gameObject.SetActive(false);

        if (progress == null)
        {
            questInfo.text = quest.Instructions;
            acceptButton.gameObject.SetActive(true);
            EventSystem.current.SetSelectedGameObject(acceptButton.gameObject);
        }
        else if (progress.state == QuestState.ReadyToTurnIn)
        {
            questInfo.text = quest.Deliver;
            turnInButton.gameObject.SetActive(true);
            EventSystem.current.SetSelectedGameObject(turnInButton.gameObject);
        }
        else if (progress.state == QuestState.InProgress)
        {
            bool hasTalkObjectiveForThisNPC = progress.objectives.Exists(obj =>
                obj.type == ObjectiveType.Talk &&
                obj.ObjectiveID == npc.NPC_ID &&
                !obj.IsCompleted);

            if (hasTalkObjectiveForThisNPC)
            {
                questInfo.text = quest.Deliver;
                turnInButton.gameObject.SetActive(true);
                EventSystem.current.SetSelectedGameObject(turnInButton.gameObject);
            }
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
        int avaliableSlots = inventory.GetFreeSlotCount();
        int starterSlots = currentQuest.Starter.Count();

        if (avaliableSlots < starterSlots)
        {
            Debug.Log("Not enough inventory space to accept this quest!");
            return;
        }

        playerquests.AcceptQuest(currentQuest);
        currentNPC.GetComponent<NPCQuestIcon>().UpdateSprite();
    }

    public void TurnInButton()
    {
        int avaliableSlots = inventory.GetFreeSlotCount();
        int rewardSlots = currentQuest.reward.Count();

        if (avaliableSlots < rewardSlots)
        {
            Debug.Log("Not enough inventory space to turn in quest!");
            return;
        }

        playerquests.UpdateObjective(ObjectiveType.Talk, currentNPC.NPC_ID);
        playerquests.TurnInQuest(currentQuest);

        NPCQuest npcQuest = currentNPC.GetComponent<NPCQuest>();
        if (npcQuest.QuestIndex < npcQuest.quests.Count - 1) npcQuest.QuestIndex++;
        currentNPC.GetComponent<NPCQuestIcon>().UpdateSprite();
    }
}