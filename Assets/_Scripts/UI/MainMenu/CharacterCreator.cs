using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterCreator : MonoBehaviour
{
    [Header("Scriptable")]
    [SerializeField] CreatorNames names;
    [SerializeField] CharacterCustomizationData customizationData;

    [Header("Scripts")]
    [SerializeField] CharacterSelect characterSelect;
    [SerializeField] CharacterCreatorUI creatorUI;

    [Header("Text")]
    [SerializeField] TextMeshProUGUI feedbackText;

    [Header("Input")]
    [SerializeField] TMP_InputField nameInput;

    [Header("Index")]
    public int skinColorIndex = 0;
    public int hairStyleIndex = 0;
    public int hairColorIndex = 0;
    public int eyeStyleIndex = 0;
    public int eyeColorIndex = 0;

    [Header("Image")]
    [SerializeField] Image head;
    [SerializeField] Image body;
    [SerializeField] Image hair;
    [SerializeField] Image eyes;

    [Header("Buttons")]
    [SerializeField] Button randomButton;
    [SerializeField] Button continueButton;

    private void OnEnable()
    {
        nameInput.onValueChanged.AddListener(ValidateName);
    }

    private void OnDisable()
    {
        nameInput.onValueChanged.RemoveListener(ValidateName);
    }

    void Start()
    {
        EventSystem.current.SetSelectedGameObject(nameInput.gameObject);

        // Disable Continue Button
        continueButton.gameObject.SetActive(false);

        // Randomize Character
        RandomizeCharacter();
    }

    public void RandomizeCharacter()
    {
        skinColorIndex = Random.Range(0, customizationData.skinColors.Length);
        hairStyleIndex = Random.Range(0, customizationData.hairs.Count);
        hairColorIndex = Random.Range(0, customizationData.hairColors.Length);
        eyeStyleIndex = Random.Range(0, customizationData.eyes.Count);
        eyeColorIndex = Random.Range(0, customizationData.eyeColors.Length);

        GetRandomName();
        UpdateUI();
        creatorUI.UpdateSelectionImages();
    }

    void GetRandomName()
    {
        if (string.IsNullOrWhiteSpace(nameInput.text))
        {
            int randomNameIndex = Random.Range(0, names.randomNames.Length);
            nameInput.text = names.randomNames[randomNameIndex];

            // Run validation on the new name
            ValidateName(nameInput.text);
        }
    }

    public void UpdateUI()
    {
        head.color = customizationData.skinColors[skinColorIndex];
        body.color = customizationData.skinColors[skinColorIndex];
        hair.sprite = customizationData.hairs[hairStyleIndex].sprites[0];
        hair.color = customizationData.hairColors[hairColorIndex];

        eyes.sprite = customizationData.eyes[eyeStyleIndex].sprites[0];

        Material eyeMat = eyes.material;
        eyeMat.SetColor("_NewColor", customizationData.eyeColors[eyeColorIndex]);
    }

    public void ResetCreatorUI()
    {
        nameInput.text = "";
        feedbackText.text = "";

        skinColorIndex = 0;
        hairStyleIndex = 0;
        hairColorIndex = 0;

        head.color = customizationData.skinColors[0];
        body.color = customizationData.skinColors[0];
        hair.sprite = customizationData.hairs[0].sprites[0];
        hair.color = customizationData.hairColors[0];

        eyes.sprite = customizationData.eyes[0].sprites[0];

        Material eyeMat = eyes.material;
        eyeMat.SetColor("_NewColor", customizationData.eyeColors[0]);
    }

    void ValidateName(string name)
    {
        // Check for minimum length
        if (name.Length < 2)
        {
            feedbackText.text = "Name must be at least 2 characters long.";
            feedbackText.color = Color.red;
            continueButton.gameObject.SetActive(false);
            return;
        }

        // Check for profanity
        foreach (string bannedWord in names.bannedWords)
        {
            if (name.ToLower().Contains(bannedWord))
            {
                feedbackText.text = "Name contains prohibited words.";
                feedbackText.color = Color.red;
                continueButton.gameObject.SetActive(false);
                return;
            }
        }

        // If all checks pass
        feedbackText.text = "Name accepted.";
        feedbackText.color = Color.green;
        continueButton.gameObject.SetActive(true);
    }

    public void ContinueButton()
    {
        bool character1Exists = PlayerPrefs.GetInt("Character1") == 1;
        bool character2Exists = PlayerPrefs.GetInt("Character2") == 1;
        bool character3Exists = PlayerPrefs.GetInt("Character3") == 1;

        if (!character1Exists)
        {
            PlayerPrefs.SetString("Character1Name", nameInput.text);
            PlayerPrefs.SetInt("Character1SkinColor", skinColorIndex);
            PlayerPrefs.SetInt("Character1HairStyle", hairStyleIndex);
            PlayerPrefs.SetInt("Character1HairColor", hairColorIndex);
            PlayerPrefs.SetInt("Character1EyeStyle", eyeStyleIndex);
            PlayerPrefs.SetInt("Character1EyeColor", eyeColorIndex);

            PlayerPrefs.SetInt("Character1", 1);
            characterSelect.LoadCharacters();
            return;
        }
        if (!character2Exists)
        {
            PlayerPrefs.SetString("Character2Name", nameInput.text);
            PlayerPrefs.SetInt("Character2SkinColor", skinColorIndex);
            PlayerPrefs.SetInt("Character2HairStyle", hairStyleIndex);
            PlayerPrefs.SetInt("Character2HairColor", hairColorIndex);
            PlayerPrefs.SetInt("Character2EyeStyle", eyeStyleIndex);
            PlayerPrefs.SetInt("Character2EyeColor", eyeColorIndex);

            PlayerPrefs.SetInt("Character2", 1);
            characterSelect.LoadCharacters();
            return;
        }
        if (!character3Exists)
        {
            PlayerPrefs.SetString("Character3Name", nameInput.text);
            PlayerPrefs.SetInt("Character3SkinColor", skinColorIndex);
            PlayerPrefs.SetInt("Character3HairStyle", hairStyleIndex);
            PlayerPrefs.SetInt("Character3HairColor", hairColorIndex);
            PlayerPrefs.SetInt("Character3EyeStyle", eyeStyleIndex);
            PlayerPrefs.SetInt("Character3EyeColor", eyeColorIndex);

            PlayerPrefs.SetInt("Character3", 1);
            characterSelect.LoadCharacters();
            return;
        }
    }
}
