using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerQuest : MonoBehaviour
{
    public UnityEvent OnAccept;
    public UnityEvent OnProgress;
    public UnityEvent OnCompleted;
    public List<QuestProgress> activeQuests = new List<QuestProgress>();

    public void AcceptQuest(Quest quest)
    {
        // Check if the quest is already active
        if (activeQuests.Exists(q => q.quest == quest))
        {
            Debug.LogWarning("Quest is already active!");
            return;
        }

        // Add a new QuestProgress for the accepted quest
        QuestProgress progress = new QuestProgress(quest);
        activeQuests.Add(progress);
        OnAccept?.Invoke();
        Debug.Log($"Quest '{quest.name}' accepted!");
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
        }

        OnProgress?.Invoke();
    }

    public void TurnInQuest(Quest quest)
    {
        QuestProgress progress = activeQuests.Find(q => q.quest == quest);

        if (progress == null)
        {
            Debug.LogWarning("You don't have this quest.");
            return;
        }

        if (progress.state != QuestState.ReadyToTurnIn)
        {
            Debug.LogWarning("Quest is not ready to turn in yet.");
            return;
        }

        // Give rewards
        Player player = GetComponent<Player>();
        Inventory inventory = GetComponent<Inventory>();
        PlayerExperience experience = GetComponent<PlayerExperience>();

        if (inventory != null)
        {
            foreach (Item item in quest.reward)
            {
                inventory.AddItem(item);
            }
        }

        if (player != null) player.CoinCollected(quest.goldReward);
        if (experience != null) experience.IncreaseEXP(quest.expReward);

        // Mark as completed
        progress.state = QuestState.Completed;
        OnCompleted?.Invoke();
        Debug.Log($"Quest '{quest.QuestName}' turned in and completed!");
    }
}
