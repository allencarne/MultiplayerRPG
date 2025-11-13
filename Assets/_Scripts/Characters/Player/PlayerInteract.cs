using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerInteract : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] Player player;
    [SerializeField] PlayerQuest playerQuest;
    [SerializeField] PlayerUI playerUI;

    [Header("Panel")]
    [SerializeField] GameObject interactPanel;
    [SerializeField] GameObject questInfoPanel;

    [Header("Input")]
    [SerializeField] PlayerInputHandler input;
    [SerializeField] PlayerInput playerInput;
    [SerializeField] InputActionReference interactAction;

    [Header("Text")]
    [SerializeField] TextMeshProUGUI npcNameText;
    [SerializeField] TextMeshProUGUI npcDialogueText;
    [SerializeField] TextMeshProUGUI interactText;

    [Header("Button")]
    [SerializeField] Button startButton;
    [SerializeField] Button questButton;
    [SerializeField] Button shopButton;

    NPC npcReference;
    IInteractable currentInteractable;

    private void Awake()
    {
        playerInput.onControlsChanged += OnControlsChanged;
    }

    private void OnDestroy()
    {
        playerInput.onControlsChanged -= OnControlsChanged;
    }

    private void Start()
    {
        interactPanel.SetActive(false);
        interactText.enabled = false;
        UpdateInteractText(null);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!player.IsLocalPlayer) return;

        IInteractable interactable = collision.GetComponent<IInteractable>();
        if (interactable == null) return;

        currentInteractable = interactable;
        interactText.enabled = true;

        npcReference = interactable as NPC;
        UpdateInteractText(collision.name);
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
            if (player.IsInteracting) return;
            player.IsInteracting = true;
            interactText.enabled = false;
            currentInteractable.Interact(this);
        }

        // Re-Interact
        if (interactText.enabled == false && currentInteractable != null)
        {
            interactText.enabled = true;
            npcReference = interactable as NPC;
            UpdateInteractText(collision.name);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!player.IsLocalPlayer) return; 

        IInteractable interactable = collision.GetComponent<IInteractable>(); 
        if (interactable == null) return; 
        if (interactable == currentInteractable)
        {
            CloseInteractUI();
        }
    }

    private void UpdateInteractText(string name)
    {
        int bindingIndex = GetBindingIndexForCurrentScheme(playerInput.currentControlScheme);
        string bindName = interactAction.action.GetBindingDisplayString(bindingIndex);
        interactText.text = $"Press <color=#00FF00>{bindName}</color> to Interact with <color=#00FF00>{name}</color>";
    }

    public void CloseInteractUI()
    {
        // Called From Closing QuestPanel, Dialogue Panel
        interactPanel.SetActive(false);
        questInfoPanel.SetActive(false);
        interactText.enabled = false;
        player.IsInteracting = false;
        npcReference = null;
    }

    public void OpenInteractUI()
    {
        // Dialogue
        npcNameText.text = npcReference.name;
        npcDialogueText.text = npcReference.GetComponent<NPCDialogue>().GetDialogue();
        playerUI._InteractUI();
    }

    public void OpenQuestUI(Quest quest)
    {
        Debug.Log(npcReference);
        Debug.Log(quest);

        // UI
        interactText.enabled = false;
        questInfoPanel.GetComponent<QuestInfoPanel>().UpdateQuestInfo(npcReference, quest);
        playerUI._QuestInfoUI();
    }

    public void OpenShopUI()
    {
        // Open Shop Panel
    }

    public void Activate()
    {
        // Activate Totem, Chests, Ect.
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