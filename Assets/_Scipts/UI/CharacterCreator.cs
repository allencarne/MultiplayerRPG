using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCreator : MonoBehaviour
{
    private string[] bannedWords = { "nigger", "nig", "niger", "bitch", "fuck", "ass", "nigga", "niga", "shit", "nazi"};

    [SerializeField] TMP_InputField nameInput;
    [SerializeField] TextMeshProUGUI feedbackText;

    [SerializeField] Button skinColorL;
    [SerializeField] Button skinColorR;
    [SerializeField] TextMeshProUGUI skinColorText;

    [SerializeField] Button hairStyleL;
    [SerializeField] Button hairStyleR;
    [SerializeField] TextMeshProUGUI hairStyleText;

    [SerializeField] Button hairColorL;
    [SerializeField] Button hairColorR;
    [SerializeField] TextMeshProUGUI hairColorText;

    [SerializeField] Button RandomButton;
    [SerializeField] Button continueButton;

    private void Start()
    {
        continueButton.interactable = false;

        // Add listener to call ValidateName as the player types
        nameInput.onValueChanged.AddListener(ValidateName);
    }

    private void ValidateName(string name)
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

    private void OnDestroy()
    {
        // Remove listener to avoid memory leaks
        nameInput.onValueChanged.RemoveListener(ValidateName);
    }

    public void ContinueButton()
    {
        Debug.Log("Continue button pressed with valid name: " + nameInput.text);
    }
}
