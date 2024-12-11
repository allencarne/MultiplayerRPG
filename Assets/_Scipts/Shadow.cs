using UnityEngine;

public class Shadow : MonoBehaviour
{
    public SpriteRenderer parentSpriteRenderer; // Reference to the parent's SpriteRenderer
    private SpriteRenderer shadowSpriteRenderer;

    void Awake()
    {
        // Get the shadow's SpriteRenderer
        shadowSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        if (parentSpriteRenderer != null)
        {
            // Update shadow sprite to match the parent's sprite
            shadowSpriteRenderer.sprite = parentSpriteRenderer.sprite;

            // Match the parent's flip state for proper orientation
            shadowSpriteRenderer.flipX = parentSpriteRenderer.flipX;
            shadowSpriteRenderer.flipY = parentSpriteRenderer.flipY;
        }
    }
}
