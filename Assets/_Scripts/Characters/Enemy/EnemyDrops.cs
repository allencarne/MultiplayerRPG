using Unity.Netcode;
using UnityEngine;

public class EnemyDrops : MonoBehaviour
{
    [SerializeField] Item[] droppableItems;

    public void DropItem()
    {
        if (droppableItems.Length == 0) return;

        foreach (Item item in droppableItems)
        {
            if (Random.Range(0f, 100f) < item.DropChance)
            {
                Vector2 randomPoint = (Vector2)transform.position + Random.insideUnitCircle * 1;
                GameObject drop = Instantiate(item.Prefab, randomPoint, Quaternion.identity);
                NetworkObject netItem = drop.GetComponent<NetworkObject>();
                netItem.Spawn();
            }
        }
    }
}
