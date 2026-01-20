using UnityEditor.Experimental.GraphView;
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
    [SerializeField] float worldMapZoom = 125;

    Vector3 cameraPosition;
    float cameraZoom;

    private void OnEnable()
    {
        cameraPosition = mapCamera.transform.position;
        cameraZoom = mapCamera.orthographicSize;

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

        mapCamera.targetTexture = worldMapRenderTexture;
        mapCamera.transform.position = worldMapCameraPosition;
        mapCamera.orthographicSize = worldMapZoom;
    }

    public void EnableMiniMap()
    {
        cameraFollow.enabled = true;
        miniMap.SetActive(true);
        miniMap_m.SetActive(true);

        mapCamera.targetTexture = miniMapRenderTexture;
        mapCamera.transform.position = cameraPosition;
        mapCamera.orthographicSize = cameraZoom;
    }
}
