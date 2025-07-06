using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemList", menuName = "Scriptable Objects/ItemList")]
public class ItemList : ScriptableObject
{
    public List<Item> allItems;

    public Item GetItemByName(string name)
    {
        return allItems.Find(i => i.name == name);
    }
}
