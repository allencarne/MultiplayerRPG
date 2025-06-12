using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerUI : NetworkBehaviour
{
    [SerializeField] GameObject HUD;
    [SerializeField] GameObject MobileHUD;

    [SerializeField] GameObject characterFirstSelect;

    [SerializeField] GameObject HeroPanel;
    [SerializeField] GameObject CharacterPanel;
    [SerializeField] GameObject JournalPanel;
    [SerializeField] GameObject SocialPanel;
    [SerializeField] GameObject MapPanel;
    [SerializeField] GameObject SettingsPanel;

    [SerializeField] Button characterButton;
    [SerializeField] Button journalButton;
    [SerializeField] Button socialButton;
    [SerializeField] Button mapButton;
    [SerializeField] Button settingsButton;
    [SerializeField] Button closeButton;

    [SerializeField] PlayerInput input;

    private void Start()
    {
        if (!IsOwner) return;

        CloseMenu();
    }

    public void OpenCharacterUI()
    {
        if (!IsOwner) return;

        if (HeroPanel.activeSelf && CharacterPanel.activeSelf)
        {
            CloseMenu();
            return;
        }

        EnableUI();
        CharacterPanel.SetActive(true);
        characterButton.interactable = false;
        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(characterFirstSelect);
    }

    public void OpenJournalUI()
    {
        if (!IsOwner) return;

        if (HeroPanel.activeSelf && JournalPanel.activeSelf)
        {
            CloseMenu();
            return;
        }

        EnableUI();
        JournalPanel.SetActive(true);
        journalButton.interactable = false;
        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(closeButton.gameObject);
    }

    public void OpenSocialUI()
    {
        if (!IsOwner) return;

        if (HeroPanel.activeSelf && SocialPanel.activeSelf)
        {
            CloseMenu();
            return;
        }

        EnableUI();
        SocialPanel.SetActive(true);
        socialButton.interactable = false;
        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(closeButton.gameObject);
    }

    public void OpenMapUI()
    {
        if (!IsOwner) return;

        if (HeroPanel.activeSelf && MapPanel.activeSelf)
        {
            CloseMenu();
            return;
        }

        EnableUI();
        MapPanel.SetActive(true);
        mapButton.interactable = false;
        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(closeButton.gameObject);
    }

    public void OpenSettingsUI()
    {
        if (!IsOwner) return;

        if (HeroPanel.activeSelf && SettingsPanel.activeSelf)
        {
            CloseMenu();
            return;
        }

        EnableUI();
        SettingsPanel.SetActive(true);
        settingsButton.interactable = false;
        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(closeButton.gameObject);
    }

    public void CloseButton()
    {
        if (!IsOwner) return;

        CloseMenu();
    }

    void EnableUI()
    {
        SetHUD(false);
        ClosePanels();
        HeroPanel.SetActive(true);
        input.SwitchCurrentActionMap("UI");
    }

    void CloseMenu()
    {
        if (!IsOwner) return;

        ClosePanels();
        SetHUD(true);
        EventSystem.current.SetSelectedGameObject(null);
        input.SwitchCurrentActionMap("Player");
    }

    void SetHUD(bool isMobile)
    {
        if (Application.isMobilePlatform)
        {
            MobileHUD.SetActive(isMobile);
        }
        else
        {
            HUD.SetActive(isMobile);
        }
    }

    void ClosePanels()
    {
        HeroPanel.SetActive(false);
        CharacterPanel.SetActive(false);
        JournalPanel.SetActive(false);
        SocialPanel.SetActive(false);
        MapPanel.SetActive(false);
        SettingsPanel.SetActive(false);
        characterButton.interactable = true;
        journalButton.interactable = true;
        socialButton.interactable = true;
        mapButton.interactable = true;
        settingsButton.interactable = true;
    }
}
