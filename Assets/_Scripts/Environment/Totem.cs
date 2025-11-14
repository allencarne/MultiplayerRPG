using UnityEngine;

public class Totem : MonoBehaviour, IInteractable
{
    public string DisplayName => "Totem";

    public void Interact(PlayerInteract player)
    {
        Debug.Log("Interact With Totem");
        player.Activate();
    }
}
