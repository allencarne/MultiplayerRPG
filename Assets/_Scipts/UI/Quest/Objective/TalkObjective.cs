
[System.Serializable]
public class TalkObjective : Objective
{
    public string npcName;

    public override void CheckCompletion()
    {
        isCompleted = true;
    }

    public void TalkToNPC(string talkedNpcName)
    {
        if (!isCompleted && talkedNpcName == npcName)
        {
            CheckCompletion();
        }
    }
}
