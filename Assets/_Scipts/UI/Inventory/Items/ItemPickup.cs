using UnityEngine;
using TMPro;
using Unity.Netcode;

public class ItemPickup : NetworkBehaviour
{
    public Item Item;
    [SerializeField] GameObject toolTip;
    [SerializeField] TextMeshProUGUI pickupText;
    bool _hasBeenPickedUp = false;

    public void PickUp(Player player)
    {
        // Prevent duplicate pickup
        if (_hasBeenPickedUp) return;
        _hasBeenPickedUp = true;

        if (Item is Currency)
        {
            player.CoinCollected(Item.Quantity);

            if (IsServer)
            {
                NetworkObject.Despawn(true);
            }
            else
            {
                DespawnServerRPC();
            }

            return;
        }

        // Add Item to Inventory if we have enough space
        bool wasPickedUp = player.PlayerInventory.AddItem(Item);

        // Destroy item if it was collected
        if (wasPickedUp)
        {
            if (IsServer)
            {
                NetworkObject.Despawn(true);
            }
            else
            {
                DespawnServerRPC();
            }
        }
        else
        {
            _hasBeenPickedUp = false;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void DespawnServerRPC()
    {
        NetworkObject.Despawn(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();

        if (collision.CompareTag("Player"))
        {
            if (player.IsLocalPlayer)
            {
                toolTip.SetActive(true);
                pickupText.text = "Press <color=red>Z</color> To Pickup";
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();

        if (collision.CompareTag("Player"))
        {
            if (player.IsLocalPlayer)
            {
                toolTip.SetActive(false);
                pickupText.text = "";
            }
        }
    }
}
