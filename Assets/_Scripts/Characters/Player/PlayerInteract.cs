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
    Quest activeQuest;

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
        if (!collision.CompareTag("NPC")) return;

        npcReference = collision.GetComponent<NPC>();
        interactText.enabled = true;
        UpdateInteractText(npcReference.name);
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

            playerQuest.UpdateObjective(ObjectiveType.Talk, npcReference.NPCID);
            OpenUI(npcReference);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!player.IsLocalPlayer) return;
        if (!collision.CompareTag("NPC")) return;

        CloseUI();
    }

    public void CloseUI()
    {
        interactPanel.SetActive(false);
        questInfoPanel.SetActive(false);

        interactText.enabled = false;
        player.IsInteracting = false;

        npcReference = null;
        activeQuest = null;
    }

    void OpenUI(NPC npc)
    {
        npcNameText.text = npc.name;

        // Dialogue
        npcDialogueText.text = npcReference.GetComponent<NPCDialogue>().GetDialogue();

        // Quests
        NPCQuest npcQuest = npcReference.GetComponent<NPCQuest>();
        activeQuest = npcQuest?.GetAvailableQuest(playerQuest);
        questButton.gameObject.SetActive(activeQuest != null);

        // Get Shop

        // Get Start

        // Handle UI
        OnInteract?.Invoke();
    }

    public void QuestButton()
    {
        if (activeQuest == null) return;
        interactText.enabled = false;
        interactPanel.SetActive(false);

        questInfoPanel.GetComponent<QuestInfoPanel>().UpdateQuestInfo(npcReference, activeQuest);
    }
}