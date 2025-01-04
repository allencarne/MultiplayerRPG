using UnityEngine;

public class RandomizeTree : MonoBehaviour
{
    [Header("Flip Settings")]
    public bool enableFlipX = false;

    [SerializeField] GameObject grass;
    [SerializeField] GameObject coconut;

    private void Start()
    {
        // random chance to disable grass and coconut
        if (Random.value > 0.5f)
        {
            grass.SetActive(false);
        }

        if (Random.value > 0.5f)
        {
            coconut.SetActive(false);
        }

        // Randomly flip X
        Vector3 localScale = transform.localScale;
        if (enableFlipX)
        {
            localScale.x *= Random.value > 0.5f ? -1 : 1;
        }
        transform.localScale = localScale;
    }
}
