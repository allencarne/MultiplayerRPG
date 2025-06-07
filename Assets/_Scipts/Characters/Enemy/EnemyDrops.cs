using UnityEngine;

public class EnemyDrops : MonoBehaviour
{
    [SerializeField] Item[] droppableItems;
    Item selectedItem;
    float dropRadius = 1;

    public void DropItem()
    {
        if (droppableItems.Length == 0) return;

        for (int i = 0; i < droppableItems.Length; i++)
        {
            Item selectedItem = droppableItems[i];
            int randomChance = Random.Range(0, 101);

            if (randomChance <= selectedItem.DropChance)
            {
                Vector2 randomPoint = (Vector2)transform.position + Random.insideUnitCircle * dropRadius;
                Instantiate(selectedItem.Prefab, randomPoint, Quaternion.identity);
            }
        }
    }
}
