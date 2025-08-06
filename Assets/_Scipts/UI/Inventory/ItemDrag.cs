using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDrag : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public Image image;
    public InventorySlot inventorySlot;

    private Transform originalParent;
    private Canvas rootCanvas;

    private void Awake()
    {
        inventorySlot = GetComponentInParent<InventorySlot>();
        rootCanvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        image.raycastTarget = false;

        // Store the original parent and move to top-level canvas to render above all
        originalParent = image.transform.parent;
        image.transform.SetParent(rootCanvas.transform, true);

        // Set background image to solid black
        // Hide Text_Stack
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            (RectTransform)rootCanvas.transform, eventData.position, eventData.pressEventCamera, out Vector3 worldPoint);

        image.transform.position = worldPoint;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        image.raycastTarget = true;

        // Return to original parent
        image.transform.SetParent(originalParent, true);

        // Reset position and put it on top of siblings (front of UI)
        image.transform.localScale = Vector3.one;
        image.transform.localPosition = Vector3.zero;
        image.transform.SetAsFirstSibling();
    }
}
