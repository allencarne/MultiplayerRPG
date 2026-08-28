using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Quest/QuestList")]
public class QuestList : ScriptableObject
{
    public Quest[] QuestDatabase;
}
