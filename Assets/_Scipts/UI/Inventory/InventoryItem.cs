using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    public Item item;
    Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }
}
