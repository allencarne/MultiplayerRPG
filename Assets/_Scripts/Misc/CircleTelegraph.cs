using Unity.Netcode;
using UnityEngine;

public class CircleTelegraph : NetworkBehaviour
{
    [SerializeField] SpriteRenderer frontSprite;
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

    private void Update()
    {
        if (!IsServer) return;

        // Calculate the scale increment per frame to achieve the target scale in FillSpeed seconds
        float scaleIncrement = Time.deltaTime / FillSpeed;

        // Adjust the scale of frontSprite
        Vector3 currentScale = frontSprite.transform.localScale;
        float newScaleX = Mathf.Min(currentScale.x + scaleIncrement, 1f);
        float newScaleY = Mathf.Min(currentScale.y + scaleIncrement, 1f);

        // Set the new scale
        frontSprite.transform.localScale = new Vector3(newScaleX, newScaleY, currentScale.z);

        // Check if the scale has reached 1
        if (frontSprite.transform.localScale.x >= 1f && frontSprite.transform.localScale.y >= 1f)
        {
            Destroy(gameObject);
        }
    }

    void Destroy()
    {
        Destroy(gameObject);
    }
}