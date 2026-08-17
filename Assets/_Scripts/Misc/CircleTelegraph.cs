using Unity.Netcode;
using UnityEngine;

public class CircleTelegraph : NetworkBehaviour, ITelegraph
{
    [SerializeField] SpriteRenderer frontSprite;
    CharacterStats stats;
    float fillSpeed;

    public void Init(CharacterStats _stats, float _fillDuration)
    {
        stats = _stats;
        fillSpeed = _fillDuration;
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

        // Calculate the scale increment per frame to achieve the target scale in FillSpeed seconds
        float scaleIncrement = Time.deltaTime / fillSpeed;

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