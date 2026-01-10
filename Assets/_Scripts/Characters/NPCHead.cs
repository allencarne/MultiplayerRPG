using UnityEngine;

public class NPCHead : MonoBehaviour
{
    [SerializeField] NPC npc;
    [SerializeField] CharacterCustomizationData data;

    [SerializeField] SpriteRenderer eyesSprite;
    [SerializeField] SpriteRenderer hairSprite;
    [SerializeField] SpriteRenderer helmSprite;

    public void SetEyes(Vector2 direction)
    {
        int dirIndex;

        if (direction.y > 0) dirIndex = 2;        // Up
        else if (direction.y < 0) dirIndex = 0;   // Down
        else if (direction.x < 0) dirIndex = 1;   // Left
        else dirIndex = 3;                        // Right

        eyesSprite.sprite = data.eyes[npc.Data.eyeStyleIndex].sprites[dirIndex];
    }

    public void SetHair(Vector2 direction)
    {
        int dirIndex;

        if (direction.y > 0) dirIndex = 2;        // Up
        else if (direction.y < 0) dirIndex = 0;   // Down
        else if (direction.x < 0) dirIndex = 1;   // Left
        else dirIndex = 3;                        // Right

        hairSprite.sprite = data.hairs[npc.Data.hairStyleIndex].sprites[dirIndex];
    }

    public void SetHelm(Vector2 direction)
    {
        int dirIndex;

        if (direction.y > 0) dirIndex = 2;        // Up
        else if (direction.y < 0) dirIndex = 0;   // Down
        else if (direction.x < 0) dirIndex = 1;   // Left
        else dirIndex = 3;                        // Right

        helmSprite.sprite = data.helms[npc.Data.HelmIndex].sprites[dirIndex];
    }
}
