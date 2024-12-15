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

    private void Start()
    {
        // Closes all panels at the start
        HeroPanel.SetActive(false);
        InventoryPanel.SetActive(false);
        SkillPanel.SetActive(false);
        StatsPanel.SetActive(false);
        MapPanel.SetActive(false);
        SettingsPanel.SetActive(false);
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
}
