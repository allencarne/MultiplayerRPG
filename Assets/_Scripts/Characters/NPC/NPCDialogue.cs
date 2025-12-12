using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    [SerializeField] NPC npc;
    int conversationIndex;

    public string GetDialogue()
    {
        if (npc.Data.Dialogue == null || npc.Data.Dialogue.Length == 0) return "...";

        // Clamp index so it never goes out of bounds
        if (conversationIndex < 0 || conversationIndex >= npc.Data.Dialogue.Length) conversationIndex = 0;

        return npc.Data.Dialogue[conversationIndex];
    }

    public void AdvanceDialogue()
    {
        conversationIndex++;

        // Optional: Loop back to start
        if (conversationIndex >= npc.Data.Dialogue.Length) conversationIndex = npc.Data.Dialogue.Length - 1;
    }

    public void ResetDialogue()
    {
        conversationIndex = 0;
    }
}
