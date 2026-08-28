using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Item/ItemList")]
public class ItemList : ScriptableObject
{
    public List<Item> allItems;

    public Item GetItemByName(string name)
    {
        return allItems.Find(i => i.name == name);
    }

    public Item GetItemById(FixedString64Bytes itemId)
    {
        return allItems.Find(i => i.ITEM_ID == itemId);
    }
}
