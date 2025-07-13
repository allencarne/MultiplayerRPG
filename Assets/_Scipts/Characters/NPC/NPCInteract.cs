using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class NPCInteract : MonoBehaviour, IInteractable
{
    [TextArea(3,8)] public string[] Dialogue;
    [SerializeField] GameObject interactUI;
    [SerializeField] GameObject GuideUI;
    [SerializeField] GameObject firstSelected;

    bool isInteracting;

    public void Interact()
    {
        Debug.Log("interacted");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GuideUI.SetActive(true);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        PlayerInputHandler inputHandler = collision.GetComponent<PlayerInputHandler>();
        PlayerInput playerInput = collision.GetComponent<PlayerInput>();

        if (inputHandler != null && playerInput != null)
        {
            if (inputHandler.InteractInput & !isInteracting)
            {
                isInteracting = true;
                interactUI.SetActive(true);
                GuideUI.SetActive(false);
                playerInput.SwitchCurrentActionMap("UI");
                if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(firstSelected);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        PlayerInput playerInput = collision.GetComponent<PlayerInput>();


        if (collision.CompareTag("Player"))
        {
            if (playerInput != null)
            {
                playerInput.SwitchCurrentActionMap("Player");
                interactUI.SetActive(false);
                GuideUI.SetActive(false);
                isInteracting = false;
                if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
            }
        }
    }
}
