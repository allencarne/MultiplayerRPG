using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI interactText;

    // Interact
    [SerializeField] GameObject interactUI;
    [SerializeField] TextMeshProUGUI npcNameText;
    [SerializeField] Button dialogueButton;
    [SerializeField] Button questButton;
    [SerializeField] Button shopButton;
    [SerializeField] Button backButton;

    [SerializeField] Player player;
    [SerializeField] PlayerInputHandler input;
    [SerializeField] PlayerInput playerInput;
    [SerializeField] PlayerQuest playerQuest;
    [SerializeField] InputActionReference interactAction;

    NPC npcReference;

    private void Awake()
    {
        playerInput.onControlsChanged += OnControlsChanged;
    }

    private void Start()
    {
        interactText.enabled = false;
        interactUI.SetActive(false);
        UpdateInteractText(null);
    }

    private void OnDestroy()
    {
        playerInput.onControlsChanged -= OnControlsChanged;
    }

    private void OnControlsChanged(PlayerInput input)
    {
        UpdateInteractText(null);
    }

    private void UpdateInteractText(string name)
    {
        if (interactAction == null) return;

        string controlScheme = playerInput.currentControlScheme;
        int bindingIndex = GetBindingIndexForCurrentScheme(controlScheme);

        string bindName = interactAction.action.GetBindingDisplayString(bindingIndex);
        interactText.text = $"Press <color=#00FF00>{bindName}</color> to Interact with <color=#00FF00>{name}</color>";
    }

    private int GetBindingIndexForCurrentScheme(string scheme)
    {
        if (string.IsNullOrEmpty(scheme)) return 0;

        for (int i = 0; i < interactAction.action.bindings.Count; i++)
        {
            var binding = interactAction.action.bindings[i];
            if (binding.groups.Contains(scheme))
            {
                return i;
            }
        }

        return 0; // Fallback
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!player.IsLocalPlayer) return;
        if (!collision.CompareTag("NPC")) return;
        interactText.enabled = true;
        UpdateInteractText(collision.name);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!player.IsLocalPlayer) return;
        if (!collision.CompareTag("NPC")) return;
        NPC npc = collision.GetComponent<NPC>();

        if (input.InteractInput)
        {
            if (player.IsInteracting) return;
            player.IsInteracting = true;

            interactText.enabled = false;
            playerInput.SwitchCurrentActionMap("UI");
            SetupInteractUI(npc.name, npc.type);
        }

        if (npcReference == null)
        {
            npcReference = npc;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!player.IsLocalPlayer) return;
        if (!collision.CompareTag("NPC")) return;
        interactText.enabled = false;
        player.IsInteracting = false;
        npcReference = null;
    }

    void SetupInteractUI(string name, NPC.Type type)
    {
        npcNameText.text = name;

        bool hasDialogue = GetDialogue();
        bool hasQuest = GetQuest();
        bool hasShop = GetShop();
        backButton.gameObject.SetActive(true);

        // Build list of active buttons
        List<Button> activeButtons = new List<Button>();
        if (hasDialogue) activeButtons.Add(dialogueButton);
        if (hasQuest) activeButtons.Add(questButton);
        if (hasShop) activeButtons.Add(shopButton);
        activeButtons.Add(backButton);

        // Set navigation dynamically
        for (int i = 0; i < activeButtons.Count; i++)
        {
            Button current = activeButtons[i];
            Button up = i > 0 ? activeButtons[i - 1] : activeButtons[^1];
            Button down = i < activeButtons.Count - 1 ? activeButtons[i + 1] : activeButtons[0];
            SetNavigation(current, up, down);
        }

        // Set first selected button
        if (EventSystem.current != null && activeButtons.Count > 0)
        {
            EventSystem.current.SetSelectedGameObject(activeButtons[0].gameObject);
        }

        // Enable UI
        interactUI.SetActive(true);
    }

    bool GetDialogue()
    {
        if (npcReference != null)
        {
            NPCDialogue dialogue = npcReference.GetComponent<NPCDialogue>();
            if (dialogue != null && dialogue.Dialogue != null && dialogue.Dialogue.Length > 0)
            {
                dialogueButton.gameObject.SetActive(true);
                return true;
            }
        }

        dialogueButton.gameObject.SetActive(false);
        return false;
    }

    bool GetQuest()
    {
        if (npcReference != null)
        {
            NPCQuest tracker = npcReference.GetComponent<NPCQuest>();
            Quest currentQuest = tracker.quests[tracker.QuestIndex];
            QuestProgress progress = playerQuest.activeQuests.Find(q => q.quest == currentQuest);

            if (tracker.quests.Count == 0)
            {
                questButton.gameObject.SetActive(false);
                return false;
            }

            switch (progress.state)
            {
                case QuestState.Unavailable:
                    questButton.gameObject.SetActive(false);
                    return false;
                case QuestState.Available:
                    questButton.gameObject.SetActive(true);
                    return true;
                case QuestState.InProgress:
                    questButton.gameObject.SetActive(false);
                    return false;
                case QuestState.ReadyToTurnIn:
                    questButton.gameObject.SetActive(true);
                    return true;
                case QuestState.Completed:
                    questButton.gameObject.SetActive(false);
                    return false;
            }
        }

        questButton.gameObject.SetActive(false);
        return false;
    }

    bool GetShop()
    {
        // Add shop logic later
        shopButton.gameObject.SetActive(false);
        return false;
    }

    private void SetNavigation(Button button, Button selectOnUp, Button selectOnDown)
    {
        var nav = button.navigation;
        nav.mode = Navigation.Mode.Explicit;
        nav.selectOnUp = selectOnUp;
        nav.selectOnDown = selectOnDown;
        button.navigation = nav;
    }

    public void BackButton()
    {
        if (!player.IsInteracting) return;

        player.IsInteracting = false;
        interactText.enabled = true;
        interactUI.SetActive(false);
        playerInput.SwitchCurrentActionMap("Player");
        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
        npcReference = null;
    }

    public void DialogueButton()
    {
        if (npcReference == null) return;

        NPCDialogue dialogue = npcReference.GetComponent<NPCDialogue>();
        if (dialogue != null)
        {
            interactText.enabled = false;
            interactUI.SetActive(false);

            dialogue.StartDialogue(player);
        }
    }

    public void QuestButton()
    {
        if (npcReference == null) return;

        NPCQuest npcQuest = npcReference.GetComponent<NPCQuest>();
        if (npcQuest != null)
        {
            interactText.enabled = false;
            interactUI.SetActive(false);

            npcQuest.ShowQuestUI();
        }
    }
}
