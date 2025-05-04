using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCreator : MonoBehaviour
{
    string[] bannedWords = { "nigger", "nig", "niger", "bitch", "fuck", "ass", "nigga", "niga", "shit", "nazi, slut, cunt, whore"};

    [SerializeField] CharacterSelect characterSelect;
    [SerializeField] CharacterCustomizationData customizationData;

    [SerializeField] TMP_InputField nameInput;
    [SerializeField] TextMeshProUGUI feedbackText;

    [SerializeField] TextMeshProUGUI skinColorText;
    [SerializeField] TextMeshProUGUI hairStyleText;
    [SerializeField] TextMeshProUGUI hairColorText;

    int currentSkinIndex = 0;
    int currentHairStyleIndex = 0;
    int currentHairColorIndex = 0;

    [SerializeField] Image body;
    [SerializeField] Image hair;

    [SerializeField] Button randomButton;
    [SerializeField] Button continueButton;

    void Start()
    {
        // Disable Continue Button
        continueButton.interactable = false;

        // UI
        UpdateUI();

        // Add listener
        nameInput.onValueChanged.AddListener(ValidateName);
        randomButton.onClick.AddListener(RandomizeCharacter);
    }

    public void CycleSkinColor(bool next)
    {
        currentSkinIndex = CycleIndex(currentSkinIndex, customizationData.skinColors.Length, next);
        UpdateUI();
    }

    public void CycleHairStyle(bool next)
    {
        currentHairStyleIndex = CycleIndex(currentHairStyleIndex, customizationData.hairStyles.Length, next);
        UpdateUI();
    }

    public void CycleHairColor(bool next)
    {
        currentHairColorIndex = CycleIndex(currentHairColorIndex, customizationData.hairColors.Length, next);
        UpdateUI();
    }

    private int CycleIndex(int currentIndex, int arrayLength, bool next)
    {
        if (next)
            return (currentIndex + 1) % arrayLength;
        else
            return (currentIndex - 1 + arrayLength) % arrayLength;
    }

    public void RandomizeCharacter()
    {
        currentSkinIndex = Random.Range(0, customizationData.skinColors.Length);
        currentHairStyleIndex = Random.Range(0, customizationData.hairStyles.Length);
        currentHairColorIndex = Random.Range(0, customizationData.hairColors.Length);
        UpdateUI();
    }

    private void UpdateUI()
    {
        skinColorText.text = $"{currentSkinIndex}";
        hairStyleText.text = $"{currentHairStyleIndex}";
        hairColorText.text = $"{currentHairColorIndex}";

        body.color = customizationData.skinColors[currentSkinIndex];
        hair.sprite = customizationData.hairStyles[currentHairStyleIndex];
        hair.color = customizationData.hairColors[currentHairColorIndex];
    }

    public void ResetCreatorUI()
    {
        nameInput.text = "";
        feedbackText.text = "";

        currentSkinIndex = 0;
        currentHairStyleIndex = 0;
        currentHairColorIndex = 0;

        skinColorText.text = $"{currentSkinIndex}";
        hairStyleText.text = $"{currentHairStyleIndex}";
        hairColorText.text = $"{currentHairColorIndex}";

        body.color = customizationData.skinColors[0];
        hair.sprite = customizationData.hairStyles[0];
        hair.color = customizationData.hairColors[0];
    }

    void ValidateName(string name)
    {
        // Check for minimum length
        if (name.Length < 3)
        {
            feedbackText.text = "Name must be at least 3 characters long.";
            feedbackText.color = Color.red;
            continueButton.interactable = false;
            return;
        }

        // Check for profanity
        foreach (string bannedWord in bannedWords)
        {
            if (name.ToLower().Contains(bannedWord))
            {
                feedbackText.text = "Name contains prohibited words.";
                feedbackText.color = Color.red;
                continueButton.interactable = false;
                return;
            }
        }

        // If all checks pass
        feedbackText.text = "Name accepted.";
        feedbackText.color = Color.green;
        continueButton.interactable = true;
    }

    void OnDestroy()
    {
        // Remove listener to avoid memory leaks
        nameInput.onValueChanged.RemoveListener(ValidateName);
    }

    public void ContinueButton()
    {
        bool character1Exists = PlayerPrefs.GetInt("Character1") == 1;
        bool character2Exists = PlayerPrefs.GetInt("Character2") == 1;
        bool character3Exists = PlayerPrefs.GetInt("Character3") == 1;

        if (!character1Exists)
        {
            PlayerPrefs.SetString("Character1Name", nameInput.text);
            PlayerPrefs.SetInt("Character1SkinColor", currentSkinIndex);
            PlayerPrefs.SetInt("Character1HairStyle", currentHairStyleIndex);
            PlayerPrefs.SetInt("Character1HairColor", currentHairColorIndex);

            PlayerPrefs.SetInt("Character1", 1);
            characterSelect.LoadCharacters();
            return;
        }
        if (!character2Exists)
        {
            PlayerPrefs.SetString("Character2Name", nameInput.text);
            PlayerPrefs.SetInt("Character2SkinColor", currentSkinIndex);
            PlayerPrefs.SetInt("Character2HairStyle", currentHairStyleIndex);
            PlayerPrefs.SetInt("Character2HairColor", currentHairColorIndex);

            PlayerPrefs.SetInt("Character2", 1);
            characterSelect.LoadCharacters();
            return;
        }
        if (!character3Exists)
        {
            PlayerPrefs.SetString("Character3Name", nameInput.text);
            PlayerPrefs.SetInt("Character3SkinColor", currentSkinIndex);
            PlayerPrefs.SetInt("Character3HairStyle", currentHairStyleIndex);
            PlayerPrefs.SetInt("Character3HairColor", currentHairColorIndex);

            PlayerPrefs.SetInt("Character3", 1);
            characterSelect.LoadCharacters();
            return;
        }
    }
}
