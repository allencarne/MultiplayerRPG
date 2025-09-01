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
    private int steps = 4;

    private void Start()
    {
        zoomSlider.minValue = 0f;
        zoomSlider.maxValue = 1f;
        zoomSlider.value = .75f;

        alphaSlider.minValue = 0f;
        alphaSlider.maxValue = 1f;
        alphaSlider.value = .75f;

        zoomSlider.onValueChanged.AddListener(OnZoomChanged);
        alphaSlider.onValueChanged.AddListener(OnAlphaChanged);

        OnZoomChanged(zoomSlider.value);
        OnAlphaChanged(alphaSlider.value);
    }

    private void OnZoomChanged(float value)
    {
        float stepSize = 1f / steps;
        float snapped = Mathf.Round(value / stepSize) * stepSize;

        zoomSlider.SetValueWithoutNotify(snapped);

        float zoom = Mathf.Lerp(maxZoom, minZoom, snapped);
        mapCamera.orthographicSize = zoom;
    }

    private void OnAlphaChanged(float value)
    {
        float stepSize = 1f / steps;
        float snapped = Mathf.Round(value / stepSize) * stepSize;

        alphaSlider.SetValueWithoutNotify(snapped);

        Color c = map.color;
        c.a = snapped;
        map.color = c;
        mapBorder.color = c;
    }
}
