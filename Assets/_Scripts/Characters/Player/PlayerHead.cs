using UnityEngine;

public class PlayerHead : MonoBehaviour
{
    [SerializeField] CharacterCustomizationData data;
    [SerializeField] PlayerCustomization custom;

    public void SetHead(Vector2 direction)
    {
        //custom.net_FacingDirection.Value = direction;

        SetEyes(direction);
        SetHair(direction);
        SetHelm(direction);
    }

    void SetEyes(Vector2 direction)
    {
        int dirIndex;

        if (direction.y > 0) dirIndex = 2;        // Up
        else if (direction.y < 0) dirIndex = 0;   // Down
        else if (direction.x < 0) dirIndex = 1;   // Left
        else dirIndex = 3;                        // Right

        custom.eyesSprite.sprite = data.eyes[custom.net_EyeIndex.Value].sprites[dirIndex];
    }

    void SetHair(Vector2 direction)
    {
        int dirIndex;

        if (direction.y > 0) dirIndex = 2;        // Up
        else if (direction.y < 0) dirIndex = 0;   // Down
        else if (direction.x < 0) dirIndex = 1;   // Left
        else dirIndex = 3;                        // Right

        custom.hairSprite.sprite = data.hairs[custom.net_HairIndex.Value].sprites[dirIndex];
    }

    void SetHelm(Vector2 direction)
    {
        int dirIndex;

        if (direction.y > 0) dirIndex = 2;        // Up
        else if (direction.y < 0) dirIndex = 0;   // Down
        else if (direction.x < 0) dirIndex = 1;   // Left
        else dirIndex = 3;                        // Right

        custom.helmSprite.sprite = data.helms[custom.net_HeadIndex.Value].sprites[dirIndex];
    }
}
