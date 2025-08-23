using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject parent; // The parent GameObject (e.g., Equipment)
    [SerializeField] GameObject EquipmentPanel; // The Equipment Panel
    [SerializeField] GameObject InventoryPanel; // The Inventory Panel

    [SerializeField] EquipmentSlot equipmentSlot; // The Equipment Slot reference
    [SerializeField] GameObject toolTip; // Tooltip GameObject

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (toolTip != null && equipmentSlot.Item != null)
        {
            // Ensure InventoryPanel is above EquipmentPanel in the hierarchy
            SwapPanelsOrder();

            // Set the parent object as the last child of EquipmentPanel
            SetParentAsLastChild();

            // Enable the tooltip
            toolTip.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (toolTip != null)
        {
            // Disable the tooltip
            toolTip.SetActive(false);

            SwapPanelsOrder();
        }
    }

    private void SwapPanelsOrder()
    {
        Transform parentTransform = EquipmentPanel.transform.parent;

        if (parentTransform != null)
        {
            // Get the current sibling indices
            int equipmentIndex = EquipmentPanel.transform.GetSiblingIndex();
            int inventoryIndex = InventoryPanel.transform.GetSiblingIndex();

            // Swap their indices
            EquipmentPanel.transform.SetSiblingIndex(inventoryIndex);
            InventoryPanel.transform.SetSiblingIndex(equipmentIndex);
        }
    }

        private void SetParentAsLastChild()
    {
        if (parent != null && EquipmentPanel != null)
        {
            // Set parent as the last child of EquipmentPanel
            parent.transform.SetParent(EquipmentPanel.transform);

            // Move parent to the end of the child list
            parent.transform.SetAsLastSibling();
        }
    }
}
