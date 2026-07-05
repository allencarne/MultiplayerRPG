using Unity.Netcode;
using UnityEngine;

public class EnemyDrops : MonoBehaviour
{
    [SerializeField] EnemyData enemyData;

    public void DropItem()
    {
        if (enemyData.DroppableItems.Length == 0) return;

        foreach (Item item in enemyData.DroppableItems)
        {
            if (Random.Range(0f, 100f) < item.DropChance)
            {
                Vector2 randomPoint = (Vector2)transform.position + Random.insideUnitCircle * 1;
                GameObject drop = Instantiate(item.Prefab, randomPoint, Quaternion.identity);
                NetworkObject netItem = drop.GetComponent<NetworkObject>();
                netItem.Spawn();
                drop.GetComponent<ItemStatGenerator>().RollStats();
            }
        }
    }
}
