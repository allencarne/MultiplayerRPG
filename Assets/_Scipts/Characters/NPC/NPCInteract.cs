using UnityEngine;

public class NPCInteract : MonoBehaviour, IInteractable
{
    [TextArea(3,8)] public string[] Dialogue;
    [SerializeField] GameObject interactUI;
    [SerializeField] GameObject GuideUI;

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
        PlayerInputHandler playerInput = collision.GetComponent<PlayerInputHandler>();
        if (playerInput != null)
        {
            if (playerInput.InteractInput & !isInteracting)
            {
                isInteracting = true;
                Debug.Log("Press");

                interactUI.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            interactUI.SetActive(false);
            GuideUI.SetActive(false);
            isInteracting = false;
        }
    }
}
