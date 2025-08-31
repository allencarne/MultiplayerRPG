using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    [TextArea(3, 8)] public string[] Dialogue;
    int conversationIndex;

    public string GetDialogue()
    {
        if (Dialogue == null || Dialogue.Length == 0) return "...";

        // Clamp index so it never goes out of bounds
        if (conversationIndex < 0 || conversationIndex >= Dialogue.Length) conversationIndex = 0;

        return Dialogue[conversationIndex];
    }

    public void AdvanceDialogue()
    {
        conversationIndex++;

        // Optional: Loop back to start
        if (conversationIndex >= Dialogue.Length) conversationIndex = Dialogue.Length - 1;
    }

    public void ResetDialogue()
    {
        conversationIndex = 0;
    }
}
