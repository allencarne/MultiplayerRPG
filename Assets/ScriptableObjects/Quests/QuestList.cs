using UnityEngine;

[CreateAssetMenu(fileName = "QuestList", menuName = "Scriptable Objects/QuestList")]
public class QuestList : ScriptableObject
{
    public Quest[] QuestDatabase;
}
