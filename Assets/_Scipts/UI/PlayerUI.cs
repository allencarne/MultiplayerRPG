using TMPro;
using UnityEngine;
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
                }
            }
            else
            {
                HUD.SetActive(false);
                CloseHeroPanel();
                HeroPanel.SetActive(true);
                CharacterPanel.SetActive(true);
                characterButton.interactable = false;
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
                }
            }
            else
            {
                HUD.SetActive(false);
                CloseHeroPanel();
                HeroPanel.SetActive(true);
                JournalPanel.SetActive(true);
                journalButton.interactable = false;
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
                }
            }
            else
            {
                HUD.SetActive(false);
                CloseHeroPanel();
                HeroPanel.SetActive(true);
                SocialPanel.SetActive(true);
                socialButton.interactable = false;
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
                }
            }
            else
            {
                HUD.SetActive(false);
                CloseHeroPanel();
                HeroPanel.SetActive(true);
                MapPanel.SetActive(true);
                mapButton.interactable = false;
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
                }
            }
            else
            {
                CloseHeroPanel();
                HUD.SetActive(true);
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
        }
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
