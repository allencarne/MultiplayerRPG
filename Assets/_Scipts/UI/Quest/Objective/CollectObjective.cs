
[System.Serializable]
public class CollectObjective : Objective
{
    public string itemName;
    public int requiredAmount;
    public int currentAmount;

    public override void CheckCompletion()
    {
        if (currentAmount >= requiredAmount)
        {
            isCompleted = true;
        }
    }

    public void ItemCollected(string collectedItemName)
    {
        if (!isCompleted && collectedItemName == itemName)
        {
            currentAmount++;
            CheckCompletion();
        }
    }
}
