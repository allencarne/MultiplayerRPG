using UnityEngine;

public class CharacterSelect : MonoBehaviour
{
    [SerializeField] GameObject CharacterCreatorPanel;

    private void Start()
    {
        bool character1Exists = PlayerPrefs.GetInt("Character1", 0) == 1;
        bool character2Exists = PlayerPrefs.GetInt("Character2", 0) == 1;
        bool character3Exists = PlayerPrefs.GetInt("Character3", 0) == 1;

        if (character1Exists)
        {
            Debug.Log("Character 1 Save Data found");
        }
        if (character2Exists)
        {
            Debug.Log("Character 2 Save Data found");
        }
        if (character3Exists)
        {
            Debug.Log("Character 3 Save Data found");
        }

        // If no character save data exists, enable the Character Creator Panel
        if (!character1Exists && !character2Exists && !character3Exists)
        {
            Debug.Log("No characters detected. Enabling Character Creator Panel.");
            CharacterCreatorPanel.SetActive(true);
        }
        else
        {
            Debug.Log("Existing characters found. Keeping Character Creator Panel disabled.");
            CharacterCreatorPanel.SetActive(false);
        }
    }
}