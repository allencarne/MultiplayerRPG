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

    public void OpenInventoryUI(bool isMenuButton)
    {
        if (HeroPanel.activeSelf)
        {
            // If hero panel is enabled

            if (InventoryPanel.activeSelf)
            {
                if (isMenuButton)
                {
                    CloseHeroPanel();
                }
            }
            else
            {
                TitleText.text = "Inventory";
                CloseHeroPanel();
                HeroPanel.SetActive(true);
                InventoryPanel.SetActive(true);
            }
        }
        else
        {
            // If hero panel is disabled

            TitleText.text = "Inventory";
            CloseHeroPanel();
            HeroPanel.SetActive(true);
            InventoryPanel.SetActive(true);
        }
    }

    public void OpenSkillsUI(bool isMenuButton)
    {
        if (HeroPanel.activeSelf)
        {
            // If hero panel is enabled

            if (SkillPanel.activeSelf)
            {
                if (isMenuButton)
                {
                    CloseHeroPanel();
                }
            }
            else
            {
                TitleText.text = "Skills";
                CloseHeroPanel();
                HeroPanel.SetActive(true);
                SkillPanel.SetActive(true);
            }
        }
        else
        {
            // If hero panel is disabled

            TitleText.text = "Skills";
            CloseHeroPanel();
            HeroPanel.SetActive(true);
            SkillPanel.SetActive(true);
        }
    }

    public void OpenStatsUI(bool isMenuButton)
    {
        if (HeroPanel.activeSelf)
        {
            // If hero panel is enabled

            if (StatsPanel.activeSelf)
            {
                if (isMenuButton)
                {
                    CloseHeroPanel();
                }
            }
            else
            {
                TitleText.text = "Stats";
                CloseHeroPanel();
                HeroPanel.SetActive(true);
                StatsPanel.SetActive(true);
            }
        }
        else
        {
            // If hero panel is disabled

            TitleText.text = "Stats";
            CloseHeroPanel();
            HeroPanel.SetActive(true);
            StatsPanel.SetActive(true);
        }
    }

    public void OpenMapUI(bool isMenuButton)
    {
        if (HeroPanel.activeSelf)
        {
            // If hero panel is enabled

            if (MapPanel.activeSelf)
            {
                if (isMenuButton)
                {
                    CloseHeroPanel();
                }
            }
            else
            {
                TitleText.text = "Map";
                CloseHeroPanel();
                HeroPanel.SetActive(true);
                MapPanel.SetActive(true);
            }
        }
        else
        {
            // If hero panel is disabled

            TitleText.text = "Map";
            CloseHeroPanel();
            HeroPanel.SetActive(true);
            MapPanel.SetActive(true);
        }
    }

    public void OpenSettingsUI(bool isMenuButton)
    {
        if (HeroPanel.activeSelf)
        {
            // If hero panel is enabled

            if (SettingsPanel.activeSelf)
            {
                if (isMenuButton)
                {
                    CloseHeroPanel();
                }
            }
            else
            {
                CloseHeroPanel();
            }
        }
        else
        {
            // If hero panel is disabled

            TitleText.text = "Settings";
            CloseHeroPanel();
            HeroPanel.SetActive(true);
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
