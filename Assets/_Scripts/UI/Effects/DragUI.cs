using UnityEngine;
using UnityEngine.EventSystems;

public class DragUI : MonoBehaviour, IDragHandler
{
    [SerializeField] RectTransform rect;
    [SerializeField] Canvas canvas;

    public void OnDrag(PointerEventData eventData)
    {
        rect.anchoredPosition += eventData.delta / canvas.scaleFactor;
        ClampToScreen();
    }

    void ClampToScreen()
    {
        Vector3[] corners = new Vector3[4];
        rect.GetWorldCorners(corners);

        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        // Get canvas bounds in world space
        Vector3[] canvasCorners = new Vector3[4];
        canvasRect.GetWorldCorners(canvasCorners);

        Rect canvasBounds = new Rect(
            canvasCorners[0].x,
            canvasCorners[0].y,
            canvasCorners[2].x - canvasCorners[0].x,
            canvasCorners[2].y - canvasCorners[0].y
        );

        // Calculate current bounds of the UI element
        Rect uiBounds = new Rect(
            corners[0].x,
            corners[0].y,
            corners[2].x - corners[0].x,
            corners[2].y - corners[0].y
        );

        // Calculate offset needed to clamp
        Vector2 offset = Vector2.zero;

        if (uiBounds.xMin < canvasBounds.xMin)
            offset.x = canvasBounds.xMin - uiBounds.xMin;
        else if (uiBounds.xMax > canvasBounds.xMax)
            offset.x = canvasBounds.xMax - uiBounds.xMax;

        if (uiBounds.yMin < canvasBounds.yMin)
            offset.y = canvasBounds.yMin - uiBounds.yMin;
        else if (uiBounds.yMax > canvasBounds.yMax)
            offset.y = canvasBounds.yMax - uiBounds.yMax;

        rect.position += (Vector3)offset;
    }
}