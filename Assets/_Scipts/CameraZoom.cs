using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public float zoomSpeed = 2f;
    public float minZoom = 5f;
    public float maxZoom = 20f;

    public PlayerInputHandler inputHandler;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = GetComponent<Camera>();

        inputHandler.ZoomPerformed += HandleZoom;
    }

    private void OnDestroy()
    {
        // Unsubscribe from the OnZoom action to avoid memory leaks
        if (inputHandler != null)
        {
            inputHandler.ZoomPerformed -= HandleZoom;
        }
    }

    private void HandleZoom(Vector2 zoomInput)
    {
        // Use the y-axis (vertical scroll) to determine zoom direction
        float zoomDirection = zoomInput.y;

        if (zoomDirection != 0)
        {
            // Adjust the camera's orthographic size
            mainCamera.orthographicSize -= zoomDirection * zoomSpeed * Time.deltaTime;

            // Clamp the size between minZoom and maxZoom
            mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize, minZoom, maxZoom);

            // Debug log the current size
            Debug.Log("Current Zoom Level: " + mainCamera.orthographicSize);
        }
    }
}
