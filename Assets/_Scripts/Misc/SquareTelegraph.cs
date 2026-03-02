using Unity.Netcode;
using UnityEngine;

public class SquareTelegraph : NetworkBehaviour
{
    [SerializeField] private SpriteRenderer frontSprite;
    [HideInInspector] public CharacterStats stats;

    [HideInInspector] public float FillSpeed;

    public void Init()
    {
        if (stats == null) return;
        stats.OnInterrupted.AddListener(Destroy);
        stats.OnDeath.AddListener(Destroy);
    }

    private void OnDisable()
    {
        if (stats == null) return;
        stats.OnInterrupted.RemoveListener(Destroy);
        stats.OnDeath.RemoveListener(Destroy);
    }

    private void Start()
    {
        if (frontSprite != null)
        {
            // Start with scale.x = 0
            frontSprite.transform.localScale = new Vector3(0f, frontSprite.transform.localScale.y, frontSprite.transform.localScale.z);
        }
    }

    private void Update()
    {
        if (!IsServer) return;

        float scaleIncrement = Time.deltaTime / FillSpeed;

        // Grow only along the X axis
        Vector3 currentScale = frontSprite.transform.localScale;
        float newScaleX = Mathf.Min(currentScale.x + scaleIncrement, 1f);

        frontSprite.transform.localScale = new Vector3(newScaleX, currentScale.y, currentScale.z);

        // Once fully filled, destroy
        if (newScaleX >= 1f)
        {
            Destroy(gameObject);
        }
    }

    void Destroy()
    {
        Destroy(gameObject);
    }
}
