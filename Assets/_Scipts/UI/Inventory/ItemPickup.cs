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
                Debug.Log("Pickup Coin");

                player.coins += 1;

                Destroy(gameObject);
                return;
            }

            player.inventory.AddItem(item);
            Destroy(gameObject);
        }
    }
}
