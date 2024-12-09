using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    [SerializeField] CharacterCreator characterCreator;
    [SerializeField] GameObject CharacterCreatorPanel;
    [SerializeField] CharacterCustomizationData customizationData;

    [Header("Character 1")]
    [SerializeField] TextMeshProUGUI Character1NameText;
    [SerializeField] Image body1;
    [SerializeField] Image hair1;

    [Header("Character 2")]
    [SerializeField] TextMeshProUGUI Character2NameText;
    [SerializeField] Image body2;
    [SerializeField] Image hair2;

    [Header("Character 3")]
    [SerializeField] TextMeshProUGUI Character3NameText;
    [SerializeField] Image body3;
    [SerializeField] Image hair3;

    private void Start()
    {
        bool character1Exists = PlayerPrefs.GetInt("Character1") == 1;
        bool character2Exists = PlayerPrefs.GetInt("Character2") == 1;
        bool character3Exists = PlayerPrefs.GetInt("Character3") == 1;

        if (!character1Exists && !character2Exists && !character3Exists)
        {
            CharacterCreatorPanel.SetActive(true);
        }

        LoadCharacters();
    }

    public void LoadCharacters()
    {
        bool character1Exists = PlayerPrefs.GetInt("Character1") == 1;
        bool character2Exists = PlayerPrefs.GetInt("Character2") == 1;
        bool character3Exists = PlayerPrefs.GetInt("Character3") == 1;

        if (character1Exists)
        {
            Character1NameText.text = PlayerPrefs.GetString("Character1Name");
            body1.color = customizationData.skinColors[PlayerPrefs.GetInt("Character1SkinColor")];
            hair1.sprite = customizationData.hairStyles[PlayerPrefs.GetInt("Character1HairStyle")];
            hair1.color = customizationData.hairColors[PlayerPrefs.GetInt("Character1HairColor")];
        }
        if (character2Exists)
        {
            Character2NameText.text = PlayerPrefs.GetString("Character2Name");
            body2.color = customizationData.skinColors[PlayerPrefs.GetInt("Character2SkinColor")];
            hair2.sprite = customizationData.hairStyles[PlayerPrefs.GetInt("Character2HairStyle")];
            hair2.color = customizationData.hairColors[PlayerPrefs.GetInt("Character2HairColor")];
        }
        if (character3Exists)
        {
            Character3NameText.text = PlayerPrefs.GetString("Character3Name");
            body3.color = customizationData.skinColors[PlayerPrefs.GetInt("Character3SkinColor")];
            hair3.sprite = customizationData.hairStyles[PlayerPrefs.GetInt("Character3HairStyle")];
            hair3.color = customizationData.hairColors[PlayerPrefs.GetInt("Character3HairColor")];
        }
    }

    private void Update()
    {
        Debug.Log(PlayerPrefs.GetInt("Character1", 0));
    }

    public void DeleteCharacter1()
    {
        PlayerPrefs.SetString("Character1Name", "Empty");
        PlayerPrefs.SetInt("Character1SkinColor", 0);
        PlayerPrefs.SetInt("Character1HairStyle", 0);
        PlayerPrefs.SetInt("Character1HairColor", 0);

        PlayerPrefs.SetInt("Character1", 0);

        Character1NameText.text = "Empty";
        body1.color = Color.white;
        hair1.sprite = customizationData.hairStyles[0];
        hair1.color = Color.black;
    }

    public void DeleteCharacter2()
    {
        PlayerPrefs.SetString("Character2Name", "Empty");
        PlayerPrefs.SetInt("Character2SkinColor", 0);
        PlayerPrefs.SetInt("Character2HairStyle", 0);
        PlayerPrefs.SetInt("Character2HairColor", 0);

        PlayerPrefs.SetInt("Character2", 0);

        Character2NameText.text = "Empty";
        body2.color = Color.white;
        hair2.sprite = customizationData.hairStyles[0];
        hair2.color = Color.black;
    }

    public void DeleteCharacter3()
    {
        PlayerPrefs.SetString("Character3Name", "Empty");
        PlayerPrefs.SetInt("Character3SkinColor", 0);
        PlayerPrefs.SetInt("Character3HairStyle", 0);
        PlayerPrefs.SetInt("Character3HairColor", 0);

        PlayerPrefs.SetInt("Character3", 0);

        Character3NameText.text = "Empty";
        body3.color = Color.white;
        hair3.sprite = customizationData.hairStyles[0];
        hair3.color = Color.black;
    }

    public void SelectCharacter1()
    {
        bool character1Exists = PlayerPrefs.GetInt("Character1") == 1;

        if (character1Exists)
        {
            // Tell the Player Script what Character number is selected
            PlayerPrefs.SetInt("SelectedCharacter", 1);

            SceneManager.LoadScene("Game");
        }
        else
        {
            characterCreator.ResetCreatorUI();
            CharacterCreatorPanel.SetActive(true);
        }
    }

    public void SelectCharacter2()
    {
        bool character2Exists = PlayerPrefs.GetInt("Character2") == 1;

        if (character2Exists)
        {
            // Tell the Player Script what Character number is selected
            PlayerPrefs.SetInt("SelectedCharacter", 2);

            SceneManager.LoadScene("Game");
        }
        else
        {
            characterCreator.ResetCreatorUI();
            CharacterCreatorPanel.SetActive(true);
        }
    }

    public void SelectCharacter3()
    {
        bool character3Exists = PlayerPrefs.GetInt("Character3") == 1;

        if (character3Exists)
        {
            // Tell the Player Script what Character number is selected
            PlayerPrefs.SetInt("SelectedCharacter", 3);

            SceneManager.LoadScene("Game");
        }
        else
        {
            characterCreator.ResetCreatorUI();
            CharacterCreatorPanel.SetActive(true);
        }
    }
}