using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "ScriptableObjects/Inventory/Item")]
public class Item : ScriptableObject
{
    [Header("Inventory")]
    Inventory inventory;

    [Header("Item")]
    public GameObject prefab;

    [Header("Stats")]
    new public string name;
    public Sprite icon;
    public int cost;
    public int sellValue;

    [Header("Bools")]
    public bool isCurrency;

    [Header("Stack")]
    public bool isStackable;
    public int quantity;
}
