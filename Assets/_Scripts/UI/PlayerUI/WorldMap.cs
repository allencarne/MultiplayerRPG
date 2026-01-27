using UnityEngine;
using UnityEngine.UI;

public class WorldMap : MonoBehaviour
{
    [SerializeField] Transform playerTransform;
    [SerializeField] ScrollRect scrollRect;

    [SerializeField] Camera mapCamera;
    [SerializeField] RawImage map;
    [SerializeField] GameObject miniMap;
    [SerializeField] GameObject miniMap_m;

    [SerializeField] RenderTexture worldMapRenderTexture;
    [SerializeField] RenderTexture miniMapRenderTexture;

    Vector3 worldMapCameraPosition = new Vector3(200, 40, -10);
    float worldMapZoom = 125;
    Vector2 baseImageSize = new Vector2(640, 360);

    RectTransform mapRectTransform;
    Vector3 cameraPosition;
    float cameraZoom;
    Transform originalParent;

    private void Awake()
    {
        mapRectTransform = map.GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        cameraPosition = mapCamera.transform.position;
        cameraZoom = mapCamera.orthographicSize;
        originalParent = mapCamera.transform.parent;

        EnableWorldMap();
    }

    private void OnDisable()
    {
        EnableMiniMap();
    }

    public void EnableWorldMap()
    {
        miniMap.SetActive(false);
        miniMap_m.SetActive(false);

        mapCamera.transform.SetParent(null);

        mapCamera.targetTexture = worldMapRenderTexture;
        mapCamera.transform.position = worldMapCameraPosition;
        mapCamera.orthographicSize = worldMapZoom;

        CenterOnPlayer();
    }

    public void EnableMiniMap()
    {
        mapCamera.transform.SetParent(originalParent);

        miniMap.SetActive(true);
        miniMap_m.SetActive(true);

        mapCamera.targetTexture = miniMapRenderTexture;
        mapCamera.transform.position = cameraPosition;
        mapCamera.orthographicSize = cameraZoom;

        mapCamera.transform.localPosition = new Vector3(0, 0, -10);
    }

    public void SetZoom50()
    {
        SetZoomLevel(0.5f);
        CenterOnPlayer();
    }

    public void SetZoom75()
    {
        SetZoomLevel(0.75f);
        CenterOnPlayer();
    }

    public void SetZoom100()
    {
        SetZoomLevel(1f);
        CenterOnPlayer();
    }

    public void SetZoom125()
    {
        SetZoomLevel(1.25f);
        CenterOnPlayer();
    }

    public void SetZoom150()
    {
        SetZoomLevel(1.50f);
        CenterOnPlayer();
    }

    private void SetZoomLevel(float zoomLevel)
    {
        Vector2 newSize = baseImageSize * zoomLevel;
        mapRectTransform.sizeDelta = newSize;
    }

    private void CenterOnPlayer()
    {
        if (playerTransform == null || scrollRect == null) return;

        // Get player's world position
        Vector3 playerWorldPos = playerTransform.position;

        // Convert player world position to viewport position on the map camera
        Vector3 viewportPos = mapCamera.WorldToViewportPoint(playerWorldPos);

        // Clamp to valid range
        viewportPos.x = Mathf.Clamp01(viewportPos.x);
        viewportPos.y = Mathf.Clamp01(viewportPos.y);

        // Set scroll position (note: viewport coordinates match scroll normalized position)
        scrollRect.normalizedPosition = new Vector2(viewportPos.x, viewportPos.y);
    }
}