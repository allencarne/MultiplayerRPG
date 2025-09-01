using UnityEngine;

public class UIZoom : MonoBehaviour
{
    [SerializeField] RectTransform uiPanel;
    private Vector3 panelScale;
    private float scaleStep = 0.2f;
    private float minScale = 1.0f;
    private float maxScale = 1.8f;
    string panelId;
    bool isMobile;

    private void Start()
    {
        panelId = gameObject.name;
        isMobile = Application.isMobilePlatform;

        float defaultScale = 1.0f;

        // If we're on mobile and there's no saved scale yet, use 1.4 instead of 1.0
        if (isMobile && !PlayerPrefs.HasKey($"UIPanelScale_{panelId}"))
        {
            defaultScale = 1.6f;
        }

        // Load the saved scale (or fallback to defaultScale)
        float savedScale = PlayerPrefs.GetFloat($"UIPanelScale_{panelId}", defaultScale);
        savedScale = Mathf.Clamp(savedScale, minScale, maxScale);

        panelScale = new Vector3(savedScale, savedScale, 1);
        uiPanel.localScale = panelScale;
    }

    public void IncreasePanelSize()
    {
        panelScale += Vector3.one * scaleStep;
        panelScale = ClampScale(panelScale);
        uiPanel.localScale = panelScale;
        SaveScale();
    }

    public void DecreasePanelSize()
    {
        panelScale -= Vector3.one * scaleStep;
        panelScale = ClampScale(panelScale);
        uiPanel.localScale = panelScale;
        SaveScale();
    }

    private Vector3 ClampScale(Vector3 scale)
    {
        scale.x = Mathf.Clamp(scale.x, minScale, maxScale);
        scale.y = Mathf.Clamp(scale.y, minScale, maxScale);
        scale.z = 1;
        return scale;
    }

    private void SaveScale()
    {
        PlayerPrefs.SetFloat($"UIPanelScale_{panelId}", panelScale.x);
        PlayerPrefs.Save();
    }
}
