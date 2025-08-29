using Unity.Netcode;
using UnityEngine;

public class PlayerUI : NetworkBehaviour
{
    [Header("Panel")]
    [SerializeField] GameObject inventoryPanel;
    [SerializeField] GameObject skillPanel;
    [SerializeField] GameObject attributePanel;
    [SerializeField] GameObject questLogPanel;
    [SerializeField] GameObject settingsPanel;

    [Header("HUD")]
    [SerializeField] GameObject HUD;
    [SerializeField] GameObject MobileHUD;

    private void Start()
    {
        if (Application.isMobilePlatform)
        {
            HUD.SetActive(false);
            MobileHUD.SetActive(true);
        }
        else
        {
            HUD.SetActive(true);
            MobileHUD.SetActive(false);
        }
    }

    public void _InventoryUI()
    {
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
    }

    public void _SkillUI()
    {
        skillPanel.SetActive(!skillPanel.activeSelf);
    }

    public void _AttributeUI()
    {
        attributePanel.SetActive(!attributePanel.activeSelf);
    }

    public void _QuestLogUI()
    {
        questLogPanel.SetActive(!questLogPanel.activeSelf);
    }

    public void _SettingsUI()
    {
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }
}
