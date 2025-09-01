using UnityEngine;
using UnityEngine.UI;

public class MiniMap : MonoBehaviour
{
    [SerializeField] Camera mapCamera;
    [SerializeField] RawImage map;
    [SerializeField] Image mapBorder;
    [SerializeField] Slider zoomSlider;
    [SerializeField] Slider alphaSlider;
    float minZoom = 11.25f;
    float maxZoom = 60f;

    private void Start()
    {
        // Set slider ranges
        zoomSlider.minValue = 0f;
        zoomSlider.maxValue = 1f;
        zoomSlider.value = .75f;

        alphaSlider.minValue = 0f;
        alphaSlider.maxValue = 1f;
        alphaSlider.value = .75f;

        // Hook up events
        zoomSlider.onValueChanged.AddListener(OnZoomChanged);
        alphaSlider.onValueChanged.AddListener(OnAlphaChanged);

        // Initialize
        OnZoomChanged(zoomSlider.value);
        OnAlphaChanged(alphaSlider.value);
    }

    private void OnZoomChanged(float value)
    {
        // Lerp between minZoom and maxZoom
        float zoom = Mathf.Lerp(minZoom, maxZoom, value);
        mapCamera.orthographicSize = zoom;
    }

    private void OnAlphaChanged(float value)
    {
        Color c = map.color;
        c.a = value;
        map.color = c;
        mapBorder.color = c;
    }
}
