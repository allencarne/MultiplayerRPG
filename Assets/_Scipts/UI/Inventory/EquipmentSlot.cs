using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour
{
    public int index;
    [SerializeField] EquipmentManager equipmentManager;
    public Image icon;
    public Item Item;

    public void AddItem(Item newItem)
    {
        Item = newItem;

        icon.sprite = Item.Icon;
        icon.enabled = true;
    }

    public void ClearSlot()
    {
        Item = null;

        icon.sprite = null;
        icon.enabled = false;
    }

    public void UseItem()
    {
        if (Item != null)
        {
            equipmentManager.UnEquip(index);
        }
    }
}
