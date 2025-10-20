using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerQuest : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] Inventory inventory;
    [SerializeField] PlayerExperience experience;
    [SerializeField] Item coin;

    public UnityEvent OnAccept;
    public UnityEvent OnProgress;
    public UnityEvent OnCompleted;
    public List<QuestProgress> activeQuests = new List<QuestProgress>();

    public void AcceptQuest(Quest quest)
    {
        if (activeQuests.Exists(q => q.quest == quest)) return;

        QuestProgress progress = new QuestProgress(quest);
        foreach (Item item in quest.Starter) inventory.AddItem(item);
        activeQuests.Add(progress);

        CheckInventoryForQuestItems(progress);

        OnAccept?.Invoke();
    }

    void CheckInventoryForQuestItems(QuestProgress progress)
    {
        foreach (QuestObjective objective in progress.objectives)
        {
            if (objective.type != ObjectiveType.Collect) continue;

            int itemCount = GetItemCountInInventory(objective.ObjectiveID);
            if (itemCount > 0)
            {
                objective.CurrentAmount = Mathf.Min(itemCount, objective.RequiredAmount);
            }
        }

        progress.CheckCompletion();
    }

    int GetItemCountInInventory(string itemID)
    {
        int total = 0;
        foreach (InventorySlotData slot in inventory.items)
        {
            if (slot != null && slot.item.ITEM_ID == itemID)
            {
                total += slot.quantity;
            }
        }
        return total;
    }

    public void UpdateObjective(ObjectiveType type, string id, int amount = 1)
    {
        foreach (QuestProgress progress in activeQuests)
        {
            if (progress.state != QuestState.InProgress) continue;

            foreach (QuestObjective obj in progress.objectives)
            {
                if (obj.type == type && obj.ObjectiveID == id && !obj.IsCompleted)
                {
                    obj.CurrentAmount += amount;
                    if (obj.CurrentAmount > obj.RequiredAmount)
                        obj.CurrentAmount = obj.RequiredAmount;
                }
            }

            progress.CheckCompletion();
            OnProgress?.Invoke();
        }
    }

    public void TurnInQuest(Quest quest)
    {
        QuestProgress progress = activeQuests.Find(q => q.quest == quest);
        if (progress == null || progress.state != QuestState.ReadyToTurnIn) return;

        // Verify player still has all required items
        if (!HasRequiredItems(progress))
        {
            Debug.Log("You no longer have the required items!");
            CheckInventoryForQuestItems(progress);
            OnProgress?.Invoke();
            return;
        }

        // Remove collected items from inventory
        RemoveQuestItems(progress);

        foreach (Item item in quest.reward) inventory.AddItem(item);
        if (quest.goldReward > 0) inventory.AddItem(coin, quest.goldReward);
        if (experience != null) experience.IncreaseEXP(quest.expReward);

        progress.state = QuestState.Completed;
        OnCompleted?.Invoke();
    }

    public Quest GetQuestReadyToTurnInForReceiver(string npcID)
    {
        foreach (QuestProgress progress in activeQuests)
        {
            if (progress.state != QuestState.ReadyToTurnIn)
                continue;

            Quest quest = progress.quest;

            if (quest.HasTalkObjective())
            {
                string receiverID = quest.GetReceiverID();
                if (receiverID == npcID)
                    return quest;
            }
            else
            {
                return quest;
            }
        }

        return null;
    }

    void RemoveQuestItems(QuestProgress progress)
    {
        foreach (QuestObjective objective in progress.objectives)
        {
            if (objective.type == ObjectiveType.Collect && objective.IsCompleted)
            {
                inventory.RemoveItemByID(objective.ObjectiveID, objective.RequiredAmount);
            }
        }
    }

    bool HasRequiredItems(QuestProgress progress)
    {
        foreach (QuestObjective objective in progress.objectives)
        {
            if (objective.type == ObjectiveType.Collect)
            {
                int currentCount = GetItemCountInInventory(objective.ObjectiveID);
                if (currentCount < objective.RequiredAmount)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void OnItemRemoved(string itemID, int quantity)
    {
        foreach (QuestProgress progress in activeQuests)
        {
            if (progress.state != QuestState.InProgress && progress.state != QuestState.ReadyToTurnIn) continue;

            foreach (QuestObjective objective in progress.objectives)
            {
                if (objective.type == ObjectiveType.Collect && objective.ObjectiveID == itemID)
                {
                    // Recalculate current amount based on actual inventory
                    int currentCount = GetItemCountInInventory(itemID);
                    objective.CurrentAmount = Mathf.Min(currentCount, objective.RequiredAmount);
                }
            }

            progress.CheckCompletion();
            OnProgress?.Invoke();
        }
    }
}