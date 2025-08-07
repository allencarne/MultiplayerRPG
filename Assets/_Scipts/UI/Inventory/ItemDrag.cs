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
    bool isDragging = false;
    private InventorySlot originSlot;

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

    private void Update()
    {
        GameObject selected = EventSystem.current.currentSelectedGameObject;

        if (isDragging)
        {
            GameObject current = EventSystem.current.currentSelectedGameObject;
            if (current != null)
            {
                image.transform.position = current.transform.position;
            }
        }

        if (selected != gameObject) return;

        // X button (PS: ☐ or Xbox: X)
        if (Input.GetKeyDown(KeyCode.JoystickButton2))
        {
            if (!isDragging)
            {
                BeginControllerDrag();
            }
            else
            {
                EndControllerDrag();
            }
        }
    }

    void BeginControllerDrag()
    {
        if (inventorySlot.slotData == null) return;

        isDragging = true;
        originSlot = inventorySlot;

        originalParent = image.transform.parent;
        image.transform.SetParent(rootCanvas.transform, true);
        image.raycastTarget = false;
    }

    void EndControllerDrag()
    {
        isDragging = false;
        image.raycastTarget = true;

        // Return image to slot
        image.transform.SetParent(originalParent, true);
        image.transform.localPosition = Vector3.zero;
        image.transform.SetAsFirstSibling();

        // Determine drop target
        GameObject targetGO = EventSystem.current.currentSelectedGameObject;
        if (targetGO == null) return;

        InventorySlot targetSlot = targetGO.GetComponent<InventorySlot>();
        if (targetSlot == null || targetSlot == originSlot) return;

        // Reuse the same drop logic
        targetSlot.HandleDropFrom(originSlot);
    }
}
