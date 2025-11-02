using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerQuest : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] Inventory inventory;
    [SerializeField] PlayerExperience experience;
    [SerializeField] Item coin;

    public UnityEvent OnQuestStateChanged;
    public UnityEvent<Quest> OnQuestTurnedIn;
    public List<QuestProgress> activeQuests = new List<QuestProgress>();

    public QuestState GetQuestStateForNpc(NPC npc, NPCQuest npcQuest)
    {
        foreach (QuestProgress progress in activeQuests)
        {
            if (progress.quest.HasTalkObjective() && progress.quest.GetReceiverID() == npc.NPC_ID)
            {
                if (progress.state == QuestState.InProgress || progress.state == QuestState.ReadyToTurnIn)
                {
                    return QuestState.ReadyToTurnIn;
                }
            }

            if (!npcQuest.quests.Contains(progress.quest)) continue;
            if (progress.state == QuestState.ReadyToTurnIn) return QuestState.ReadyToTurnIn;
            if (progress.state == QuestState.InProgress) return QuestState.InProgress;
        }

        if (npcQuest.quests == null || npcQuest.quests.Count == 0) return QuestState.None;
        Quest candidateQuest = npcQuest.quests[npcQuest.QuestIndex];

        if (player.PlayerLevel.Value < candidateQuest.LevelRequirment) return QuestState.Unavailable;
        if (!npcQuest.HasMetQuestRequirements(this, candidateQuest)) return QuestState.Unavailable;

        foreach (Quest quest in npcQuest.quests)
        {
            bool alreadyAccepted = activeQuests.Exists(q => q.quest == quest);
            if (!alreadyAccepted) return QuestState.Available;
        }

        return QuestState.None;
    }

    public void AcceptQuest(Quest quest)
    {
        if (activeQuests.Exists(q => q.quest == quest)) return;

        QuestProgress progress = new QuestProgress(quest);
        foreach (Item item in quest.Starter) inventory.AddItem(item);
        activeQuests.Add(progress);

        CheckInventoryForQuestItems(progress);

        OnQuestStateChanged?.Invoke();
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
            OnQuestStateChanged?.Invoke();
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
            OnQuestStateChanged?.Invoke();
            return;
        }

        // Remove collected items from inventory
        RemoveQuestItems(progress);

        foreach (Item item in quest.reward) inventory.AddItem(item);
        if (quest.goldReward > 0) inventory.AddItem(coin, quest.goldReward);
        if (experience != null) experience.IncreaseEXP(quest.expReward);

        progress.state = QuestState.Completed;
        OnQuestStateChanged?.Invoke();
        OnQuestTurnedIn?.Invoke(quest);
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
            OnQuestStateChanged?.Invoke();
        }
    }

    public bool HasRequiredItemsForUI(QuestProgress progress)
    {
        return HasRequiredItems(progress);
    }
}