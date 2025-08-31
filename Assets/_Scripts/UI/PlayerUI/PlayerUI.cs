using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerUI : NetworkBehaviour
{
    [SerializeField] PlayerInput playerInput;

    [Header("FirstSelected")]
    [SerializeField] GameObject inventoryFirstSelected;
    [SerializeField] GameObject skillFirstSelected;
    [SerializeField] GameObject attributeFirstSelected;
    [SerializeField] GameObject questLogFirstSelected;
    [SerializeField] GameObject settingsfirstselected;

    [SerializeField] GameObject interactfirstSelected;
    [SerializeField] GameObject questInfoFirstSelected;

    [Header("Panel")]
    [SerializeField] GameObject inventoryPanel;
    [SerializeField] GameObject skillPanel;
    [SerializeField] GameObject attributePanel;
    [SerializeField] GameObject questLogPanel;
    [SerializeField] GameObject settingsPanel;

    [SerializeField] GameObject interactPanel;
    [SerializeField] GameObject questInfoPanel;

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
            if (UsingGamepad()) EventSystem.current.SetSelectedGameObject(inventoryFirstSelected);
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
            if (UsingGamepad()) EventSystem.current.SetSelectedGameObject(skillFirstSelected);
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
            if (UsingGamepad()) EventSystem.current.SetSelectedGameObject(attributeFirstSelected);
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
            if (UsingGamepad()) EventSystem.current.SetSelectedGameObject(questLogFirstSelected);
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

    public void _InteractUI()
    {
        if (interactPanel.activeSelf)
        {
            interactPanel.SetActive(false);
            UpdateSelectedUI();
        }
        else
        {
            interactPanel.SetActive(true);
            if (UsingGamepad()) EventSystem.current.SetSelectedGameObject(interactfirstSelected);
        }
    }

    public void _QuestInfoUI()
    {
        if (questInfoPanel.activeSelf)
        {
            questInfoPanel.SetActive(false);
            UpdateSelectedUI();
        }
        else
        {
            questInfoPanel.SetActive(true);
            if (UsingGamepad()) EventSystem.current.SetSelectedGameObject(questInfoFirstSelected);
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
            EventSystem.current.SetSelectedGameObject(inventoryFirstSelected);
        else if (skillPanel.activeSelf)
            EventSystem.current.SetSelectedGameObject(skillFirstSelected);
        else if (attributePanel.activeSelf)
            EventSystem.current.SetSelectedGameObject(attributeFirstSelected);
        else if (questLogPanel.activeSelf)
            EventSystem.current.SetSelectedGameObject(questLogFirstSelected);
        else if (settingsPanel.activeSelf)
            EventSystem.current.SetSelectedGameObject(settingsfirstselected);
        else if (interactPanel.activeSelf)
            EventSystem.current.SetSelectedGameObject(interactfirstSelected);
        else if (questInfoPanel.activeSelf)
            EventSystem.current.SetSelectedGameObject(questInfoFirstSelected);
        else
            EventSystem.current.SetSelectedGameObject(null);
    }
}
