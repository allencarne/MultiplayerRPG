using UnityEngine;
using UnityEngine.UI;

public class WorldMap : MonoBehaviour
{
    [SerializeField] Camera mapCamera;
    [SerializeField] RawImage map;
    [SerializeField] CameraFollow cameraFollow;
    [SerializeField] GameObject miniMap;
    [SerializeField] GameObject miniMap_m;

    [Header("World Map Settings")]
    [SerializeField] RenderTexture worldMapRenderTexture;
    [SerializeField] RenderTexture miniMapRenderTexture;
    [SerializeField] Vector3 worldMapCameraPosition = new Vector3(200, 40, -10);

    [Header("Zoom Settings")]
    [SerializeField] float worldMapZoom = 125;
    [SerializeField] Vector2 baseImageSize = new Vector2(640, 360);
    RectTransform mapRectTransform;
    private float currentZoomLevel = 1f;

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
        cameraFollow.enabled = false;
        miniMap.SetActive(false);
        miniMap_m.SetActive(false);

        mapCamera.transform.SetParent(null);

        mapCamera.targetTexture = worldMapRenderTexture;
        mapCamera.transform.position = worldMapCameraPosition;
        mapCamera.orthographicSize = worldMapZoom;

        SetZoomLevel(1f);
    }

    public void EnableMiniMap()
    {
        mapCamera.transform.SetParent(originalParent);

        cameraFollow.enabled = true;
        miniMap.SetActive(true);
        miniMap_m.SetActive(true);

        mapCamera.targetTexture = miniMapRenderTexture;
        mapCamera.transform.position = cameraPosition;
        mapCamera.orthographicSize = cameraZoom;
    }

    public void SetZoom50()
    {
        SetZoomLevel(0.5f);
    }

    public void SetZoom75()
    {
        SetZoomLevel(0.75f);
    }

    public void SetZoom100()
    {
        SetZoomLevel(1f);
    }

    public void SetZoom125()
    {
        SetZoomLevel(1.25f);
    }

    public void SetZoom150()
    {
        SetZoomLevel(1.50f);
    }

    private void SetZoomLevel(float zoomLevel)
    {
        currentZoomLevel = zoomLevel;
        Vector2 newSize = baseImageSize * zoomLevel;
        mapRectTransform.sizeDelta = newSize;
    }
}