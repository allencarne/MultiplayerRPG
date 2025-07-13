using UnityEngine;

public class NPCInteract : MonoBehaviour, IInteractable
{
    [TextArea(3,8)] public string[] Dialogue;


    public void Interact()
    {
        Debug.Log("interacted");
    }
}
