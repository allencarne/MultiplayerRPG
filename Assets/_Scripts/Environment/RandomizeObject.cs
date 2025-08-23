using UnityEngine;

public class RandomizeObject : MonoBehaviour
{
    [Header("Flip Settings")]
    public bool enableFlipX = false;
    public bool enableFlipY = false;

    [Header("Sprite Settings")]
    public bool enableRandomSprite = false;
    public Sprite[] sprites;

    [SerializeField] SpriteRenderer spriteRenderer;

    void Start()
    {
        // Randomly flip X and/or Y if enabled
        Vector3 localScale = transform.localScale;
        if (enableFlipX)
        {
            localScale.x *= Random.value > 0.5f ? -1 : 1;
        }
        if (enableFlipY)
        {
            localScale.y *= Random.value > 0.5f ? -1 : 1;
        }
        transform.localScale = localScale;

        // Randomly choose a sprite
        if (enableRandomSprite && sprites != null && sprites.Length > 0)
        {
            spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length)];
        }
    }
}
