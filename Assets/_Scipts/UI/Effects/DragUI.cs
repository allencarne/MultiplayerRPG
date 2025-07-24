using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragUI : MonoBehaviour, IDragHandler
{
    [SerializeField] private RectTransform rect;

    Vector3 dragOffset = new Vector3 (0, -7, 0);

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Convert the mouse position to world space and calculate the offset
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rect, eventData.position, Camera.main, out Vector3 worldPoint))
        {
            dragOffset = rect.position - worldPoint;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Convert the current mouse position to world space
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rect, eventData.position, Camera.main, out Vector3 worldPoint))
        {
            rect.position = worldPoint + dragOffset;
        }
    }
}