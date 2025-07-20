using TMPro;
using Unity.Services.Matchmaker.Models;
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

    [SerializeField] PlayerInputHandler input;
    [SerializeField] PlayerInput playerInput;
    [SerializeField] InputActionReference interactAction;
    bool isInteracting = false;

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
        if (!collision.CompareTag("NPC")) return;
        interactText.enabled = true;
        UpdateInteractText(collision.name);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("NPC")) return;
        NPC npc = collision.GetComponent<NPC>();

        if (input.InteractInput)
        {
            if (isInteracting) return;
            isInteracting = true;

            interactText.enabled = false;
            playerInput.SwitchCurrentActionMap("UI");
            SetupInteractUI(npc.name, npc.type);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("NPC")) return;
        interactText.enabled = false;
    }

    void SetupInteractUI(string name, NPC.Type type)
    {
        // Name
        npcNameText.text = name;

        // Enable Buttons based on NPC Type
        switch (type)
        {
            case NPC.Type.Quest:
                dialogueButton.gameObject.SetActive(true);
                questButton.gameObject.SetActive(true);
                shopButton.gameObject.SetActive(false);
                backButton.gameObject.SetActive(true);

                SetNavigation(dialogueButton, backButton, questButton);
                SetNavigation(questButton, dialogueButton, backButton);
                SetNavigation(backButton, questButton, dialogueButton);

                break;
            case NPC.Type.Vendor:
                dialogueButton.gameObject.SetActive(true);
                questButton.gameObject.SetActive(false);
                shopButton.gameObject.SetActive(true);
                backButton.gameObject.SetActive(true);

                SetNavigation(dialogueButton, backButton, shopButton);
                SetNavigation(shopButton, dialogueButton, backButton);
                SetNavigation(backButton, shopButton, dialogueButton);

                break;
            case NPC.Type.Villager:
                dialogueButton.gameObject.SetActive(true);
                questButton.gameObject.SetActive(false);
                shopButton.gameObject.SetActive(false);
                backButton.gameObject.SetActive(true);

                SetNavigation(dialogueButton, backButton, backButton);
                SetNavigation(backButton, dialogueButton, dialogueButton);

                break;
            case NPC.Type.Guard:
                dialogueButton.gameObject.SetActive(true);
                questButton.gameObject.SetActive(false);
                shopButton.gameObject.SetActive(false);
                backButton.gameObject.SetActive(true);

                SetNavigation(dialogueButton, backButton, backButton);
                SetNavigation(backButton, dialogueButton, dialogueButton);

                break;
            case NPC.Type.Patrol:
                dialogueButton.gameObject.SetActive(true);
                questButton.gameObject.SetActive(false);
                shopButton.gameObject.SetActive(false);
                backButton.gameObject.SetActive(true);

                SetNavigation(dialogueButton, backButton, backButton);
                SetNavigation(backButton, dialogueButton, dialogueButton);

                break;
        }

        // Set first selected
        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(dialogueButton.gameObject);

        // Enable UI
        interactUI.SetActive(true);
    }

    private void SetNavigation(Button button, Button selectOnUp, Button selectOnDown)
    {
        var nav = button.navigation;
        nav.mode = Navigation.Mode.Explicit;
        nav.selectOnUp = selectOnUp;
        nav.selectOnDown = selectOnDown;
        button.navigation = nav;
    }
}
