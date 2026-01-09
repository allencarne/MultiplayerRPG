using TMPro;
using UnityEngine;

public class CharacterStatPreview : MonoBehaviour
{
    [SerializeField] int playerIndex;
    [SerializeField] TextMeshProUGUI statText;

    private void Start()
    {
        int level = PlayerPrefs.GetInt($"{playerIndex}PlayerLevel", 1);
        int classInxed = PlayerPrefs.GetInt($"{playerIndex}PlayerClass");

        switch (classInxed)
        {
            case 0:
                statText.text = $"Level: {level} \n <sprite name=\"Beginner_Icon\">  Beginner";
                break;
            case 1:
                statText.text = $"Level: {level} \n <sprite name=\"Warrior_Icon\">  Warrior";
                break;
            case 2:
                statText.text = $"Level: {level} \n <sprite name=\"Magician_Icon\">  Magician";
                break;
            case 3:
                statText.text = $"Level: {level} \n <sprite name=\"Archer_Icon\">  Archer";
                break;
            case 4:
                statText.text = $"Level: {level} \n <sprite name=\"Rogue_Icon\">  Rogue";
                break;
        }
    }
}
