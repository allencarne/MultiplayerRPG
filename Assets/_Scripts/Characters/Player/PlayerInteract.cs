using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerInteract : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] Player player;
    [SerializeField] PlayerQuest playerQuest;

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

    public UnityEvent OnInteract;
    NPC npcReference;

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

    private void OnControlsChanged(PlayerInput input)
    {
        UpdateInteractText(null);
    }

    private void UpdateInteractText(string name)
    {
        int bindingIndex = GetBindingIndexForCurrentScheme(playerInput.currentControlScheme);
        string bindName = interactAction.action.GetBindingDisplayString(bindingIndex);
        interactText.text = $"Press <color=#00FF00>{bindName}</color> to Interact with <color=#00FF00>{name}</color>";
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!player.IsLocalPlayer) return;

        IInteractable interactable = collision.GetComponent<IInteractable>();
        if (interactable != null)
        {
            // Enable Text
            interactText.enabled = true;
            UpdateInteractText(collision.name);
        }

        if (!collision.CompareTag("NPC")) return;
        interactText.enabled = true;
        npcReference = collision.GetComponent<NPC>();
        UpdateInteractText(npcReference.NPC_Name);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!player.IsLocalPlayer) return;
        if (!collision.CompareTag("NPC")) return;

        if (input.InteractInput && npcReference != null)
        {
            if (player.IsInteracting) return;
            player.IsInteracting = true;
            interactText.enabled = false;

            OpenInteractUI(npcReference);
        }

        if (npcReference == null && interactText.enabled == false)
        {
            interactText.enabled = true;
            npcReference = collision.GetComponent<NPC>();
            UpdateInteractText(npcReference.NPC_Name);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!player.IsLocalPlayer) return;
        if (!collision.CompareTag("NPC")) return;

        CloseInteractUI();
    }

    public void CloseInteractUI()
    {
        interactPanel.SetActive(false);
        questInfoPanel.SetActive(false);
        interactText.enabled = false;
        player.IsInteracting = false;
        npcReference = null;
    }

    void OpenInteractUI(NPC npc)
    {
        // Dialogue
        npcNameText.text = npc.name;
        npcDialogueText.text = npcReference.GetComponent<NPCDialogue>().GetDialogue();

        // Quests
        NPCQuest npcQuest = npcReference.GetComponent<NPCQuest>();
        Quest currentQuest = npcQuest?.GetAvailableQuest(playerQuest);
        questButton.gameObject.SetActive(currentQuest != null);

        // Buttons
        shopButton.gameObject.SetActive(false);
        startButton.gameObject.SetActive(false);
        OnInteract?.Invoke();
    }

    public void QuestButton()
    {
        if (npcReference == null) return;

        // Get Quest
        NPCQuest npcQuest = npcReference.GetComponent<NPCQuest>();
        Quest currentQuest = npcQuest?.GetAvailableQuest(playerQuest);
        if (currentQuest == null) return;

        // UI
        interactText.enabled = false;
        interactPanel.SetActive(false);

        // Update Panel
        questInfoPanel.GetComponent<QuestInfoPanel>().UpdateQuestInfo(npcReference, currentQuest);
    }
}