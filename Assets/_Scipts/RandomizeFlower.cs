using UnityEngine;

public class RandomizeFlower : MonoBehaviour
{
    [Header("Flip Settings")]
    public bool enableFlipX = false;

    [SerializeField] SpriteRenderer stem;
    [SerializeField] SpriteRenderer flower;

    [SerializeField] Sprite[] stems;
    [SerializeField] Sprite[] flowers;

    [SerializeField] Color[] flowerColors;

    private void Start()
    {
        // Randomly flip X
        Vector3 localScale = transform.localScale;
        if (enableFlipX)
        {
            localScale.x *= Random.value > 0.5f ? -1 : 1;
        }
        transform.localScale = localScale;

        if (stems.Length == flowers.Length && stems.Length > 0)
        {
            int randomIndex = Random.Range(0, stems.Length);

            stem.sprite = stems[randomIndex];
            flower.sprite = flowers[randomIndex];
        }

        if (flowerColors != null && flowerColors.Length > 0)
        {
            int randomColor = Random.Range(0, flowerColors.Length);

            flower.color = flowerColors[randomColor];
        }
    }
}
