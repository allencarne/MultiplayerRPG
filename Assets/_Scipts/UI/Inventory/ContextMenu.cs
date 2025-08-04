using UnityEngine;

public class ContextMenu : MonoBehaviour
{
    [SerializeField] GameObject contextMenu;
    [SerializeField] InventorySlot inventorySlot;
    [SerializeField] ItemToolTip tooltip;

    public void UseButton()
    {
        inventorySlot.UseItem();
        contextMenu.SetActive(false);
    }

    public void MoveButton()
    {

    }

    public void SplitButton()
    {

    }

    public void DropButton()
    {

    }
}
