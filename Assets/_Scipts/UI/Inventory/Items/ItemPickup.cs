using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] Item item;
    Player player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.gameObject.GetComponent<Player>();

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
    }
}
