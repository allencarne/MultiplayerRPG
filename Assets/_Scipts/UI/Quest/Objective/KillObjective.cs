
[System.Serializable]
public class KillObjective : Objective
{
    public string enemyName;
    public int requiredAmount;
    public int currentAmount;

    public override void CheckCompletion()
    {
        if (currentAmount >= requiredAmount)
        {
            isCompleted = true;
        }
    }

    public void EnemyKilled(string killedEnemyName)
    {
        if (!isCompleted && killedEnemyName == enemyName)
        {
            currentAmount++;
            CheckCompletion();
        }
    }
}
