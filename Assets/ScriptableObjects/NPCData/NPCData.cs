using UnityEngine;

[CreateAssetMenu(fileName = "NPCData", menuName = "Scriptable Objects/NPCData")]
public class NPCData : ScriptableObject
{
    public int NPCID;
    public string NPCName;
    public int NPC_Level;

    public Quest[] Quests;
    [TextArea(3, 8)] public string[] Dialogue;
    public Item[] Items;
}
