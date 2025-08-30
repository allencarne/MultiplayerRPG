using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerUI : NetworkBehaviour
{
    [SerializeField] PlayerInput playerInput;
    [SerializeField] GameObject inventoryfirstselected;

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

        if (inventoryPanel.activeInHierarchy && UsingGamepad())
        {
            EventSystem.current.SetSelectedGameObject(inventoryfirstselected);
        }

        if (!inventoryPanel.activeInHierarchy) EventSystem.current.SetSelectedGameObject(null);
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

    private bool UsingGamepad()
    {
        // PlayerInput keeps track of control scheme
        return playerInput != null && playerInput.currentControlScheme == "Gamepad";
    }
}
