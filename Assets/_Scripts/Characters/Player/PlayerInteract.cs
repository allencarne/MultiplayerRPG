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
    [SerializeField] GameObject interactUI;

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
        interactUI.SetActive(false);
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
        interactUI.SetActive(false);
        interactText.enabled = false;
        player.IsInteracting = false;
        npcReference = null;
    }

    void OpenUI(NPC npc)
    {
        npcNameText.text = npc.name;

        // Get NPC Dialogue
        npcDialogueText.text = npcReference.GetComponent<NPCDialogue>().GetDialogue();

        OnInteract?.Invoke();
    }

    bool GetQuest()
    {
        // No NPC Reference
        if (npcReference == null)
        {
            questButton.gameObject.SetActive(false);
            return false;
        }

        // If NPC Has No Quests
        NPCQuest tracker = npcReference.GetComponent<NPCQuest>();
        if (tracker == null || tracker.quests.Count == 0)
        {
            questButton.gameObject.SetActive(false);
            return false;
        }

        Quest currentQuest = tracker.quests[tracker.QuestIndex];
        QuestProgress progress = playerQuest.activeQuests.Find(q => q.quest == currentQuest);

        // We have not started the quest
        if (progress == null)
        {
            if (player.PlayerLevel.Value < currentQuest.LevelRequirment)
            {
                questButton.gameObject.SetActive(false);
                return false;
            }
            else
            {
                questButton.gameObject.SetActive(true);
                return true;
            }
        }

        if (progress != null)
        {
            if (progress.state == QuestState.Available || progress.state == QuestState.ReadyToTurnIn)
            {
                questButton.gameObject.SetActive(true);
                return true;
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

    public void QuestButton()
    {
        if (npcReference == null) return;

        NPCQuest npcQuest = npcReference.GetComponent<NPCQuest>();
        if (npcQuest != null)
        {
            interactText.enabled = false;
            //interactUI.SetActive(false);

            npcQuest.ShowQuestUI();
        }
    }
}