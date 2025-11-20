using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : NetworkBehaviour
{
    [Header("Player")]
    [SerializeField] Player player;
    [SerializeField] PlayerQuest playerQuest;
    [SerializeField] PlayerUI playerUI;

    [Header("Input")]
    [SerializeField] PlayerInputHandler input;
    [SerializeField] PlayerInput playerInput;
    [SerializeField] InputActionReference interactAction;

    [Header("Text")]
    [SerializeField] TextMeshProUGUI npcDialogueNameText;
    [SerializeField] TextMeshProUGUI npcDialogueText;
    [SerializeField] TextMeshProUGUI interactText;

    [Header("Panel")]
    [SerializeField] QuestInfoPanel questInfoPanel;

    IInteractable currentInteractable;
    bool hasInteracted = false;

    private void Awake()
    {
        playerInput.onControlsChanged += OnControlsChanged;
    }

    protected new void OnDestroy()
    {
        playerInput.onControlsChanged -= OnControlsChanged;
    }

    private void Start()
    {
        interactText.enabled = false;
        UpdateInteractText(null);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!player.IsLocalPlayer) return;

        IInteractable interactable = collision.GetComponent<IInteractable>();
        if (interactable == null) return;

        UpdateInteractText(interactable.DisplayName);

        currentInteractable = interactable;
        interactText.enabled = true;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!player.IsLocalPlayer) return;
        if (currentInteractable == null) return;

        IInteractable interactable = collision.GetComponent<IInteractable>();
        if (interactable == null) return;

        // Interact
        if (input.InteractInput)
        {
            if (player.IsInteracting || hasInteracted) return;
            player.IsInteracting = true;
            interactText.enabled = false;
            hasInteracted = true;
            currentInteractable.Interact(this);
        }

        // Re-Interact
        if (!player.IsInteracting && hasInteracted)
        {
            interactText.enabled = true;
            hasInteracted = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!player.IsLocalPlayer) return; 

        IInteractable interactable = collision.GetComponent<IInteractable>(); 
        if (interactable == null) return; 
        if (interactable == currentInteractable)
        {
            CloseUI();
        }
    }

    public void OpenDialogueUI(string _name, NPCDialogue dialogue)
    {
        // Dialogue
        npcDialogueNameText.text = _name;
        npcDialogueText.text = dialogue.GetDialogue();
        playerUI._InteractUI();
    }

    public void OpenQuestUI(Quest quest, NPC npc)
    {
        interactText.enabled = false;
        questInfoPanel.UpdateQuestInfo(npc, quest);
        playerUI._QuestInfoUI();
    }

    public void OpenShopUI()
    {
        // Open Shop Panel
    }

    public void CloseUI()
    {
        // Called From: QuestPanel, Decline Button, Turn-In Button, Dialogue Panel
        interactText.enabled = false;
        player.IsInteracting = false;
        hasInteracted = true;
    }

    private void UpdateInteractText(string name)
    {
        int bindingIndex = GetBindingIndexForCurrentScheme(playerInput.currentControlScheme);
        string bindName = interactAction.action.GetBindingDisplayString(bindingIndex);
        interactText.text = $"Press <color=#00FF00>{bindName}</color> to Interact with <color=#00FF00>{name}</color>";
    }

    private void OnControlsChanged(PlayerInput input)
    {
        UpdateInteractText(null);
    }

    private int GetBindingIndexForCurrentScheme(string scheme)
    {
        if (string.IsNullOrEmpty(scheme)) return 0;

        for (int i = 0; i < interactAction.action.bindings.Count; i++)
        {
            InputBinding binding = interactAction.action.bindings[i];
            if (binding.groups.Contains(scheme))
            {
                return i;
            }
        }

        return 0;
    }
}