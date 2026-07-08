using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDrag : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    // References
    [SerializeField] ContextMenu contextMenu;
    public InventorySlot inventorySlot;

    // UI
    private Transform originalParent;
    private Canvas rootCanvas;
    public bool canDrag = false;
    public RectTransform itemVisual;

    private void Awake()
    {
        inventorySlot = GetComponentInParent<InventorySlot>();
        rootCanvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Only allow left mouse button to start dragging
        if (eventData.button != PointerEventData.InputButton.Left) return;

        // We can only drag if this slot contains a valid item
        canDrag = inventorySlot.slotData != null && inventorySlot.slotData.item != null;

        // Stop immediately if there isn't an item to drag
        if (!canDrag) return;

        // Remember where the item came from so it can be restored later
        originalParent = itemVisual.parent;

        // Move the item visual to the root canvas
        // This prevents it from being clipped by its inventory slot and ensures
        // it renders on top of the rest of the UI while dragging
        itemVisual.SetParent(rootCanvas.transform, true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Ignore drag events if dragging isn't allowed
        if (!canDrag) return;

        // Convert the mouse position on the screen into a position within the canvas
        RectTransformUtility.ScreenPointToWorldPointInRectangle((RectTransform)rootCanvas.transform, eventData.position, eventData.pressEventCamera, out Vector3 worldPoint);

        // Move the item visual so it follows the mouse cursor
        itemVisual.position = worldPoint;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Ignore if we never started dragging
        if (!canDrag) return;

        // Put the item visual back into its original inventory slot
        itemVisual.SetParent(originalParent, true);


        // Restore its normal scale in case any parent scaling affected it
        itemVisual.localScale = Vector3.one;

        // Reset its local position so it sits exactly where it belongs
        itemVisual.localPosition = Vector3.zero;

        // Ensure it renders behind other UI elements inside the slot
        // (such as the border or selection highlight)
        itemVisual.SetAsFirstSibling();

        // Dragging has finished
        canDrag = false;

        // If the item wasn't dropped onto another valid UI element,
        // treat it as being dropped into the world
        if (eventData.pointerEnter == null || !IsOverBlockedUI(eventData.pointerEnter))
        {
            contextMenu._DropButton();
        }
    }

    bool IsOverBlockedUI(GameObject obj)
    {
        // Start with the object currently under the mouse
        Transform t = obj.transform;

        // Walk up through all of its parent objects
        while (t != null)
        {
            // If any object in the hierarchy has the BlockInputUI tag,
            // we consider this a valid UI area that blocks dropping
            if (t.CompareTag("BlockInputUI")) return true;

            // Move up one level in the hierarchy
            t = t.parent;
        }

        // No blocking UI was found
        return false;
    }
}
