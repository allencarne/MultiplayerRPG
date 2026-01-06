using UnityEngine;

public class PlayerHead : MonoBehaviour
{
    [SerializeField] CharacterCustomizationData data;
    [SerializeField] PlayerCustomization custom;

    public void SetEyes(Vector2 direction)
    {
        int dirIndex;

        if (direction.y > 0) dirIndex = 2;        // Up
        else if (direction.y < 0) dirIndex = 0;   // Down
        else if (direction.x < 0) dirIndex = 1;   // Left
        else dirIndex = 3;                        // Right

        switch (custom.net_EyeIndex.Value)
        {
            case 0: custom.eyesSprite.sprite = data.Eye0[dirIndex]; break;
            case 1: custom.eyesSprite.sprite = data.Eye1[dirIndex]; break;
            case 2: custom.eyesSprite.sprite = data.Eye2[dirIndex]; break;
        }
    }

    public void SetHair(Vector2 direction)
    {
        int dirIndex;

        if (direction.y > 0) dirIndex = 2;        // Up
        else if (direction.y < 0) dirIndex = 0;   // Down
        else if (direction.x < 0) dirIndex = 1;   // Left
        else dirIndex = 3;                        // Right

        switch (custom.net_HairIndex.Value)
        {
            case 0: custom.playerHair.sprite = data.Hair0[dirIndex]; break;
            case 1: custom.playerHair.sprite = data.Hair1[dirIndex]; break;
            case 2: custom.playerHair.sprite = data.Hair2[dirIndex]; break;
        }
    }
}
