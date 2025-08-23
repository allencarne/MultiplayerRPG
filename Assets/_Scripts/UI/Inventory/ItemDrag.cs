using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDrag : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public Image image;
    public InventorySlot inventorySlot;

    private Transform originalParent;
    private Canvas rootCanvas;
    private bool canDrag = false;

    private void Awake()
    {
        inventorySlot = GetComponentInParent<InventorySlot>();
        rootCanvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        canDrag = inventorySlot.slotData != null && inventorySlot.slotData.item != null;

        if (!canDrag)
            return;

        image.raycastTarget = false;
        originalParent = image.transform.parent;
        image.transform.SetParent(rootCanvas.transform, true);

        // Set background image to solid black
        // Hide Text_Stack
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!canDrag)
            return;

        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            (RectTransform)rootCanvas.transform, eventData.position, eventData.pressEventCamera, out Vector3 worldPoint);

        image.transform.position = worldPoint;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!canDrag)
            return;

        image.raycastTarget = true;
        image.transform.SetParent(originalParent, true);
        image.transform.localScale = Vector3.one;
        image.transform.localPosition = Vector3.zero;
        image.transform.SetAsFirstSibling();

        canDrag = false;
    }
}
