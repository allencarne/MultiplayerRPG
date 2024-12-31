using UnityEngine;
using TMPro;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI pickupText;
    [SerializeField] Item item;
    Player player;


    public void PickUp(Player player)
    {
        if (item.isCurrency)
        {
            player.CoinCollected(item.quantity);

            Destroy(gameObject);
            return;
        }

        // Add Item to Inventory if we have enough space
        bool wasPickedUp = player.inventory.AddItem(item);

        // Destroy item if it was collected
        if (wasPickedUp)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            pickupText.text = "Press <color=red>Z</color> To Pickup";
        }


        if (collision.CompareTag("Player"))
        {
            player = collision.gameObject.GetComponent<Player>();

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            pickupText.text = "";
        }
    }
}
