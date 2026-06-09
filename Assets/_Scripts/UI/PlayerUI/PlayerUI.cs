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
            skillButtons.HandleInventory();
        }
        else
        {
            inventoryPanel.SetActive(true);
            inventoryPanel.transform.SetAsLastSibling();
            skillButtons.HandleInventory();
            if (UsingGamepad()) StartCoroutine(SelectNextFrame(inventoryFirstSelected));
        }
    }

    public void _SkillUI()
    {
        if (skillPanel.activeSelf)
        {
            skillPanel.SetActive(false);
            skillButtons.HandleAllSkills();
        }
        else
        {
            skillPanel.SetActive(true);
            skillPanel.transform.SetAsLastSibling();
            skillButtons.HandleAllSkills();
            if (UsingGamepad()) StartCoroutine(SelectNextFrame(skillFirstSelected));
        }
    }

    public void _AttributeUI()
    {
        if (attributePanel.activeSelf)
        {
            attributePanel.SetActive(false);
            skillButtons.HandleAttributes();
        }
        else
        {
            attributePanel.SetActive(true);
            attributePanel.transform.SetAsLastSibling();
            skillButtons.HandleAttributes();
            if (UsingGamepad()) StartCoroutine(SelectNextFrame(attributeFirstSelected));
        }
    }

    public void _QuestLogUI()
    {
        if (questLogPanel.activeSelf)
        {
            questLogPanel.SetActive(false);
        }
        else
        {
            questLogPanel.SetActive(true);
            questLogPanel.transform.SetAsLastSibling();
            if (UsingGamepad()) StartCoroutine(SelectNextFrame(questLogFirstSelected));
        }
    }

    public void _SettingsUI()
    {
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
        if (UsingGamepad()) StartCoroutine(SelectNextFrame(settingsfirstselected));
    }

    public void _InteractUI()
    {
        if (interactPanel.activeSelf)
        {
            interactPanel.SetActive(false);
            playerInteract.CloseUI();
        }
        else
        {
            interactPanel.SetActive(true);
            interactPanel.transform.SetAsLastSibling();
            if (UsingGamepad()) StartCoroutine(SelectNextFrame(interactfirstSelected));
        }
    }

    public void _QuestInfoUI()
    {
        if (questInfoPanel.activeSelf)
        {
            questInfoPanel.SetActive(false);
            playerInteract.CloseUI();
        }
        else
        {
            questInfoPanel.SetActive(true);
            questInfoPanel.transform.SetAsLastSibling();
            StartCoroutine(SelectNextFrame(questInfoFirstSelected));
        }
    }

    public void _VendorUI()
    {
        if (vendorPanel.activeSelf)
        {
            vendorPanel.SetActive(false);
            playerInteract.CloseUI();
        }
        else
        {
            vendorPanel.SetActive(true);
            vendorPanel.transform.SetAsLastSibling();
            if (UsingGamepad()) StartCoroutine(SelectNextFrame(vendorFirstSelected));
        }
    }

    public void _MapUI()
    {
        if (mapPanel.activeSelf)
        {
            mapPanel.SetActive(false);
        }
        else
        {
            mapPanel.SetActive(true);
            mapPanel.transform.SetAsLastSibling();
            if (UsingGamepad()) StartCoroutine(SelectNextFrame(mapFirstSelected));
        }
    }

    private bool UsingGamepad()
    {
        return playerInput != null && playerInput.currentControlScheme == "Gamepad";
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

    IEnumerator SelectNextFrame(GameObject target)
    {
        yield return null;
        EventSystem.current.SetSelectedGameObject(target);
    }
}
