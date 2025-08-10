
[System.Serializable]
public class EquipObjective : Objective
{
    public string itemName;

    public override void CheckCompletion()
    {
        isCompleted = true;
    }

    public void EquipItem(string equippedItemName)
    {
        if (!isCompleted && equippedItemName == itemName)
        {
            CheckCompletion();
        }
    }
}
