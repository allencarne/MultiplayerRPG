using Unity.Netcode;
using UnityEngine;

public class EnemyDrops : MonoBehaviour
{
    [SerializeField] EnemyData enemyData;

    public void DropItem()
    {
        // If there are no droppable items, exit the method
        if (enemyData.DroppableItems.Length == 0) return;

        // Calculate the rarity boost based on the enemy type
        float boost = enemyData.Scaling.GetRarityBoost(enemyData.Enemy_Type);

        foreach (Item item in enemyData.DroppableItems)
        {
            // Roll for the item drop based on its drop chance
            if (Random.Range(0f, 100f) < item.DropChance)
            {
                // Generate a random point around the enemy's position to drop the item
                Vector2 randomPoint = (Vector2)transform.position + Random.insideUnitCircle * 1.5f;

                // Instantiate the item prefab at the random point with no rotation
                GameObject drop = Instantiate(item.Prefab, randomPoint, Quaternion.identity);

                // Get the NetworkObject component from the dropped item
                NetworkObject netItem = drop.GetComponent<NetworkObject>();

                // Spawn the item on the network so that all clients can see it
                netItem.Spawn();

                // Get the ItemStatGenerator component from the dropped item
                ItemStatGenerator generator = drop.GetComponent<ItemStatGenerator>();

                // Roll the stats for the item with the calculated rarity boost
                generator.RollStats(boost);

                // If the item drops in bulk, calculate the quantity based on the enemy's level and type
                if (item.DropsInBulk)
                {
                    generator.net_Quantity.Value = enemyData.Scaling.RollCurrencyAmount(enemyData.Enemy_Level, enemyData.Enemy_Type);
                }
            }
        }
    }
}
