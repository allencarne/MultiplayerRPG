using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI TitleText;

    [SerializeField] GameObject HeroPanel;
    [SerializeField] GameObject InventoryPanel;
    [SerializeField] GameObject SkillPanel;
    [SerializeField] GameObject StatsPanel;
    [SerializeField] GameObject MapPanel;
    [SerializeField] GameObject SettingsPanel;

    [SerializeField] RectTransform uiPanel;
    private Vector3 panelScale;
    private float scaleStep = 0.2f;
    private float minScale = 1.0f;
    private float maxScale = 1.8f;

    private void Start()
    {
        // Load the saved scale or default to (1, 1, 1)
        float savedScale = PlayerPrefs.GetFloat("UIPanelScale", 1.0f);
        savedScale = Mathf.Clamp(savedScale, minScale, maxScale);
        panelScale = new Vector3(savedScale, savedScale, 1);
        uiPanel.localScale = panelScale;

        // Closes all panels at the start
        CloseHeroPanel();
    }

    public void OpenInventoryUI()
    {
        if (HeroPanel.activeSelf)
        {
            // If hero panel is enabled

            if (InventoryPanel.activeSelf)
            {
                CloseHeroPanel();
            }
            else
            {
                TitleText.text = "Inventory";
                HeroPanel.SetActive(true);

                InventoryPanel.SetActive(true);
                SkillPanel.SetActive(false);
                StatsPanel.SetActive(false);
                MapPanel.SetActive(false);
                SettingsPanel.SetActive(false);
            }
        }
        else
        {
            // If hero panel is disabled

            TitleText.text = "Inventory";
            HeroPanel.SetActive(true);

            InventoryPanel.SetActive(true);
            SkillPanel.SetActive(false);
            StatsPanel.SetActive(false);
            MapPanel.SetActive(false);
            SettingsPanel.SetActive(false);
        }
    }

    public void OpenSkillsUI()
    {
        if (HeroPanel.activeSelf)
        {
            // If hero panel is enabled

            if (SkillPanel.activeSelf)
            {
                CloseHeroPanel();
            }
            else
            {
                TitleText.text = "Skills";
                HeroPanel.SetActive(true);

                InventoryPanel.SetActive(false);
                SkillPanel.SetActive(true);
                StatsPanel.SetActive(false);
                MapPanel.SetActive(false);
                SettingsPanel.SetActive(false);
            }
        }
        else
        {
            // If hero panel is disabled

            TitleText.text = "Skills";
            HeroPanel.SetActive(true);

            InventoryPanel.SetActive(false);
            SkillPanel.SetActive(true);
            StatsPanel.SetActive(false);
            MapPanel.SetActive(false);
            SettingsPanel.SetActive(false);
        }
    }

    public void OpenStatsUI()
    {
        if (HeroPanel.activeSelf)
        {
            // If hero panel is enabled

            if (StatsPanel.activeSelf)
            {
                CloseHeroPanel();
            }
            else
            {
                TitleText.text = "Stats";
                HeroPanel.SetActive(true);

                InventoryPanel.SetActive(false);
                SkillPanel.SetActive(false);
                StatsPanel.SetActive(true);
                MapPanel.SetActive(false);
                SettingsPanel.SetActive(false);
            }
        }
        else
        {
            // If hero panel is disabled

            TitleText.text = "Stats";
            HeroPanel.SetActive(true);

            InventoryPanel.SetActive(false);
            SkillPanel.SetActive(false);
            StatsPanel.SetActive(true);
            MapPanel.SetActive(false);
            SettingsPanel.SetActive(false);
        }
    }

    public void OpenMapUI()
    {
        if (HeroPanel.activeSelf)
        {
            // If hero panel is enabled

            if (MapPanel.activeSelf)
            {
                CloseHeroPanel();
            }
            else
            {
                TitleText.text = "Map";
                HeroPanel.SetActive(true);

                InventoryPanel.SetActive(false);
                SkillPanel.SetActive(false);
                StatsPanel.SetActive(false);
                MapPanel.SetActive(true);
                SettingsPanel.SetActive(false);
            }
        }
        else
        {
            // If hero panel is disabled

            TitleText.text = "Map";
            HeroPanel.SetActive(true);

            InventoryPanel.SetActive(false);
            SkillPanel.SetActive(false);
            StatsPanel.SetActive(false);
            MapPanel.SetActive(true);
            SettingsPanel.SetActive(false);
        }
    }

    public void OpenSettingsUI()
    {
        if (HeroPanel.activeSelf)
        {
            // If hero panel is enabled

            if (SettingsPanel.activeSelf)
            {
                CloseHeroPanel();
            }
            else
            {
                TitleText.text = "Settings";
                HeroPanel.SetActive(true);

                InventoryPanel.SetActive(false);
                SkillPanel.SetActive(false);
                StatsPanel.SetActive(false);
                MapPanel.SetActive(false);
                SettingsPanel.SetActive(true);
            }
        }
        else
        {
            // If hero panel is disabled

            TitleText.text = "Settings";
            HeroPanel.SetActive(true);

            InventoryPanel.SetActive(false);
            SkillPanel.SetActive(false);
            StatsPanel.SetActive(false);
            MapPanel.SetActive(false);
            SettingsPanel.SetActive(true);
        }
    }

    public void CloseHeroPanel()
    {
        HeroPanel.SetActive(false);

        InventoryPanel.SetActive(false);
        SkillPanel.SetActive(false);
        StatsPanel.SetActive(false);
        MapPanel.SetActive(false);
        SettingsPanel.SetActive(false);
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
        PlayerPrefs.SetFloat("UIPanelScale", panelScale.x);
        PlayerPrefs.Save();
    }
}
