
[System.Serializable]
public abstract class Objective
{
    public string description;
    public bool isCompleted;

    public abstract void CheckCompletion();
}
