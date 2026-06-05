using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] PlayerInput playerInput;
    [SerializeField] AttributeSkillButtons skillButtons;
    [SerializeField] PlayerInteract playerInteract;

    [Header("FirstSelected")]
    [SerializeField] GameObject inventoryFirstSelected;
    [SerializeField] GameObject skillFirstSelected;
    [SerializeField] GameObject attributeFirstSelected;
    [SerializeField] GameObject questLogFirstSelected;
    [SerializeField] GameObject settingsfirstselected;

    [SerializeField] GameObject interactfirstSelected;
    [SerializeField] GameObject questInfoFirstSelected;
    [SerializeField] GameObject vendorFirstSelected;
    [SerializeField] GameObject mapFirstSelected;

    [Header("Panel")]
    [SerializeField] GameObject inventoryPanel;
    [SerializeField] GameObject skillPanel;
    [SerializeField] GameObject attributePanel;
    [SerializeField] GameObject questLogPanel;
    [SerializeField] GameObject settingsPanel;

    [SerializeField] GameObject interactPanel;
    [SerializeField] GameObject questInfoPanel;
    [SerializeField] GameObject vendorPanel;
    [SerializeField] GameObject mapPanel;

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
            //UpdateSelectedUI();
            skillButtons.HandleInventory();
        }
        else
        {
            inventoryPanel.SetActive(true);
            inventoryPanel.transform.SetAsLastSibling();

            if (UsingGamepad()) EventSystem.current.SetSelectedGameObject(inventoryFirstSelected);
            skillButtons.HandleInventory();
        }
    }

    public void _SkillUI()
    {
        if (skillPanel.activeSelf)
        {
            skillPanel.SetActive(false);
            //UpdateSelectedUI();
            skillButtons.HandleAllSkills();
        }
        else
        {
            skillPanel.SetActive(true);
            skillPanel.transform.SetAsLastSibling();

            if (UsingGamepad()) EventSystem.current.SetSelectedGameObject(skillFirstSelected);
            skillButtons.HandleAllSkills();
        }
    }

    public void _AttributeUI()
    {
        if (attributePanel.activeSelf)
        {
            attributePanel.SetActive(false);
            //UpdateSelectedUI();
            skillButtons.HandleAttributes();
        }
        else
        {
            attributePanel.SetActive(true);
            attributePanel.transform.SetAsLastSibling();

            if (UsingGamepad()) EventSystem.current.SetSelectedGameObject(attributeFirstSelected);
            skillButtons.HandleAttributes();
        }
    }

    public void _QuestLogUI()
    {
        if (questLogPanel.activeSelf)
        {
            questLogPanel.SetActive(false);
            //UpdateSelectedUI();
        }
        else
        {
            questLogPanel.SetActive(true);
            questLogPanel.transform.SetAsLastSibling();

            if (UsingGamepad()) EventSystem.current.SetSelectedGameObject(questLogFirstSelected);
        }
    }

    public void _SettingsUI()
    {
        // Find the last active panel child (topmost visually)
        GameObject topmostPanel = GetTopmostActivePanel();

        if (topmostPanel != null)
        {
            if (topmostPanel == inventoryPanel) { _InventoryUI(); return; }
            if (topmostPanel == skillPanel) { _SkillUI(); return; }
            if (topmostPanel == attributePanel) { _AttributeUI(); return; }
            if (topmostPanel == questLogPanel) { _QuestLogUI(); return; }
            if (topmostPanel == settingsPanel) { settingsPanel.SetActive(false); return; }
            if (topmostPanel == interactPanel) { _InteractUI(); return; }
            if (topmostPanel == questInfoPanel) { _QuestInfoUI(); return; }
            if (topmostPanel == vendorPanel) { _VendorUI(); return; }
            if (topmostPanel == mapPanel) { _MapUI(); return; }
        }

        settingsPanel.SetActive(true);
        settingsPanel.transform.SetAsLastSibling();
        if (UsingGamepad()) EventSystem.current.SetSelectedGameObject(settingsfirstselected);
    }

    private GameObject GetTopmostActivePanel()
    {

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            GameObject child = transform.GetChild(i).gameObject;
            if (!IsManagedPanel(child)) continue;
            if (child.activeSelf) return child;
        }

        return null;
    }

    private bool IsManagedPanel(GameObject obj)
    {
        return obj == inventoryPanel || obj == skillPanel ||
               obj == attributePanel || obj == questLogPanel ||
               obj == settingsPanel || obj == interactPanel ||
               obj == questInfoPanel || obj == vendorPanel ||
               obj == mapPanel;
    }

    public void _InteractUI()
    {
        if (interactPanel.activeSelf)
        {
            interactPanel.SetActive(false);
            playerInteract.CloseUI();
            //UpdateSelectedUI();
        }
        else
        {
            interactPanel.SetActive(true);
            interactPanel.transform.SetAsLastSibling();

            if (UsingGamepad()) EventSystem.current.SetSelectedGameObject(interactfirstSelected);
        }
    }

    public void _QuestInfoUI()
    {
        if (questInfoPanel.activeSelf)
        {
            questInfoPanel.SetActive(false);
            playerInteract.CloseUI();
            //UpdateSelectedUI();
        }
        else
        {
            questInfoPanel.SetActive(true);
            questInfoPanel.transform.SetAsLastSibling();
            StartCoroutine(SelectNextFrame(questInfoFirstSelected));
        }
    }

    IEnumerator SelectNextFrame(GameObject target)
    {
        yield return null;
        EventSystem.current.SetSelectedGameObject(target);
    }

    public void _VendorUI()
    {
        if (vendorPanel.activeSelf)
        {
            vendorPanel.SetActive(false);
            playerInteract.CloseUI();
            //UpdateSelectedUI();
        }
        else
        {
            vendorPanel.SetActive(true);
            vendorPanel.transform.SetAsLastSibling();

            if (UsingGamepad()) EventSystem.current.SetSelectedGameObject(vendorFirstSelected);
        }
    }

    public void _MapUI()
    {
        if (mapPanel.activeSelf)
        {
            mapPanel.SetActive(false);
            //UpdateSelectedUI();
        }
        else
        {
            mapPanel.SetActive(true);
            mapPanel.transform.SetAsLastSibling();

            if (UsingGamepad()) EventSystem.current.SetSelectedGameObject(mapFirstSelected);
        }
    }

    private bool UsingGamepad()
    {
        return playerInput != null && playerInput.currentControlScheme == "Gamepad";
    }
}
