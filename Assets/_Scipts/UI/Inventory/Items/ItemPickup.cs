using UnityEngine;
using TMPro;
using Unity.Netcode;

public class ItemPickup : NetworkBehaviour
{
    public Item Item;
    [SerializeField] GameObject toolTip;
    [SerializeField] TextMeshProUGUI pickupText;

    public void PickUp(Player player)
    {
        if (Item.IsCurrency)
        {
            player.CoinCollected(Item.Quantity);

            Destroy(gameObject);
            return;
        }

        // Add Item to Inventory if we have enough space
        bool wasPickedUp = player.inventory.AddItem(Item);

        // Destroy item if it was collected
        if (wasPickedUp)
        {
            Destroy(gameObject);
        }
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
