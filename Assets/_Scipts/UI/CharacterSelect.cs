using UnityEngine;

public class CharacterSelect : MonoBehaviour
{
    [SerializeField] GameObject CharacterCratorPanel;

    private void Start()
    {
        // if we do not have an existing character
            // Enable Creator Panel


        // Testing
        CharacterCratorPanel.SetActive(true);
    }
}
