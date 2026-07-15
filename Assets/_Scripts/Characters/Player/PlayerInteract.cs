using System.Collections.Generic;
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
    [SerializeField] GameObject questPanel;
    [SerializeField] GameObject dialoguePanel;
    [SerializeField] GameObject vendorPanel;

    [SerializeField] QuestInfoPanel questInfoPanel;
    [SerializeField] VendorInfoPanel vendorInfoPanel;

    readonly List<IInteractable> interactablesInRange = new List<IInteractable>();

    IInteractable currentTarget;
    bool hasInteracted = false;

    public override void OnNetworkSpawn()
    {
        playerInput.onControlsChanged += OnControlsChanged;
    }

    public override void OnNetworkDespawn()
    {
        playerInput.onControlsChanged -= OnControlsChanged;
    }

    private void Start()
    {
        interactText.enabled = false;
    }

    private void Update()
    {
        if (!player.IsLocalPlayer) return;

        RefreshTarget();

        if (currentTarget == null) return;

        // Interact
        if (input.InteractInput)
        {
            if (player.IsInteracting || hasInteracted) return;
            player.IsInteracting = true;
            interactText.enabled = false;
            hasInteracted = true;
            currentTarget.Interact(this);
        }

        // Re-Interact - re-show the prompt once whatever UI we opened has closed,
        // as long as we're still standing near the same target
        if (!player.IsInteracting && hasInteracted)
        {
            interactText.enabled = true;
            hasInteracted = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!player.IsLocalPlayer) return;

        IInteractable interactable = collision.GetComponent<IInteractable>();
        if (interactable == null || interactablesInRange.Contains(interactable)) return;

        interactablesInRange.Add(interactable);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!player.IsLocalPlayer) return;

        IInteractable interactable = collision.GetComponent<IInteractable>();
        if (interactable == null) return;

        interactablesInRange.Remove(interactable);

        if (currentTarget == interactable)
        {
            SetTarget(null);

            // Preserve old exit behavior: close any open UI tied to this interactable
            CloseUI();
            questPanel.SetActive(false);
            dialoguePanel.SetActive(false);
            vendorPanel.SetActive(false);
        }
    }

    void RefreshTarget()
    {
        IInteractable closest = null;
        float closestDist = float.MaxValue;

        foreach (IInteractable interactable in interactablesInRange)
        {
            // IInteractable doesn't expose a Transform directly, but every
            // implementation is a MonoBehaviour, so we can get one via Component
            Transform t = ((Component)interactable).transform;
            float dist = (t.position - transform.position).sqrMagnitude;

            if (dist < closestDist)
            {
                closestDist = dist;
                closest = interactable;
            }
        }

        if (closest != currentTarget) SetTarget(closest);
    }

    void SetTarget(IInteractable target)
    {
        currentTarget = target;

        if (currentTarget == null)
        {
            interactText.enabled = false;
        }
        else
        {
            UpdateInteractText(currentTarget.DisplayName);
            interactText.enabled = true;
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
        playerUI._QuestInfoUI();
        questInfoPanel.UpdateQuestInfo(npc, quest);
    }

    public void OpenShopUI(NPCData data)
    {
        player.CanSellItems = true;
        interactText.enabled = false;

        foreach (InventorySlotData item in data.SlotData)
        {
            InventorySlotData rolledItem = item.item.ItemStatRules.BuildFixedItem(item);
            vendorInfoPanel.CreateItem(rolledItem);
        }

        playerUI._VendorUI();
    }

    public void CloseUI()
    {
        // Called From: QuestPanel, Decline Button, Turn-In Button, Dialogue Panel
        interactText.enabled = false;
        player.CanSellItems = false;
        player.IsInteracting = false;
        hasInteracted = true;
        vendorInfoPanel.RemoveItems();
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
            if (binding.groups.Contains(scheme)) return i;
        }

        return 0;
    }
}