using Unity.Netcode;
using UnityEngine;

public class Telegraph : NetworkBehaviour
{
    [SerializeField] private SpriteRenderer frontSprite;
    public float FillSpeed = 1f;

    [HideInInspector] public CrowdControl crowdControl;
    [HideInInspector] public Enemy enemy;

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

        if (crowdControl != null && crowdControl.IsInterrupted)
        {
            Destroy(gameObject);
            return;
        }

        if (enemy != null && enemy.isDead)
        {
            Destroy(gameObject);
            return;
        }

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
}
