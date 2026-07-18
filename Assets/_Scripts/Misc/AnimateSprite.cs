using System.Collections;
using UnityEngine;

public class AnimateSprite : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Sprite[] sprites;
    [SerializeField] float loopRate;

    int currentFrame = 0;

    private void Start()
    {
        InvokeRepeating("LoopSprites", 0, loopRate);
    }

    void LoopSprites()
    {
        // Checks if the current frame is equal to or larger than the sprite index
        // If it is, then reset the current fraom back to 0
        if (currentFrame >= sprites.Length) currentFrame = 0;

        // Set the sprite renderer to the current frame
        spriteRenderer.sprite = sprites[currentFrame];

        // Increase the current frame index
        currentFrame++;
    }
}
