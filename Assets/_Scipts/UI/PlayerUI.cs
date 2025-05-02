using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] GameObject HUD;

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
        CloseHeroPanel();
    }

    public void OpenCharacterUI(bool isMenuButton)
    {
        if (HeroPanel.activeSelf)
        {
            // If hero panel is enabled

            if (CharacterPanel.activeSelf)
            {
                if (isMenuButton)
                {
                    CloseHeroPanel();
                    HUD.SetActive(true);
                    EventSystem.current.SetSelectedGameObject(null);

                    input.SwitchCurrentActionMap("Player");
                }
            }
            else
            {
                HUD.SetActive(false);
                CloseHeroPanel();
                HeroPanel.SetActive(true);
                CharacterPanel.SetActive(true);
                characterButton.interactable = false;
                EventSystem.current.SetSelectedGameObject(closeButton.gameObject);

                input.SwitchCurrentActionMap("UI");
            }
        }
        else
        {
            // If hero panel is disabled

            HUD.SetActive(false);
            CloseHeroPanel();
            HeroPanel.SetActive(true);
            CharacterPanel.SetActive(true);
            characterButton.interactable = false;
            EventSystem.current.SetSelectedGameObject(closeButton.gameObject);

            input.SwitchCurrentActionMap("UI");
        }
    }

    public void OpenJournalUI(bool isMenuButton)
    {
        if (HeroPanel.activeSelf)
        {
            // If hero panel is enabled

            if (JournalPanel.activeSelf)
            {
                if (isMenuButton)
                {
                    CloseHeroPanel();
                    HUD.SetActive(true);
                    EventSystem.current.SetSelectedGameObject(null);

                    input.SwitchCurrentActionMap("Player");
                }
            }
            else
            {
                HUD.SetActive(false);
                CloseHeroPanel();
                HeroPanel.SetActive(true);
                JournalPanel.SetActive(true);
                journalButton.interactable = false;
                EventSystem.current.SetSelectedGameObject(closeButton.gameObject);

                input.SwitchCurrentActionMap("UI");
            }
        }
        else
        {
            // If hero panel is disabled

            HUD.SetActive(false);
            CloseHeroPanel();
            HeroPanel.SetActive(true);
            JournalPanel.SetActive(true);
            journalButton.interactable = false;
            EventSystem.current.SetSelectedGameObject(closeButton.gameObject);

            input.SwitchCurrentActionMap("UI");
        }
    }

    public void OpenSocialUI(bool isMenuButton)
    {
        if (HeroPanel.activeSelf)
        {
            // If hero panel is enabled

            if (SocialPanel.activeSelf)
            {
                if (isMenuButton)
                {
                    CloseHeroPanel();
                    HUD.SetActive(true);
                    EventSystem.current.SetSelectedGameObject(null);

                    input.SwitchCurrentActionMap("Player");
                }
            }
            else
            {
                HUD.SetActive(false);
                CloseHeroPanel();
                HeroPanel.SetActive(true);
                SocialPanel.SetActive(true);
                socialButton.interactable = false;
                EventSystem.current.SetSelectedGameObject(closeButton.gameObject);

                input.SwitchCurrentActionMap("UI");
            }
        }
        else
        {
            // If hero panel is disabled

            HUD.SetActive(false);
            CloseHeroPanel();
            HeroPanel.SetActive(true);
            SocialPanel.SetActive(true);
            socialButton.interactable = false;
            EventSystem.current.SetSelectedGameObject(closeButton.gameObject);

            input.SwitchCurrentActionMap("UI");
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
                    HUD.SetActive(true);
                    EventSystem.current.SetSelectedGameObject(null);

                    input.SwitchCurrentActionMap("Player");
                }
            }
            else
            {
                HUD.SetActive(false);
                CloseHeroPanel();
                HeroPanel.SetActive(true);
                MapPanel.SetActive(true);
                mapButton.interactable = false;
                EventSystem.current.SetSelectedGameObject(closeButton.gameObject);

                input.SwitchCurrentActionMap("UI");
            }
        }
        else
        {
            // If hero panel is disabled

            HUD.SetActive(false);
            CloseHeroPanel();
            HeroPanel.SetActive(true);
            MapPanel.SetActive(true);
            mapButton.interactable = false;
            EventSystem.current.SetSelectedGameObject(closeButton.gameObject);

            input.SwitchCurrentActionMap("UI");
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
                    HUD.SetActive(true);
                    EventSystem.current.SetSelectedGameObject(null);

                    input.SwitchCurrentActionMap("Player");
                }
            }
            else
            {
                CloseHeroPanel();
                HUD.SetActive(true);
                EventSystem.current.SetSelectedGameObject(null);

                input.SwitchCurrentActionMap("Player");
            }
        }
        else
        {
            // If hero panel is disabled

            HUD.SetActive(false);
            CloseHeroPanel();
            HeroPanel.SetActive(true);
            SettingsPanel.SetActive(true);
            settingsButton.interactable = false;
            EventSystem.current.SetSelectedGameObject(closeButton.gameObject);

            input.SwitchCurrentActionMap("UI");
        }
    }

    public void CloseButton()
    {
        CloseHeroPanel();
        HUD.SetActive(true);
        input.SwitchCurrentActionMap("Player");
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void CloseHeroPanel()
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
        mapButton.interactable= true;
        settingsButton.interactable = true;
    }
}
