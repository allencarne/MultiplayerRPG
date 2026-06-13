using System.Collections;
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
        GetObjectives(quest, progress);

        if (progress == null)
        {
            questInfo.text = quest.Instructions;
            acceptButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Accept";
        }
        else
        {
            bool canTurnInHere = false;

            if (progress.state == QuestState.ReadyToTurnIn)
            {
                if (currentQuest.HasTalkObjective())
                {
                    canTurnInHere = currentQuest.GetReceiverID() == npc.Data.NPC_ID;
                }
                else
                {
                    canTurnInHere = npc.Data.Quests != null && npc.Data.Quests.Contains(currentQuest);
                }
            }

            // Also show Turn In at receiver if talk objective still InProgress AND target is this NPC
            if (!canTurnInHere && progress.state == QuestState.InProgress)
            {
                bool hasTalkObjectiveForThisNPC = progress.objectives.Exists(obj =>
                    obj.type == ObjectiveType.Talk &&
                    obj.ObjectiveID == npc.Data.NPC_ID &&
                    !obj.IsCompleted);

                if (hasTalkObjectiveForThisNPC)
                {
                    canTurnInHere = true;
                }
            }

            if (canTurnInHere)
            {
                questInfo.text = quest.Deliver;
                acceptButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Turn In";
            }
            else
            {
                // Default display
                questInfo.text = quest.Instructions;
                acceptButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Accept";
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

    void GetObjectives(Quest quest, QuestProgress progress)
    {
        foreach (QuestObjective objective in quest.Objectives)
        {
            GameObject objectiveText = Instantiate(objectiveUI_Text, objectiveListUI.transform);
            TextMeshProUGUI text = objectiveText.GetComponent<TextMeshProUGUI>();

            if (text != null)
            {
                // Check if the objective is completed based on the quest progress
                bool isCompleted = progress != null && progress.objectives.Exists(o => o.ObjectiveID == objective.ObjectiveID && o.IsCompleted);

                if (isCompleted)
                {
                    // If the objective is completed, display it with strikethrough
                    text.text = $"<s>{objective.Description}</s>";
                }
                else
                {
                    // If the objective is not completed, display it normally
                    text.text = objective.Description;
                }
            }
        }
    }

    public void AcceptButton()
    {
        QuestProgress progress = playerquests.activeQuests.Find(q => q.quest == currentQuest);

        if (progress == null)
        {
            AcceptQuest();
        }
        else
        {
            TurnInQuest();
        }
    }

    void AcceptQuest()
    {
        int avaliableSlots = inventory.GetFreeSlotCount();
        int starterSlots = currentQuest.Starter.Count();

        if (avaliableSlots < starterSlots)
        {
            Debug.Log("Not enough inventory space to accept this quest!");
            return;
        }

        playerquests.AcceptQuest(currentQuest);
    }

    void TurnInQuest()
    {
        int avaliableSlots = inventory.GetFreeSlotCount();
        int rewardSlots = currentQuest.reward.Count();

        if (avaliableSlots < rewardSlots)
        {
            Debug.Log("Not enough inventory space to turn in quest!");
            return;
        }

        QuestProgress progress = playerquests.activeQuests.Find(q => q.quest == currentQuest);
        if (progress == null)
        {
            Debug.LogWarning("No active quest progress found for this quest.");
            return;
        }

        if (!playerquests.HasRequiredItemsForUI(progress))
        {
            Debug.Log("You no longer have the required quest items to turn in!");
            return;
        }

        bool isTalkQuest = currentQuest.HasTalkObjective();
        if (isTalkQuest) playerquests.UpdateObjective(ObjectiveType.Talk, currentNPC.Data.NPC_ID);

        playerquests.TurnInQuest(currentQuest);
    }
}