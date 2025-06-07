using UnityEngine;

public class EnemyDrops : MonoBehaviour
{
    [SerializeField] Item[] droppableItems;
    Item selectedItem;

    public void DropItem()
    {
        if (droppableItems.Length == 0) return;

        int randomItem = Random.Range(0, droppableItems.Length);
        Item selectedItem = droppableItems[randomItem];
        int randomChance = Random.Range(0, 101);

        if (randomChance <= selectedItem.DropChance)
        {
            // Spawn the item
            Instantiate(selectedItem.Prefab, transform.position, Quaternion.identity);
        }
    }
}
