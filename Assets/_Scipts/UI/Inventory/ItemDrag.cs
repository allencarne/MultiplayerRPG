using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDrag : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] RectTransform rectTransform;
    [SerializeField] InventorySlot inventorySlot;
    Canvas canvas;
    CanvasGroup canvasGroup;

    private void Awake()
    {
        inventorySlot = GetComponentInParent<InventorySlot>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            (RectTransform)transform.parent, eventData.position, eventData.pressEventCamera, out Vector3 worldPoint);

        transform.position = worldPoint;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
    }
}
