using UnityEngine;

[System.Serializable]
public class QuestProgress
{
    public Quest quest;

    public QuestState currentState;
    public bool[] objectiveStates;

    public QuestProgress(Quest quest)
    {
        this.quest = quest;
        currentState = QuestState.Available;

        objectiveStates = new bool[quest.objectives.Length];
    }

    public bool IsObjectiveComplete(int index)
    {
        return index >= 0 && index < objectiveStates.Length && objectiveStates[index];
    }

    public void CompleteObjective(int index)
    {
        if (index >= 0 && index < objectiveStates.Length)
        {
            objectiveStates[index] = true;
        }

        if (AllObjectivesCompleted())
        {
            currentState = QuestState.ReadyToTurnIn;
        }
    }

    public bool AllObjectivesCompleted()
    {
        foreach (bool isComplete in objectiveStates)
        {
            if (!isComplete) return false;
        }
        return true;
    }
}
