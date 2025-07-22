using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class NPCDialogue : MonoBehaviour
{
    [SerializeField] GameObject diaalogueUI;
    [SerializeField] TextMeshProUGUI textBox;
    [SerializeField] Button continueButton;
    [SerializeField] Button backButton;

    [TextArea(3, 8)] public string[] Dialogue;

    int conversationIndex;
    Player playerReference;

    public void StartDialogue(Player player)
    {
        diaalogueUI.SetActive(true);
        UpdateDialogueText();
        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(continueButton.gameObject);
        if (playerReference == null) playerReference = player;
    }

    public void ContinueButton()
    {
        if (conversationIndex < Dialogue.Length - 1)
        {
            conversationIndex++;
            UpdateDialogueText();
        }
        else
        {
            CloseDialogue();
        }
    }

    public void BackButton()
    {
        if (conversationIndex > 0)
        {
            conversationIndex--;
            UpdateDialogueText();
        }
        else
        {
            CloseDialogue();
        }
    }

    void UpdateDialogueText()
    {
        textBox.text = Dialogue[conversationIndex];
    }

    void CloseDialogue()
    {
        if (playerReference != null)
        {
            PlayerInteract playerInteract = playerReference.GetComponent<PlayerInteract>();
            if (playerInteract != null)
            {
                diaalogueUI.SetActive(false);
                playerInteract.BackButton();
            }
        }
    }
}
