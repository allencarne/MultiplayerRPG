using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class CameraZoom : MonoBehaviour
{
    private List<RaycastResult> raycastResults = new List<RaycastResult>();
    public PlayerInputHandler inputHandler;
    private Camera mainCamera;

    float zoomStep = 1f;
    float minZoom = 5.25f;
    float maxZoom = 15.25f;

    private void Start()
    {
        mainCamera = GetComponent<Camera>();
    }

    public void GetPlayer()
    {
        inputHandler.ZoomPerformed += HandleZoom;
    }

    private void OnDestroy()
    {
        if (inputHandler != null)
        {
            inputHandler.ZoomPerformed -= HandleZoom;
        }
    }

    private void HandleZoom(Vector2 zoomInput)
    {
        if (IsPointerOverScrollView()) return;

        // Use the y-axis (vertical scroll) to determine zoom direction
        float zoomDirection = zoomInput.y;

        if (zoomDirection != 0)
        {
            // Adjust the camera's orthographic size by zoomStep
            float newZoom = mainCamera.orthographicSize - Mathf.Sign(zoomDirection) * zoomStep;

            // Clamp the size between minZoom and maxZoom
            mainCamera.orthographicSize = Mathf.Clamp(newZoom, minZoom, maxZoom);
        }
    }

    private bool IsPointerOverScrollView()
    {
        if (EventSystem.current == null)
            return false;

        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        raycastResults.Clear();
        EventSystem.current.RaycastAll(eventData, raycastResults);

        foreach (RaycastResult result in raycastResults)
        {
            if (result.gameObject.CompareTag("BlockInputUI"))
            {
                return true;
            }
        }

        return false;
    }
}
