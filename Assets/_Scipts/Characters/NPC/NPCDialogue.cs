using TMPro;
using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    [SerializeField] GameObject diaalogueUI;
    [SerializeField] TextMeshProUGUI textBox;

    [TextArea(3, 8)] public string[] Dialogue;

    public void StartDialogue()
    {
        diaalogueUI.SetActive(true);
    }
}
