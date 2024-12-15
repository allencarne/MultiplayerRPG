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
        }
        else
        {
            // If hero panel is disabled
        }


        TitleText.text = "Inventory";
        HeroPanel.SetActive(!HeroPanel.activeSelf);

        InventoryPanel.SetActive(!InventoryPanel.activeSelf);
        SkillPanel.SetActive(false);
        StatsPanel.SetActive(false);
        MapPanel.SetActive(false);
        SettingsPanel.SetActive(false);
    }

    public void OpenSkillsUI()
    {
        TitleText.text = "Skills";
        HeroPanel.SetActive(!HeroPanel.activeSelf);

        InventoryPanel.SetActive(false);
        SkillPanel.SetActive(!SkillPanel.activeSelf);
        StatsPanel.SetActive(false);
        MapPanel.SetActive(false);
        SettingsPanel.SetActive(false);
    }

    public void OpenStatsUI()
    {
        TitleText.text = "Stats";
        HeroPanel.SetActive(!HeroPanel.activeSelf);

        InventoryPanel.SetActive(false);
        SkillPanel.SetActive(false);
        StatsPanel.SetActive(!StatsPanel.activeSelf);
        MapPanel.SetActive(false);
        SettingsPanel.SetActive(false);
    }

    public void OpenMapUI()
    {
        TitleText.text = "Map";
        HeroPanel.SetActive(!HeroPanel.activeSelf);

        InventoryPanel.SetActive(false);
        SkillPanel.SetActive(false);
        StatsPanel.SetActive(false);
        MapPanel.SetActive(!MapPanel.activeSelf);
        SettingsPanel.SetActive(false);
    }

    public void OpenSettingsUI()
    {
        TitleText.text = "Settings";
        HeroPanel.SetActive(!HeroPanel.activeSelf);

        InventoryPanel.SetActive(false);
        SkillPanel.SetActive(false);
        StatsPanel.SetActive(false);
        MapPanel.SetActive(false);
        SettingsPanel.SetActive(!SettingsPanel.activeSelf);
    }
}
