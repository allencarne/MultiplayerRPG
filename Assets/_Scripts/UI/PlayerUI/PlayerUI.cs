using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerUI : NetworkBehaviour
{
    [SerializeField] PlayerInput playerInput;

    [Header("FirstSelected")]
    [SerializeField] GameObject inventoryfirstselected;
    [SerializeField] GameObject skillfirstselected;
    [SerializeField] GameObject attributefirstselected;
    [SerializeField] GameObject questfirstselected;
    [SerializeField] GameObject settingsfirstselected;

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
        if (inventoryPanel.activeSelf)
        {
            inventoryPanel.SetActive(false);
            UpdateSelectedUI();
        }
        else
        {
            inventoryPanel.SetActive(true);
            if (UsingGamepad()) EventSystem.current.SetSelectedGameObject(inventoryfirstselected);
        }
    }

    public void _SkillUI()
    {
        if (skillPanel.activeSelf)
        {
            skillPanel.SetActive(false);
            UpdateSelectedUI();
        }
        else
        {
            skillPanel.SetActive(true);
            if (UsingGamepad()) EventSystem.current.SetSelectedGameObject(skillfirstselected);
        }
    }

    public void _AttributeUI()
    {
        if (attributePanel.activeSelf)
        {
            attributePanel.SetActive(false);
            UpdateSelectedUI();
        }
        else
        {
            attributePanel.SetActive(true);
            if (UsingGamepad()) EventSystem.current.SetSelectedGameObject(attributefirstselected);
        }
    }

    public void _QuestLogUI()
    {
        if (questLogPanel.activeSelf)
        {
            questLogPanel.SetActive(false);
            UpdateSelectedUI();
        }
        else
        {
            questLogPanel.SetActive(true);
            if (UsingGamepad()) EventSystem.current.SetSelectedGameObject(questfirstselected);
        }
    }

    public void _SettingsUI()
    {
        if (settingsPanel.activeSelf)
        {
            settingsPanel.SetActive(false);
            UpdateSelectedUI();
        }
        else
        {
            settingsPanel.SetActive(true);
            if (UsingGamepad()) EventSystem.current.SetSelectedGameObject(settingsfirstselected);
        }
    }

    private bool UsingGamepad()
    {
        return playerInput != null && playerInput.currentControlScheme == "Gamepad";
    }

    void UpdateSelectedUI()
    {
        if (!UsingGamepad())
            return;

        if (inventoryPanel.activeSelf)
            EventSystem.current.SetSelectedGameObject(inventoryfirstselected);
        else if (skillPanel.activeSelf)
            EventSystem.current.SetSelectedGameObject(skillfirstselected);
        else if (attributePanel.activeSelf)
            EventSystem.current.SetSelectedGameObject(attributefirstselected);
        else if (questLogPanel.activeSelf)
            EventSystem.current.SetSelectedGameObject(questfirstselected);
        else if (settingsPanel.activeSelf)
            EventSystem.current.SetSelectedGameObject(settingsfirstselected);
        else
            EventSystem.current.SetSelectedGameObject(null);
    }
}
