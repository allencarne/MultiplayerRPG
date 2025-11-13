using UnityEngine;

public class Totem : MonoBehaviour, IInteractable
{
    public void Interact(PlayerInteract player)
    {
        Debug.Log("Interact With Totem");
        player.Activate();
    }
}
